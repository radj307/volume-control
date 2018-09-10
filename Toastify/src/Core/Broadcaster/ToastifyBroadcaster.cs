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
using Toastify.Model;

namespace Toastify.Core.Broadcaster
{
    public class ToastifyBroadcaster
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastifyBroadcaster));

        private readonly ClientWebSocket socket;
        private KestrelWebSocketHost<ToastifyWebSocketHostStartup> webHost;

        private bool webHostStarted;

        private Thread receiveMessageThread;

        #region Public Properties

        public uint Port { get; private set; }

        #endregion

        #region Events

        private event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        public ToastifyBroadcaster(uint port)
        {
            this.Port = port;
            this.webHost = new KestrelWebSocketHost<ToastifyWebSocketHostStartup>($"http://localhost:{this.Port}");
            this.socket = new ClientWebSocket();
        }

        private async Task<bool> EnsureConnection()
        {
            const int maxWaitMilliseconds = 5000;
            int currentWait = 0;

            while (this.socket.State == WebSocketState.Connecting && currentWait <= maxWaitMilliseconds)
            {
                currentWait += 50;
                await Task.Delay(50);
            }

            if (currentWait >= maxWaitMilliseconds)
                return false;

            currentWait = 0;
            while ((this.socket.State == WebSocketState.CloseSent || this.socket.State == WebSocketState.CloseReceived) && currentWait <= maxWaitMilliseconds)
            {
                currentWait += 50;
                await Task.Delay(50);
            }

            if (currentWait >= maxWaitMilliseconds)
                return false;

            if (this.socket.State != WebSocketState.Open)
            {
                Uri baseUri = this.webHost.Uri;
                var uriBuilder = new UriBuilder(baseUri)
                {
                    Scheme = "ws",
                    Path = ToastifyWebSocketHostStartup.INTERNAL_PATH
                };
                await this.socket.ConnectAsync(uriBuilder.Uri, CancellationToken.None);
            }

            return this.socket.State == WebSocketState.Open;
        }

        private async Task SendCommand(string command, params string[] args)
        {
            if (await this.EnsureConnection())
            {
                string argsString = string.Empty;
                if (args != null && args.Length > 0)
                    argsString = $" {string.Join(" ", args)}";

                byte[] bytes = Encoding.UTF8.GetBytes($"{command}{argsString}");
                await this.socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async void ReceiveMessageLoop()
        {
            if (!await this.EnsureConnection())
            {
                logger.Error("Couldn't establish a connection to the local WebSocket.");
                return;
            }

            var buffer = new byte[4 * 1024];
            WebSocketReceiveResult receiveResult = await this.socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

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

                receiveResult = await this.socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        public async Task StartAsync()
        {
            if (!this.webHostStarted)
            {
                try
                {
                    this.webHost.Start();
                }
                catch (AggregateException e)
                {
                    if (e.Message.Contains("EADDRINUSE"))
                    {
                        this.Port = (uint)GetFreeTcpPort();
                        this.webHost = new KestrelWebSocketHost<ToastifyWebSocketHostStartup>($"http://localhost:{this.Port}");
                        this.webHost.Start();
                    }
                    else
                        throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
                this.webHostStarted = true;
            }

            if (this.receiveMessageThread == null)
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
            await this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            await this.webHost.StopAsync();
            if (this.receiveMessageThread != null)
            {
                this.receiveMessageThread.Abort();
                this.receiveMessageThread = null;
            }
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

        public async Task BroadcastCurrentSong(Song song)
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