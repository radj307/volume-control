using System;
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
using ToastifyAPI.Core;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Core.Broadcaster
{
    public class ToastifyBroadcaster : IToastifyBroadcaster
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastifyBroadcaster));

        private ClientWebSocket socket;
        private KestrelWebSocketHost<ToastifyWebSocketHostStartup> webHost;
        private Thread receiveMessageThread;

        private bool isSending;
        private bool isReceiving;

        #region Public Properties

        public uint Port { get; private set; }

        #endregion

        #region Events

        private event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        public ToastifyBroadcaster(uint port = 41348)
        {
            this.Port = port;
        }

        public async Task StartAsync()
        {
            await this.StartAsync(false);
        }

        public async Task StartAsync(bool restart)
        {
            bool messageThreadNeededToBeStopped = false;
            bool socketNeededToBeClosed = false;
            bool webHostNeededToBeStopped = false;

            if (restart || this.Port != this.webHost?.Uri.Port)
            {
                messageThreadNeededToBeStopped = this.StopReceiveMessageThread();
                socketNeededToBeClosed = await this.CloseSocket((WebSocketCloseStatus)1012);
                webHostNeededToBeStopped = await this.StopWebHost();
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

            if (this.webHost != null)
            {
                this.receiveMessageThread = new Thread(this.ReceiveMessageLoop)
                {
                    Name = "ToastifyBroadcaster_ReceiveMessageThread"
                };
                this.receiveMessageThread.Start();
            }
        }

        public async Task StopAsync()
        {
            bool messageThreadNeededToBeStopped = this.StopReceiveMessageThread();
            bool socketNeededToBeClosed = await this.CloseSocket(WebSocketCloseStatus.NormalClosure);
            bool webHostNeededToBeStopped = await this.StopWebHost();

            if (socketNeededToBeClosed || webHostNeededToBeStopped || messageThreadNeededToBeStopped)
                logger.Debug($"{nameof(ToastifyBroadcaster)} stopped!");
        }

        private async Task SendCommand(string command, params string[] args)
        {
            if (await this.EnsureConnection())
            {
                string argsString = string.Empty;
                if (args != null && args.Length > 0)
                    argsString = $" {string.Join(" ", args)}";

                byte[] bytes = Encoding.UTF8.GetBytes($"{command}{argsString}");

                // Wait until the previous message has been sent: only one outstanding send operation is allowed!
                await this.SendChannelAvailable();

                if (this.socket != null && this.socket.State == WebSocketState.Open)
                {
                    try
                    {
                        this.isSending = true;
                        await this.socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
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
                if (!await this.EnsureConnection())
                {
                    logger.Error("Couldn't establish a connection to the local WebSocket.");
                    return;
                }

                // Wait until the previous message has been received: only one outstanding receive operation is allowed!
                await this.ReceiveChannelAvailable();

                this.isReceiving = true;
                var buffer = new byte[4 * 1024];
                WebSocketReceiveResult receiveResult = await this.socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                this.isReceiving = false;

                string message = string.Empty;
                while (!receiveResult.CloseStatus.HasValue && await this.EnsureConnection())
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

                    if (await this.EnsureConnection())
                    {
                        await this.ReceiveChannelAvailable();
                        this.isReceiving = true;
                        receiveResult = await this.socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        this.isReceiving = false;
                    }
                }

                await this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
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

        private async Task<bool> EnsureConnection()
        {
            if (this.socket == null || this.webHost == null)
                return false;

            const int maxWaitMilliseconds = 5000;
            int currentWait = 0;

            while (this.socket?.State == WebSocketState.Connecting && currentWait <= maxWaitMilliseconds)
            {
                await Task.Delay(50);
                currentWait += 50;
            }

            while ((this.socket?.State == WebSocketState.CloseSent || this.socket?.State == WebSocketState.CloseReceived) && currentWait <= maxWaitMilliseconds)
            {
                await Task.Delay(50);
                currentWait += 50;
            }

            if (this.socket?.State != WebSocketState.Open && this.webHost != null && currentWait <= maxWaitMilliseconds)
            {
                Uri baseUri = this.webHost.Uri;
                var uriBuilder = new UriBuilder(baseUri)
                {
                    Scheme = "ws",
                    Path = ToastifyWebSocketHostStartup.INTERNAL_PATH
                };

                await this.socket.ConnectAsync(uriBuilder.Uri, CancellationToken.None);
            }

            return this.socket?.State == WebSocketState.Open;
        }

        private async Task SendChannelAvailable()
        {
            while (this.isSending)
            {
                await Task.Delay(50);
            }
        }

        private async Task ReceiveChannelAvailable()
        {
            while (this.isReceiving)
            {
                await Task.Delay(50);
            }
        }

        private async Task<bool> CloseSocket(WebSocketCloseStatus closeStatus)
        {
            if (this.socket != null)
            {
                try
                {
                    if (this.socket.State != WebSocketState.Open)
                        await this.socket.CloseAsync(closeStatus, string.Empty, CancellationToken.None);
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
                    await this.webHost.StopAsync(TimeSpan.FromSeconds(1));
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
                this.receiveMessageThread.Abort();
                this.receiveMessageThread = null;
                return true;
            }

            return false;
        }

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

            await this.SendCommand(command);
            this.MessageReceived += OnMessageReceived;

            Task waitResponse = Task.Run(() =>
            {
                while (response == null)
                {
                    Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            });
            await Task.WhenAny(waitResponse, Task.Delay(TimeSpan.FromSeconds(60)));
            this.MessageReceived -= OnMessageReceived;
            return response;
        }

        private async Task Broadcast(params string[] args)
        {
            await this.SendCommand("BROADCAST", args);
        }

        public async Task BroadcastCurrentSong<T>(T song) where T : ISong
        {
            string songJson = song != null ? JsonConvert.SerializeObject(new JsonSong(song)) : "null";
            await this.Broadcast("CURRENT-SONG", songJson);
        }

        public async Task BroadcastPlayState(bool playing)
        {
            await this.Broadcast("PLAY-STATE", $"{{ \"playing\": {JsonConvert.ToString(playing)} }}");
        }

        #endregion
    }
}