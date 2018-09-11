using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Aleab.Common.Net.WebSockets;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Toastify.Core.Broadcaster
{
    public class ToastifyWebSocketHostStartup : BaseWebSocketHostStartup
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastifyWebSocketHostStartup));

        public const string INTERNAL_PATH = "/ws/toastify/internal";
        public const string CLIENTS_PATH = "/ws/toastify";

        private readonly IList<WebSocket> clients;
        private WebSocket toastifyBroadcasterSocket;

        public ToastifyWebSocketHostStartup(IHostingEnvironment env, IConfiguration config) : base(env, config)
        {
            this.clients = new List<WebSocket>(5);
        }

        private async Task WebSocketLoop(HttpContext context, WebSocket webSocket, Func<string, WebSocket, Task> messageHandler)
        {
            var buffer = new byte[this.ReceiveBufferSize];
            WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            string message = string.Empty;
            while (!receiveResult.CloseStatus.HasValue)
            {
                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    message += Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                    if (receiveResult.EndOfMessage)
                    {
                        if (messageHandler != null)
                            await messageHandler.Invoke(message, webSocket);
                        message = string.Empty;
                    }
                }
                else
                    message = string.Empty;

                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        private async Task HandleInternal(string message, WebSocket webSocket)
        {
            if (logger.IsDebugEnabled)
                logger.Debug($"[ToastifyBroadcaster] Internal message received: \"{message}\"");

            Match match;
            bool handled = false;
            switch (message)
            {
                case string s when (match = Regex.Match(s, "^BROADCAST (.+?) (.+)$", RegexOptions.Compiled)).Success:
                    string cmd = match.Groups[1].Value;
                    switch (match.Groups[1].Value)
                    {
                        case "CURRENT-SONG":
                            foreach (var client in this.clients)
                            {
                                await RedirectTo($"{cmd} {match.Groups[2].Value}", client);
                            }

                            handled = true;
                            break;

                        case "PLAY-STATE":
                            foreach (var client in this.clients)
                            {
                                await RedirectTo($"{cmd} {match.Groups[2].Value}", client);
                            }

                            handled = true;
                            break;

                        default:
                            break;
                    }

                    break;

                case "CLIENTS":
                    await RedirectTo(this.clients.Count.ToString(), webSocket);
                    handled = true;
                    break;

                default:
                    logger.Warn($"Unrecognized command received on {INTERNAL_PATH}: {message}");
                    break;
            }

            if (!handled)
                logger.Warn($"Unrecognized command received on {INTERNAL_PATH}: {message}");
        }

        private async Task HandleClients(string message, WebSocket clientWebSocket)
        {
            if (logger.IsDebugEnabled)
                logger.Debug($"[ToastifyBroadcaster] Message received from {clientWebSocket.GetHashCode()}: \"{message}\"");

            switch (message)
            {
                case "PING":
                    if (this.toastifyBroadcasterSocket != null)
                        await RedirectTo("PONG", clientWebSocket);
                    break;

                default:
                    logger.Warn($"Unrecognized command received on {CLIENTS_PATH}: {message}");
                    break;
            }
        }

        public override void ConfigureServices(IServiceCollection services)
        {
        }

        protected override async Task ConfigureWebSocketRequestPipeline(HttpContext context, Func<Task> next, WebSocket webSocket)
        {
            switch (context.Request.Path)
            {
                case INTERNAL_PATH:
                    await this.WebSocketLoop(context, webSocket, this.HandleInternal);
                    break;

                case CLIENTS_PATH:
                    await this.WebSocketLoop(context, webSocket, this.HandleClients);
                    break;

                default:
                    await next();
                    break;
            }
        }

        protected override bool ShouldAcceptConnection(HttpContext context, WebSocket webSocket)
        {
            if (webSocket == null || context == null)
                return false;

            switch (context.Request.Path)
            {
                case INTERNAL_PATH:
                    return this.toastifyBroadcasterSocket == null || this.toastifyBroadcasterSocket == webSocket;

                case CLIENTS_PATH:
                    return true;

                default:
                    return false;
            }
        }

        protected override void OnWebSocketConnected(HttpContext context, WebSocket webSocket)
        {
            base.OnWebSocketConnected(context, webSocket);

            switch (context.Request.Path)
            {
                case INTERNAL_PATH:
                    this.toastifyBroadcasterSocket = webSocket;
                    break;

                case CLIENTS_PATH:
                    int i = Array.FindIndex(this.clients.ToArray(), w => w == webSocket);
                    if (i < 0)
                    {
                        this.clients.Add(webSocket);
                        Task.Run(async () =>
                        {
                            JsonGreetingsObject greetings = new JsonGreetingsObject
                            {
                                Song = new JsonSong(Spotify.Instance.CurrentSong),
                                Playing = Spotify.Instance.IsPlaying
                            };
                            await RedirectTo($"HELLO {JsonConvert.SerializeObject(greetings)}", webSocket);
                        });
                    }

                    break;

                default:
                    break;
            }
        }

        protected override void OnWebSocketClosed(HttpContext context, WebSocket webSocket)
        {
            base.OnWebSocketClosed(context, webSocket);

            switch (context.Request.Path)
            {
                case INTERNAL_PATH:
                    if (this.toastifyBroadcasterSocket == webSocket)
                        this.toastifyBroadcasterSocket = null;
                    break;

                case CLIENTS_PATH:
                    int i = Array.FindIndex(this.clients.ToArray(), w => w == webSocket);
                    if (i >= 0)
                        this.clients.RemoveAt(i);
                    break;

                default:
                    break;
            }
        }

        #region Static Members

        private static async Task RedirectTo(string message, WebSocket socket)
        {
            if (socket != null && socket.State == WebSocketState.Open && !string.IsNullOrWhiteSpace(message))
                await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        #endregion
    }
}