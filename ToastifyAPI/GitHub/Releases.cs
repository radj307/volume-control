using ToastifyAPI.GitHub.Model;

namespace ToastifyAPI.GitHub
{
    public static class Releases
    {
        public static Release GetReleaseByTagName(this GitHubAPI api, string tag)
        {
            string url = api.GetFullEndpointUrl($"/repos/:owner/:repo/releases/tags/{tag}");
            return api.DownloadJson<Release>(url);
        }

        public static Release GetLatestRelease(this GitHubAPI api)
        {
            string url = api.GetFullEndpointUrl("/repos/:owner/:repo/releases/latest");
            return api.DownloadJson<Release>(url);
        }

        public static string GetUrlOfLatestRelease(this GitHubAPI api)
        {
            return api.GetFullEndpointUrl("https://github.com/:owner/:repo/releases/latest");
        }
    }
}