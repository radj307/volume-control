using Newtonsoft.Json;
using System;

namespace ToastifyAPI.GitHub.Model
{
    [Serializable]
    public class Release : BaseModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("assets_url")]
        public string AssetsUrl { get; set; }

        [JsonProperty("upload_url")]
        public string UploadUrl { get; set; }

        [JsonProperty("tarball_url")]
        public string TarballUrl { get; set; }

        [JsonProperty("zipball_url")]
        public string ZipballUrl { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("target_commitish")]
        public string TargetCommitish { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("draft")]
        public bool IsDraft { get; set; }

        [JsonProperty("prerelease")]
        public bool IsPreRelease { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("published_at")]
        public DateTime? PublishedAt { get; set; }

        // TODO: Author object
        //[JsonProperty("author")]
        //public Author Author { get; set; }

        // TODO: Asset object
        //[JsonProperty("assets")]
        //public List<Asset> Assets { get; set; }
    }
}