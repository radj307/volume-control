using JetBrains.Annotations;
using System;
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

        public static bool CheckInternetConnection([CanBeNull] IProxyConfig proxyConfig)
        {
            // NOTE: Not using PING as it might be blocked in some workplaces and schools

            var httpClientHandler = CreateHttpClientHandler(proxyConfig);
            if (!CheckConnectionToUri("http://clients3.google.com/generate_204", httpClientHandler))
            {
                // Google might be blocked in some countries (China?)
                if (!CheckConnectionToUri("https://github.com", httpClientHandler))
                {
                    // As last resort, try Spotify
                    return CheckConnectionToUri("https://www.spotify.com", httpClientHandler);
                }
            }

            return true;
        }

        private static bool CheckConnectionToUri([NotNull] string uriString, [NotNull] HttpMessageHandler httpClientHandler)
        {
            Uri uri = new Uri(uriString);
            return CheckConnectionToUri(uri, httpClientHandler);
        }

        private static bool CheckConnectionToUri([NotNull] Uri uri, [NotNull] HttpMessageHandler httpClientHandler)
        {
            try
            {
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, uri));
                    return response.Result.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}