using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using ToastifyAPI.Core;
using ToastifyAPI.GitHub.Model;
using ToastifyAPI.Helpers;

namespace ToastifyAPI.GitHub
{
    public class GitHubAPI
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        private static List<Emoji> emojis;

        private static string ApiBase { get; } = "https://api.github.com";

        public string Owner { get; set; }

        public string Repository { get; set; }

        private readonly IProxyConfig proxyConfig;

        public GitHubAPI(IProxyConfig proxyConfig)
        {
            this.proxyConfig = proxyConfig;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public string GetFullEndpointUrl(string endpoint, params object[] args)
        {
            string ep = endpoint.Replace(":owner", this.Owner).Replace("{owner}", this.Owner)
                                .Replace(":repo", this.Repository).Replace("{repo}", this.Repository);
            return args != null ? $"{ApiBase}{string.Format(ep, args)}" : $"{ApiBase}{ep}";
        }

        public T DownloadJson<T>(string url) where T : BaseModel
        {
            HttpClientHandler httpClientHandler = Net.CreateHttpClientHandler(this.proxyConfig);
            using (HttpClient http = new HttpClient(httpClientHandler))
            {
                AddDefaultHeaders(http);

                using (HttpResponseMessage response = http.GetAsync(url).Result)
                {
                    byte[] raw = response.Content.ReadAsByteArrayAsync().Result;
                    string json = raw.Length > 0 ? encoding.GetString(raw) : "{}";

                    T result = JsonConvert.DeserializeObject<T>(json);
                    result.HttpResponseHeaders = response.Headers;
                    result.HttpStatusCode = response.StatusCode;
                    return result;
                }
            }
        }

        private T DownloadJsonInternal<T>(string url)
        {
            HttpClientHandler httpClientHandler = Net.CreateHttpClientHandler(this.proxyConfig);
            using (HttpClient http = new HttpClient(httpClientHandler))
            {
                AddDefaultHeaders(http);

                using (HttpResponseMessage response = http.GetAsync(url).Result)
                {
                    byte[] raw = response.Content.ReadAsByteArrayAsync().Result;
                    string json = raw.Length > 0 ? encoding.GetString(raw) : "{}";

                    T result = JsonConvert.DeserializeObject<T>(json);
                    return result;
                }
            }
        }

        #region GitHubify

        private static readonly Regex mentionRegex = new Regex(@"\B@(\w+)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex issueOrPullRegex = new Regex(@"\B#([0-9]+)\b", RegexOptions.Compiled);

        private static readonly Regex hashRegex = new Regex(@"\b([a-f0-9]{7,})\b", RegexOptions.Compiled);

        private static readonly Regex emojiRegex = new Regex(@":([^\s]+):", RegexOptions.Compiled);

        public string GitHubify(string ghText)
        {
            // Mentions (@)
            ghText = mentionRegex.Replace(ghText, match =>
            {
                string username = match.Groups[1].Value;
                string pattern = this.GitHubify_Mention(username);
                return match.Result(pattern);
            });

            // Issues or PRs (#)
            ghText = issueOrPullRegex.Replace(ghText, match =>
            {
                string sNumber = match.Groups[1].Value;
                int number = int.Parse(sNumber);
                string pattern = this.GitHubify_IssueOrPull(number);
                return match.Result(pattern);
            });

            // Hashes ([a-f0-9]{7,})
            ghText = hashRegex.Replace(ghText, match =>
            {
                string hash = match.Groups[1].Value;
                string pattern = this.GitHubify_Hash(hash);
                return match.Result(pattern);
            });

            // Emojis (::)
            ghText = emojiRegex.Replace(ghText, match =>
            {
                string emojiName = match.Groups[1].Value;
                string pattern = this.GitHubify_Emoji(emojiName);
                return match.Result(pattern);
            });

            return ghText;
        }

        /// <summary>
        /// Returns a replace pattern for a mention to the specified username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>A regex replace pattern</returns>
        private string GitHubify_Mention(string username)
        {
            string url = $"https://github.com/{username}";

            WebRequest webRequest = CreateWebRequest(url, this.proxyConfig);
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                return $"[@$1]({url})";
            }
            catch
            {
                return "@$1";
            }
            finally
            {
                response?.Close();
            }
        }

        /// <summary>
        /// Returns a replace pattern for the specified issue or PR number
        /// </summary>
        /// <param name="number">The issue or PR number</param>
        /// <param name="pull">Whether it's a pull request or not</param>
        /// <returns>A regex replace pattern</returns>
        private string GitHubify_IssueOrPull(int number, bool pull = false)
        {
            string s = $"{(pull ? "pull" : "issues")}/{number}";
            string url = $"https://github.com/aleab/toastify/{s}";

            WebRequest webRequest = CreateWebRequest(url, this.proxyConfig);
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                return $"[#$1]({url})";
            }
            catch
            {
                return pull ? "#$1" : this.GitHubify_IssueOrPull(number, true);
            }
            finally
            {
                response?.Close();
            }
        }

        /// <summary>
        /// Returns a replace pattern for the specified commit hash
        /// </summary>
        /// <param name="hash">The commit hash</param>
        /// <returns>A regex replace pattern</returns>
        private string GitHubify_Hash(string hash)
        {
            string url = $"https://github.com/aleab/toastify/commit/{hash}";

            WebRequest webRequest = CreateWebRequest(url, this.proxyConfig);
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                return $"[`$1`]({url})";
            }
            catch
            {
                return "$_";
            }
            finally
            {
                response?.Close();
            }
        }

        /// <summary>
        /// Returns a replace pattern for the specified emoji
        /// </summary>
        /// <param name="emojiName">The emoji name</param>
        /// <returns>A regex replace pattern</returns>
        private string GitHubify_Emoji(string emojiName)
        {
            // TODO: Use inline images instead of unicode characters, which are just rendered in B/W
            if (emojis == null)
            {
                const string url = "https://api.github.com/emojis";
                var list = this.DownloadJsonInternal<Dictionary<string, string>>(url);
                emojis = list.Count <= 0
                    ? new List<Emoji>(0)
                    : list.Select(kvp => new Emoji { Name = kvp.Key, Url = kvp.Value }).ToList();
            }
            if (emojis.Count <= 0)
                return string.Empty;

            Emoji emoji = emojis.SingleOrDefault(e => e.Name == emojiName);
            if (emoji == null)
                return "$_";

            string unicodeString = emoji.GetAsUnicodeString();
            return string.IsNullOrWhiteSpace(unicodeString) ? string.Empty : unicodeString;
        }

        #endregion GitHubify

        private static WebRequest CreateWebRequest(string url, IProxyConfig proxyConfig = null)
        {
            WebRequest webRequest = WebRequest.Create(url);
            AddDefaultHeaders(webRequest);
            webRequest.Method = HttpMethod.Head.Method;
            webRequest.Proxy = proxyConfig?.CreateWebProxy();
            webRequest.Timeout = 10000;

            return webRequest;
        }

        private static void AddDefaultHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "aleab/toastify");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "aleab/toastify");
        }

        private static void AddDefaultHeaders(WebRequest webRequest)
        {
            // Cannot set User-Agent and Referer headers for WebRequest's
            //webRequest.Headers[HttpRequestHeader.UserAgent] = "aleab/toastify";
            //webRequest.Headers[HttpRequestHeader.Referer] = "aleab/toastify";
        }
    }
}