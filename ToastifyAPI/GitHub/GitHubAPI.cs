using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using ToastifyAPI.GitHub.Model;

namespace ToastifyAPI.GitHub
{
    public class GitHubAPI
    {
        private static string ApiBase { get; } = "https://api.github.com";

        public string Owner { get; set; }

        public string Repository { get; set; }

        private readonly Encoding encoding = Encoding.UTF8;

        public string GetFullEndpointUrl(string endpoint, params object[] args)
        {
            string ep = endpoint.Replace(":owner", this.Owner).Replace("{owner}", this.Owner)
                                .Replace(":repo", this.Repository).Replace("{repo}", this.Repository);
            return args != null ? $"{ApiBase}{string.Format(ep, args)}" : $"{ApiBase}{ep}";
        }

        public T DownloadJson<T>(string url) where T : BaseModel
        {
            using (HttpClient http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("User-Agent", "aleab/toastify");

                using (HttpResponseMessage response = http.GetAsync(url).Result)
                {
                    byte[] raw = response.Content.ReadAsByteArrayAsync().Result;
                    string json = raw.Length > 0 ? this.encoding.GetString(raw) : "{}";

                    T result = JsonConvert.DeserializeObject<T>(json);
                    result.HttpResponseHeaders = response.Headers;
                    result.HttpStatusCode = response.StatusCode;
                    return result;
                }
            }
        }
    }
}