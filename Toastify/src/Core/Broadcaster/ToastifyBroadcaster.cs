using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Aleab.Common.Net.WebSockets;
using log4net;
using Newtonsoft.Json;
using Toastify.Threading;
using ToastifyAPI.Core;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Core.Broadcaster
{
    public class ToastifyBroadcaster : IToastifyBroadcaster, IDisposable
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastifyBroadcaster));

        #region Static Fields and Properties

        public static JsonSerializerSettings JsonSerializerSettings { get; }

        #endregion

        private ClientWebSocket socket;
        private KestrelWebSocketHost<ToastifyWebSocketHostStartup> webHost;
        private Thread receiveMessageThread;
        private CancellationTokenSource cts;

        private bool isSending;
        private bool isReceiving;
        private bool isConnecting;

        #region Public Properties

        public uint Port { get; private set; }

        #endregion

        #region Events

        private event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        static ToastifyBroadcaster()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.Default,
                FloatParseHandling = FloatParseHandling.Decimal,
                FloatFormatHandling = FloatFormatHandling.String,
                DateParseHandling = DateParseHandling.DateTime,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                Formatting = Formatting.None,
                MaxDepth = null,
                Culture = CultureInfo.InvariantCulture,
                ConstructorHandling = ConstructorHandling.Default,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                MetadataPropertyHandling = MetadataPropertyHandling.Default,
                TypeNameHandling = TypeNameHandling.None,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public ToastifyBroadcaster() : this(41348)
        {
        }

        public ToastifyBroadcaster(uint port)
        {
            this.Port = port;
        }

        public async Task StartAsync()
        {
            await this.StartAsync(false).ConfigureAwait(false);
        }

        public async Task StartAsync(bool restart)
        {
            bool messageThreadNeededToBeStopped = false;
            bool socketNeededToBeClosed = false;
            bool webHostNeededToBeStopped = false;

            if (restart || this.Port != this.webHost?.Uri.Port)
            {
                messageThreadNeededToBeStopped = this.StopReceiveMessageThread();
                socketNeededToBeClosed = await this.CloseSocket((WebSocketCloseStatus)1012).ConfigureAwait(false);
                webHostNeededToBeStopped = await this.StopWebHost().ConfigureAwait(false);
            }

            // Create a new web host and start it
            if (this.webHost == null)
            {
                this.webHost = new KestrelWebSocketHost<ToastifyWebSocketHostStartup>($"http://localhost:{this.Port}");
                try
                {
                    this.webHost.Start();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("EADDRINUSE"))
                    {
                        this.Port = (uint)GetFreeTcpPort();
                        this.webHost = new KestrelWebSocketHost<ToastifyWebSocketHostStartup>($"http://localhost:{this.Port}");
                        this.webHost.Start();
                    }
                    else
                    {
                        this.webHost = null;
                        logger.Error("Unhandled exception while starting the web host.", e);
                    }
                }

                // Create a new internal socket
                if (this.webHost != null && this.socket == null)
                {
                    this.socket = new ClientWebSocket();

                    if (messageThreadNeededToBeStopped || socketNeededToBeClosed || webHostNeededToBeStopped)
                        logger.Debug($"{nameof(ToastifyBroadcaster)} restarted!");
                    else
                        logger.Debug($"{nameof(ToastifyBroadcaster)} started!");
                }
            }

            this.cts?.Dispose();
            this.cts = new CancellationTokenSource();

            if (this.webHost != null)
            {
                this.receiveMessageThread = ThreadManager.Instance.CreateThread(this.ReceiveMessageLoop);
                this.receiveMessageThread.IsBackground = true;
                this.receiveMessageThread.Name = $"{nameof(ToastifyBroadcaster)}_ReceiveMessageThread";
                this.receiveMessageThread.Start();
            }
        }

        public async Task StopAsync()
        {
            bool messageThreadNeededToBeStopped = this.StopReceiveMessageThread();
            bool socketNeededToBeClosed = await this.CloseSocket(WebSocketCloseStatus.NormalClosure).ConfigureAwait(false);
            bool webHostNeededToBeStopped = await this.StopWebHost().ConfigureAwait(false);

            if (socketNeededToBeClosed || webHostNeededToBeStopped || messageThreadNeededToBeStopped)
                logger.Debug($"{nameof(ToastifyBroadcaster)} stopped!");
        }

        private async Task SendCommand(string command, params string[] args)
        {
            if (await this.EnsureConnection(this.cts.Token).ConfigureAwait(false))
            {
                string argsString = string.Empty;
                if (args != null && args.Length > 0)
                    argsString = $" {string.Join(" ", args)}";

                byte[] bytes = Encoding.UTF8.GetBytes($"{command}{argsString}");

                // Wait until the previous message has been sent: only one outstanding send operation is allowed!
                await this.SendChannelAvailable(this.cts.Token).ConfigureAwait(false);
                if (this.cts.IsCancellationRequested)
                    return;

                if (this.socket != null && this.socket.State == WebSocketState.Open)
                {
                    try
                    {
                        this.isSending = true;
                        await this.socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, this.cts.Token).ConfigureAwait(false);
                    }
                    finally
                    {
                        this.isSending = false;
                    }
                }
            }
        }

        private async void ReceiveMessageLoop()
        {
            try
            {
                if (!await this.EnsureConnection(this.cts.Token).ConfigureAwait(false))
                {
                    logger.Error("Couldn't establish a connection to the local WebSocket.");
                    return;
                }

                // Wait until the previous message has been received: only one outstanding receive operation is allowed!
                await this.ReceiveChannelAvailable(this.cts.Token).ConfigureAwait(false);
                if (this.cts.IsCancellationRequested)
                    return;

                this.isReceiving = true;
                var buffer = new byte[4 * 1024];
                if (this.socket.State != WebSocketState.Open)
                    return;

                WebSocketReceiveResult receiveResult = await this.socket.ReceiveAsync(new ArraySegment<byte>(buffer), this.cts.Token).ConfigureAwait(false);
                this.isReceiving = false;

                string message = string.Empty;
                while (!this.cts.IsCancellationRequested && !receiveResult.CloseStatus.HasValue && await this.EnsureConnection(this.cts.Token).ConfigureAwait(false))
                {
                    if (receiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        message += Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                        if (receiveResult.EndOfMessage)
                        {
                            this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
                            message = string.Empty;
                        }
                    }
                    else
                        message = string.Empty;

                    if (await this.EnsureConnection(this.cts.Token).ConfigureAwait(false))
                    {
                        await this.ReceiveChannelAvailable(this.cts.Token).ConfigureAwait(false);
                        if (!this.cts.IsCancellationRequested)
                        {
                            this.isReceiving = true;
                            receiveResult = await this.socket.ReceiveAsync(new ArraySegment<byte>(buffer), this.cts.Token).ConfigureAwait(false);
                            this.isReceiving = false;
                        }
                    }
                }

                await this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).ConfigureAwait(false);
            }
            catch (ThreadAbortException)
            {
                // ignore
            }
            catch (Exception e)
            {
                if (this.receiveMessageThread != null)
                    logger.Warn($"Unhandled exception in {nameof(this.ReceiveMessageLoop)}.", e);
            }
            finally
            {
                this.isReceiving = false;
            }
        }

        private async Task<bool> EnsureConnection(CancellationToken cancellationToken)
        {
            if (this.socket == null || this.webHost == null)
                return false;

            try
            {
                await this.Connection(cancellationToken).ConfigureAwait(false);
                if (cancellationToken.IsCancellationRequested)
                    return false;

                this.isConnecting = true;

                const int maxWaitMilliseconds = 5000;
                int currentWait = 0;

                while (this.socket?.State == WebSocketState.Connecting && currentWait <= maxWaitMilliseconds)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return false;
                    await Task.Delay(50, cancellationToken).ConfigureAwait(false);
                    currentWait += 50;
                }

                while ((this.socket?.State == WebSocketState.CloseSent || this.socket?.State == WebSocketState.CloseReceived) && currentWait <= maxWaitMilliseconds)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return false;
                    await Task.Delay(50, cancellationToken).ConfigureAwait(false);
                    currentWait += 50;
                }

                const int maxOpenStateWait = 1000;
                while (this.socket?.State != WebSocketState.Open && currentWait <= maxWaitMilliseconds && currentWait <= maxOpenStateWait)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return false;
                    await Task.Delay(50, cancellationToken).ConfigureAwait(false);
                    currentWait += 50;
                }

                if (cancellationToken.IsCancellationRequested || this.socket == null || this.webHost == null)
                    return false;

                if (this.socket?.State != WebSocketState.Open && currentWait <= maxWaitMilliseconds)
                {
                    Uri baseUri = this.webHost.Uri;
                    var uriBuilder = new UriBuilder(baseUri)
                    {
                        Scheme = "ws",
                        Path = ToastifyWebSocketHostStartup.InternalPath
                    };

                    await this.socket.ConnectAsync(uriBuilder.Uri, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                logger.Warn("Unhandled exception while ensuring connection to the WebSocket.", e);
            }
            finally
            {
                this.isConnecting = false;
            }

            return this.socket?.State == WebSocketState.Open;
        }

        private async Task Connection(CancellationToken cancellationToken)
        {
            while (this.isConnecting && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(50, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task SendChannelAvailable(CancellationToken cancellationToken)
        {
            while (this.isSending && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(50, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task ReceiveChannelAvailable(CancellationToken cancellationToken)
        {
            while (this.isReceiving && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(50, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<bool> CloseSocket(WebSocketCloseStatus closeStatus)
        {
            if (this.socket != null)
            {
                try
                {
                    if (this.socket.State != WebSocketState.Open)
                        await this.socket.CloseAsync(closeStatus, string.Empty, CancellationToken.None).ConfigureAwait(false);
                    this.socket?.Abort();
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception e)
                {
                    if (e.InnerException is OperationCanceledException)
                    {
                    }

                    logger.Error("Unhandled exception while closing the socket.", e);
                }
                finally
                {
                    this.socket = null;
                }

                return true;
            }

            return false;
        }

        private async Task<bool> StopWebHost()
        {
            if (this.webHost != null)
            {
                try
                {
                    await this.webHost.StopAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    logger.Error("Unhandled exception while stopping the web host instance.", e);
                }
                finally
                {
                    this.webHost = null;
                }

                return true;
            }

            return false;
        }

        private bool StopReceiveMessageThread()
        {
            if (this.receiveMessageThread != null)
            {
                this.cts.Cancel();
                if (!this.receiveMessageThread.Join(TimeSpan.FromSeconds(2)))
                    this.receiveMessageThread.Abort();
                this.receiveMessageThread = null;
                return true;
            }

            return false;
        }

        #region Dispose

        public async void Dispose()
        {
            await this.StopAsync();
            this.socket?.Dispose();
            this.cts?.Dispose();
        }

        #endregion

        #region Static Members

        private static int GetFreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        #endregion

        #region Commands

        private async Task<T> WaitForResponseCommand<T>(string command, string responseCommand) where T : class
        {
            T response = null;

            var regex = new Regex($"^{responseCommand} (.+)$", RegexOptions.Compiled);

            void OnMessageReceived(object sender, MessageReceivedEventArgs e)
            {
                if (e == null)
                    return;

                Match match = regex.Match(e.Message);
                if (match.Success)
                    response = JsonConvert.DeserializeObject<T>(match.Result("$1"));
            }

            await this.SendCommand(command).ConfigureAwait(false);
            this.MessageReceived += OnMessageReceived;

            Task waitResponse = Task.Run(() =>
            {
                while (response == null)
                {
                    Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            });
            await Task.WhenAny(waitResponse, Task.Delay(TimeSpan.FromSeconds(60))).ConfigureAwait(false);
            this.MessageReceived -= OnMessageReceived;
            return response;
        }

        private async Task Broadcast(params string[] args)
        {
            await this.SendCommand("BROADCAST", args).ConfigureAwait(false);
        }

        public async Task BroadcastCurrentTrack<T>(T track) where T : ISpotifyTrack
        {
            string trackJson = track != null ? JsonConvert.SerializeObject(new JsonTrack(track), JsonSerializerSettings) : "null";
            await this.Broadcast("CURRENT-TRACK", trackJson).ConfigureAwait(false);
        }

        public async Task BroadcastPlayState(bool playing)
        {
            await this.Broadcast("PLAY-STATE", $"{{ \"playing\": {JsonConvert.ToString(playing)} }}").ConfigureAwait(false);
        }

        #endregion
    }
}