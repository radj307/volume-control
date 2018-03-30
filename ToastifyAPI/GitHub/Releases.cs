using ToastifyAPI.GitHub.Model;

namespace ToastifyAPI.GitHub
{
    public static class Releases
    {
        public static Release GetReleaseByTagName(this GitHubAPI api, RepoInfo repo, string tag)
        {
            string url = api.GetFullEndpointUrl($"/repos/:owner/:repo/releases/tags/{tag}", repo);
            return api.DownloadJson<Release>(url);
        }

        public static Release GetLatestRelease(this GitHubAPI api, RepoInfo repo)
        {
            string url = api.GetFullEndpointUrl("/repos/:owner/:repo/releases/latest", repo);
            return api.DownloadJson<Release>(url);
        }

        public static string GetUrlOfLatestRelease(RepoInfo repo)
        {
            return repo.Format("https://github.com/:owner/:repo/releases/latest");
        }
    }
}