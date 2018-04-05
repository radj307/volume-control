using JetBrains.Annotations;
using System.Net;
using System.Net.Http;
using ToastifyAPI.Core;

namespace ToastifyAPI.Helpers
{
    public static class Net
    {
        public static HttpClientHandler CreateHttpClientHandler()
        {
            return CreateHttpClientHandler(null);
        }

        public static HttpClientHandler CreateHttpClientHandler([CanBeNull] IProxyConfig proxyConfig)
        {
            HttpClientHandler clientHandler = new HttpClientHandler
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
    }
}