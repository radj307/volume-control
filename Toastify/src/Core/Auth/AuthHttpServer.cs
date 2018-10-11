using System;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Toastify.Common;
using Toastify.Threading;
using ToastifyAPI.Core.Auth;
using ToastifyWebAuthAPI_Utils = ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Utils;

namespace Toastify.Core.Auth
{
    public class AuthHttpServer : IAuthHttpServer, IDisposable
    {
        private readonly IWebHost webHost;
        private readonly NamedPipeServerStream pipe;
        private readonly CancellationTokenSource cts;

        private Thread receiveThread;

        #region Events

        public event EventHandler<AuthEventArgs> AuthorizationFinished;

        #endregion

        public AuthHttpServer()
        {
            // Create Named Pipe
            string pipeName = $"Toastify_{nameof(AuthHttpServer)}_Pipe_{RuntimeHelpers.GetHashCode(this)}";
            this.pipe = new NamedPipeServerStream(pipeName, PipeDirection.In, 1);

            string url = ToastifyWebAuthAPI_Utils.GetRedirectUri();
            this.webHost = new WebHostBuilder()
                          .UseKestrel()
                          .UseSetting("url", url)
                          .UseSetting("pipeName", pipeName)
                          .UseStartup<AuthHttpServerStartup>()
                          .UseUrls(url)
                          .Build();

            this.cts = new CancellationTokenSource();
        }

        public async Task Start()
        {
            await this.webHost.StartAsync(this.cts.Token).ConfigureAwait(false);

            this.receiveThread = ThreadManager.Instance.CreateThread(this.ReceiveThread);
            this.receiveThread.IsBackground = true;
            this.receiveThread.Name = $"Toastify_{nameof(AuthHttpServer)}_ReceiveThread_{RuntimeHelpers.GetHashCode(this)}";
            this.receiveThread.Start();
        }

        public Task Stop()
        {
            return this.webHost.StopAsync(this.cts.Token);
        }

        private async void ReceiveThread()
        {
            try
            {
                await this.pipe.WaitForConnectionAsync(this.cts.Token).ConfigureAwait(false);
                if (this.cts.IsCancellationRequested)
                    return;

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
            }
        }

        private void OnAuthorizationFinished(string code, string state, string error)
        {
            this.AuthorizationFinished?.Invoke(this, new AuthEventArgs(code, state, error));
        }

        #region Dispose

        public void Dispose()
        {
            this.Dispose(TimeSpan.FromSeconds(1));
        }

        public void Dispose(TimeSpan timeout)
        {
            try
            {
                this.cts.Cancel();
                this.receiveThread.Join(timeout);
            }
            catch
            {
                // ignore
            }

            this.pipe?.Dispose();
            this.webHost?.Dispose();

            this.cts.Dispose();
        }

        #endregion
    }
}