using ToastifyAPI.GitHub.Model;

namespace ToastifyAPI.GitHub
{
    public static class Releases
    {
        public static Release GetByTagName(this GitHubAPI api, string tag)
        {
            string url = api.GetFullEndpointUrl($"/repos/:owner/:repo/releases/tags/{tag}");
            return api.DownloadJson<Release>(url);
        }
    }
}