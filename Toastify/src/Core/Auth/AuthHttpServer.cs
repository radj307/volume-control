using System;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Toastify.Common;
using ToastifyAPI.Core.Auth;
using ToastifyWebAuthAPI_Utils = ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Utils;

namespace Toastify.Core.Auth
{
    public class AuthHttpServer : IAuthHttpServer, IDisposable
    {
        private readonly IWebHost webHost;
        private readonly NamedPipeServerStream pipe;

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
        }

        public async Task Start()
        {
            await this.webHost.StartAsync().ConfigureAwait(false);
            this.receiveThread = new Thread(this.ReceiveThread)
            {
                Name = $"Toastify_{nameof(AuthHttpServer)}_ReceiveThread_{RuntimeHelpers.GetHashCode(this)}"
            };
            this.receiveThread.Start();
        }

        public Task Stop()
        {
            return this.webHost.StopAsync();
        }

        private void ReceiveThread()
        {
            try
            {
                this.pipe.WaitForConnection();

                StringStream ss = new StringStream(this.pipe);
                string responseString = ss.ReadString();
                var response = HttpUtility.ParseQueryString(responseString);

                string code = response.Get("code");
                string state = response.Get("state");
                string error = response.Get("error");

                this.pipe.Close();

                this.OnAuthorizationFinished(code, state, error);
            }
            catch
            {
                // ignore
            }
        }

        public void Dispose()
        {
            this.receiveThread?.Abort();
            this.pipe?.Dispose();
            this.webHost?.Dispose();
        }

        private void OnAuthorizationFinished(string code, string state, string error)
        {
            this.AuthorizationFinished?.Invoke(this, new AuthEventArgs(code, state, error));
        }
    }
}