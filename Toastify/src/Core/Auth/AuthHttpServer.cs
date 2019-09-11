using System;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Toastify.Common;
using Toastify.Threading;
using ToastifyAPI.Core.Auth;
using ToastifyWebAuthAPI_Utils = ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Utils;

namespace Toastify.Core.Auth
{
    public class AuthHttpServer : IAuthHttpServer, IDisposable
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AuthHttpServer));

        private readonly IWebHost webHost;
        private readonly NamedPipeServerStream pipe;
        private CancellationTokenSource cts;

        private Thread receiveThread;
        private Timer timeoutTimer;

        #region Public Properties

        public int Port { get; }

        #endregion

        #region Events

        public event EventHandler<AuthEventArgs> AuthorizationFinished;

        #endregion

        public AuthHttpServer()
        {
            this.Port = GetFreeTcpPort();

            // Create Named Pipe
            string pipeName = $"Toastify_{nameof(AuthHttpServer)}_Pipe_{RuntimeHelpers.GetHashCode(this)}";
            this.pipe = new NamedPipeServerStream(pipeName, PipeDirection.In, 1);

            string url = $"http://localhost:{this.Port}";
            this.webHost = new WebHostBuilder()
                          .UseKestrel()
                          .UseSetting("url", url)
                          .UseSetting("pipeName", pipeName)
                          .UseStartup<AuthHttpServerStartup>()
                          .UseUrls(url)
                          .Build();
        }

        public async Task Start()
        {
            this.cts?.Cancel();
            this.cts = new CancellationTokenSource();

            try
            {
                await this.webHost.StartAsync(this.cts.Token).ConfigureAwait(false);
            }
            catch
            {
                // ignore
            }

            this.receiveThread = ThreadManager.Instance.CreateThread(this.ReceiveThread);
            this.receiveThread.IsBackground = true;
            this.receiveThread.Name = $"Toastify_{nameof(AuthHttpServer)}_ReceiveThread_{RuntimeHelpers.GetHashCode(this)}";
            this.receiveThread.Start();

            this.ConfigureTimeoutTermination();
        }

        public Task Stop()
        {
            return this.webHost.StopAsync(this.cts.Token);
        }

        private void ConfigureTimeoutTermination()
        {
            this.timeoutTimer?.Dispose();
            this.timeoutTimer = new Timer(this.TimeoutTerminationCallback, null, TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);
        }

        private async void TimeoutTerminationCallback(object state)
        {
            logger.Warn($"Terminating {nameof(AuthHttpServer)} due to timeout.");
            try
            {
                await this.Stop();
                this.cts.Cancel();
                ThreadManager.Instance.Abort(this.receiveThread);
            }
            catch
            {
                // ignore
            }
        }

        private async void ReceiveThread()
        {
            try
            {
                logger.Debug($"{nameof(this.ReceiveThread)} started ({this.receiveThread.Name})");

                await this.pipe.WaitForConnectionAsync(this.cts.Token).ConfigureAwait(false);
                if (this.cts.IsCancellationRequested)
                    return;

                logger.Debug($"[{nameof(this.ReceiveThread)}] Pipe connection established!");
                StringStream ss = new StringStream(this.pipe);
                string responseString = ss.ReadString();
                var response = HttpUtility.ParseQueryString(responseString);

                string code = response.Get("code");
                string state = response.Get("state");
                string error = response.Get("error");

                this.OnAuthorizationFinished(code, state, error);
            }
            finally
            {
                this.pipe.Close();
                logger.Debug($"{nameof(this.ReceiveThread)} ended!");
            }
        }

        private void OnAuthorizationFinished(string code, string state, string error)
        {
            logger.Debug($"Authorization finished!{(string.IsNullOrEmpty(error) ? "" : $" Error: \"{error}\"")}");
            this.timeoutTimer.Dispose();
            this.AuthorizationFinished?.Invoke(this, new AuthEventArgs(code, state, error));
        }

        #region Static Members

        private static int GetFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            this.Dispose(TimeSpan.FromSeconds(1));
        }

        public void Dispose(TimeSpan timeout)
        {
            try
            {
                this.cts?.Cancel();
                this.receiveThread.Join(timeout);
            }
            catch
            {
                // ignore
            }

            this.pipe?.Dispose();
            this.webHost?.Dispose();

            this.cts?.Dispose();
        }

        #endregion
    }
}