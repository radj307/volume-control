using Semver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VolumeControl.Core.Extensions;

namespace VolumeControl.Helpers.Update
{
    /// <summary>
    /// Helper object for managing and comparing release query response packets.
    /// </summary>
    public class Release : IEquatable<SemVersion>, IComparable<SemVersion>, IEnumerable<Release.Asset>, IEquatable<Release>, IComparable<Release>, IComparable
    {
        /// <summary>Helper object for managing release assets (files).</summary>
        public struct Asset
        {
            /// <summary>The name of the asset file.</summary>
            public string FileName { get; set; }
            /// <summary>The size of the asset file in bytes.</summary>
            public int Size { get; set; }
            /// <summary>The download URL of the asset file.</summary>
            public string DownloadURL { get; set; }
        }

        #region Constructor
        public Release(GithubReleaseHttpResponse packet)
        {
            Version = packet.tag_name.GetSemVer() ?? new SemVersion(0);
            PreRelease = packet.prerelease;
            URL = packet.html_url;
            Assets = new();
            foreach (var asset in packet.assets)
            {
                Assets.Add(new()
                {
                    FileName = asset.name,
                    Size = asset.size,
                    DownloadURL = asset.browser_download_url
                });
            }
        }
        #endregion Constructor

        #region Properties
        public SemVersion Version { get; }
        public bool PreRelease { get; }
        public string URL { get; }
        public List<Asset> Assets { get; }

        public Asset? this[string name] => Assets.FirstOrDefault(a => a.FileName.Equals(name, StringComparison.OrdinalIgnoreCase));
        #endregion Properties

        #region Methods
        /// <inheritdoc/>
        public int CompareTo(SemVersion? other) => ((IComparable<SemVersion>)Version).CompareTo(other);
        /// <inheritdoc/>
        public int CompareTo(object? obj) => ((IComparable)Version).CompareTo(obj);
        /// <inheritdoc/>
        public int CompareTo(Release? other) => Version.CompareTo(other?.Version);
        /// <inheritdoc/>
        public bool Equals(SemVersion? other) => ((IEquatable<SemVersion>)Version).Equals(other);
        /// <inheritdoc/>
        public bool Equals(Release? other) => other is not null && Version.Equals(other.Version);
        /// <inheritdoc/>
        public IEnumerator<Asset> GetEnumerator() => ((IEnumerable<Asset>)Assets).GetEnumerator();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Assets).GetEnumerator();
        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj != null && obj is Release release && Equals(release);
        /// <inheritdoc/>
        public override int GetHashCode() => Version.GetHashCode();
        /// <summary>
        /// Checks if this release version is newer than <paramref name="other"/>.
        /// </summary>
        /// <param name="other">Another release version.</param>
        /// <returns><see langword="true"/> when this release is newer than <paramref name="other"/> or <see langword="false"/> when this release is older than or equal to <paramref name="other"/>.</returns>
        public bool IsNewerThan(Release other) => Version.CompareByPrecedence(other.Version) > 0;
        /// <summary>
        /// Checks if this release version is newer than version <paramref name="other"/>.
        /// </summary>
        /// <param name="other">Any <see cref="SemVersion"/> type.</param>
        /// <returns><see langword="true"/> when this release is newer than <paramref name="other"/> or <see langword="false"/> when this release is older than or equal to <paramref name="other"/>.</returns>
        public bool IsNewerThan(SemVersion other) => Version.CompareByPrecedence(other) > 0;
        #region Operators
        public static bool operator ==(Release left, Release right) => left is null ? right is null : left.Equals(right);
        public static bool operator !=(Release left, Release right) => !(left == right);
        public static bool operator <(Release left, Release right) => left is null ? right is not null : left.CompareTo(right) < 0;
        public static bool operator <=(Release left, Release right) => left is null || left.CompareTo(right) <= 0;
        public static bool operator >(Release left, Release right) => left is not null && left.CompareTo(right) > 0;
        public static bool operator >=(Release left, Release right) => left is null ? right is null : left.CompareTo(right) >= 0;
        #endregion Operators
        #endregion Methods
    }
}
