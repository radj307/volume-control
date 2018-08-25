using System.Net;
using System.Net.Http;
using JetBrains.Annotations;
using ToastifyAPI.Core;

namespace ToastifyAPI.Helpers
{
    public static class Net
    {
        #region Static Members

        public static HttpClientHandler CreateHttpClientHandler()
        {
            return CreateHttpClientHandler(null);
        }

        public static HttpClientHandler CreateHttpClientHandler([CanBeNull] IProxyConfig proxyConfig)
        {
            var clientHandler = new HttpClientHandler
            {
                PreAuthenticate = false,
                UseDefaultCredentials = true,
                UseProxy = false
            };

            if (proxyConfig?.IsValid() == true)
            {
                IWebProxy proxy = proxyConfig.CreateWebProxy();
                if (proxy != null)
                {
                    clientHandler.UseProxy = true;
                    clientHandler.Proxy = proxy;
                    clientHandler.UseDefaultCredentials = proxyConfig.UseDefaultCredentials;
                    clientHandler.PreAuthenticate = proxyConfig.UseDefaultCredentials;
                }
            }

            return clientHandler;
        }

        #endregion
    }
}