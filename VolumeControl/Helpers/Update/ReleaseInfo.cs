using Semver;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers.Update
{
    /// <summary>
    /// Wraps a <see cref="GithubReleaseHttpResponse"/> packet with comparison and equatability methods.
    /// </summary>
    public struct ReleaseInfo : IComparable<ReleaseInfo>, IEquatable<ReleaseInfo>, IComparable<SemVersion>, IEquatable<SemVersion>
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ReleaseInfo"/> struct.
        /// </summary>
        /// <param name="responsePacket">The <see cref="GithubReleaseHttpResponse"/> packet received from the API.</param>
        public ReleaseInfo(GithubReleaseHttpResponse responsePacket) => packet = responsePacket;
        #endregion Constructor

        #region Fields
        internal readonly GithubReleaseHttpResponse packet;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets a new <see cref="ReleaseInfo"/> struct representing the latest release by sending an HTTP GET request to the Github Releases API.
        /// </summary>
        /// <remarks><b>This requires network access!</b></remarks>
        public static ReleaseInfo Latest
        {
            get
            {
                try
                {
                    using HttpClient client = new();

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                    client.DefaultRequestHeaders.Add("User-Agent", "curl/7.64.1");

                    System.Threading.Tasks.Task<GithubReleaseHttpResponse>? getRequestTask = client.GetFromJsonAsync<GithubReleaseHttpResponse>(UpdateChecker._apiUriLatest);
                    getRequestTask.Wait();

                    return new ReleaseInfo(getRequestTask.Result);
                }
                catch (Exception ex)
                {
                    FLog.Log.Error(ex);
                }
                return new ReleaseInfo();
            }
        }
        /// <summary>
        /// The version number of the release represented by this struct.
        /// </summary>
        public SemVersion Version => _version ??= (packet.tag_name.GetSemVer() ?? new(-1));
        private SemVersion? _version = null;
        /// <summary>
        /// The HTML URL of the release.
        /// </summary>
        public string URL => packet.html_url;
        #endregion Properties

        #region Methods
        public bool TryGetAsset(string nameStartsWith, StringComparison stringComparison, out GithubAssetHttpResponse asset)
        {
            foreach (var packetAsset in packet.assets)
            {
                if (packetAsset.name.StartsWith(nameStartsWith))
                {
                    asset = packetAsset;
                    return true;
                }
            }
            asset = default!;
            return false;
        }
        public bool TryGetAsset(string nameStartsWith, out GithubAssetHttpResponse asset)
            => TryGetAsset(nameStartsWith, StringComparison.OrdinalIgnoreCase, out asset);

        /// <inheritdoc/>
        public int CompareTo(ReleaseInfo other) => this.Version.CompareSortOrderTo(other.Version);
        /// <inheritdoc/>
        public bool Equals(ReleaseInfo other) => this.Version.Equals(other.Version);

        public static bool operator <(ReleaseInfo left, ReleaseInfo right) => left.CompareTo(right) < 0;
        public static bool operator <=(ReleaseInfo left, ReleaseInfo right) => left.CompareTo(right) <= 0;
        public static bool operator >(ReleaseInfo left, ReleaseInfo right) => left.CompareTo(right) > 0;
        public static bool operator >=(ReleaseInfo left, ReleaseInfo right) => left.CompareTo(right) >= 0;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ReleaseInfo other && this.Equals(other);
        /// <inheritdoc/>
        public override int GetHashCode() => this.Version.GetHashCode();
        /// <inheritdoc/>
        public int CompareTo(SemVersion? other) => this.Version.CompareSortOrderTo(other);
        /// <inheritdoc/>
        public bool Equals(SemVersion? other) => this.Version.Equals(other);

        public static bool operator ==(ReleaseInfo left, ReleaseInfo right) => left.Equals(right);
        public static bool operator !=(ReleaseInfo left, ReleaseInfo right) => !(left == right);
        #endregion Methods
    }
}
