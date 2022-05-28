namespace VolumeControl.Helpers.Update
{
    /// <summary>Wrapper object used when deserializing JSON responses from the github API.</summary>
    public struct GithubReleaseHttpResponse
    {
        /// <summary>API url.</summary>
        public string url { get; set; }
        /// <summary>API assets url.</summary>
        public string assets_url { get; set; }
        /// <summary>API target url for uploading assets.</summary>
        public string upload_url { get; set; }
        /// <summary>URL of the regular HTML release page.</summary>
        /// <remarks>This should be the url passed to the user.</remarks>
        public string html_url { get; set; }
        /// <summary>Release ID</summary>
        public decimal id { get; set; }
        /// <summary>Information about the author of the release.</summary>
        public object? author { get; set; }
        /// <summary>?</summary>
        public string node_id { get; set; }
        /// <summary>Git tag that this release is attached to.</summary>
        public string tag_name { get; set; }
        /// <summary>Git commitish that this release is attached to.</summary>
        public string target_commitish { get; set; }
        /// <summary>The title/name of this release, as seen on github.com</summary>
        public string name { get; set; }
        /// <summary>True when this is a draft release, otherwise false.</summary>
        public bool draft { get; set; }
        /// <summary>True when this is marked as a prerelease, otherwise false.</summary>
        public bool prerelease { get; set; }
        /// <summary>Timestamp from when this release was created.</summary>
        /// <remarks>Uses sortable UTC format.</remarks>
        public string created_at { get; set; }
        /// <summary>Timestamp from when this release was published.</summary>
        /// <remarks>Uses sortable UTC format.</remarks>
        public string published_at { get; set; }
        /// <summary>List of URLs pointing towards this release's attached assets; including the executable.</summary>
        public GithubAssetHttpResponse[] assets { get; set; }
        /// <summary>URL of the source code tarball asset.</summary>
        public string tarball_url { get; set; }
        /// <summary>URL of the source code zipball asset.</summary>
        public string zipball_url { get; set; }
        /// <summary>The release body.</summary>
        public string body { get; set; }
    }
}
