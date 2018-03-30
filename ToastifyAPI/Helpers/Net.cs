using System.Net;
using System.Net.Http;
using ToastifyAPI.Core;

namespace ToastifyAPI.Helpers
{
    public static class Net
    {
        public static HttpClientHandler CreateHttpClientHandler(IProxyConfig proxyConfig = null)
        {
            HttpClientHandler clientHandler = new HttpClientHandler
            {
                PreAuthenticate = false,
                UseDefaultCredentials = true,
                UseProxy = false
            };

            if (!string.IsNullOrWhiteSpace(proxyConfig?.Host))
            {
                IWebProxy proxy = proxyConfig.CreateWebProxy();
                clientHandler.UseProxy = true;
                clientHandler.Proxy = proxy;
                clientHandler.UseDefaultCredentials = proxyConfig.UseDefaultCredentials;
                clientHandler.PreAuthenticate = proxyConfig.UseDefaultCredentials;
            }

            return clientHandler;
        }
    }
}