using Semver;
using VolumeControl.Core.Extensions;

namespace VolumeControl.Core
{
    /// <summary>
    /// A range of version numbers that abstracts away the need for direct interaction with the <see cref="SemVersion"/> type.<br/>If you want to directly interact with <see cref="SemVersion"/> objects, you will need the 'Semver' nuget package.
    /// </summary>
    public struct CompatibleVersions
    {
        /// <inheritdoc cref="CompatibleVersions"/>
        /// <remarks>This empty constructor sets all properties to null.</remarks>
        public CompatibleVersions()
        {
            Minimum = null;
            Maximum = null;
            Additional = null;
        }
        /// <inheritdoc cref="CompatibleVersions"/>
        /// <param name="min">The minimum allowable version, or null for unlimited.</param>
        /// <param name="max">The maximum allowable version, or null for unlimited.</param>
        /// <param name="additional">Optional list of specific versions to always consider compatible.</param>
        public CompatibleVersions(SemVersion? min, SemVersion? max, params SemVersion[]? additional)
        {
            Minimum = min;
            Maximum = max;
            Additional = additional;
        }
        /// <inheritdoc cref="CompatibleVersions"/>
        /// <remarks>This constructor accepts nullable strings rather than premade version objects.</remarks>
        /// <param name="min">The minimum allowable version, or null for unlimited.</param>
        /// <param name="max">The maximum allowable version, or null for unlimited.</param>
        /// <param name="additional">Optional list of specific versions to always consider compatible.</param>
        public CompatibleVersions(string? min, string? max, params string[]? additional)
        {
            Minimum = min?.GetSemVer();
            Maximum = max?.GetSemVer();
            if (additional == null)
            {
                Additional = null;
            }
            else
            {
                List<SemVersion> l = new();
                foreach (string? item in additional)
                {
                    if (item.GetSemVer() is SemVersion version)
                        l.Add(version);
                }

                Additional = l.ToArray();
            }
        }

        /// <summary>
        /// Specifies the minimum version of Volume Control that this addon is compatible with.<br/>
        /// <br/>
        /// <br/>
        /// When this is set to null, no minimum version limit is applied.<br/>
        /// Note that only versions from 5.0.0 and up will actually load addons, so that is the absolute minimum version.
        /// </summary>
        /// <remarks>Defaults to null.</remarks>
        public SemVersion? Minimum { get; set; }
        /// <summary>
        /// <br/>
        /// When this is set to null, no maximum version limit is applied.
        /// </summary>
        /// <remarks>Defaults to null.</remarks>
        public SemVersion? Maximum { get; set; }
        /// <summary>
        /// An option enumerable type 
        /// </summary>
        public IEnumerable<SemVersion>? Additional { get; set; }

        /// <summary>
        /// This method is responsible for comparing version numbers between addons.
        /// </summary>
        /// <param name="version">The version number to compare.</param>
        /// <returns>True if <paramref name="version"/> is within this range, otherwise false.</returns>
        public bool Contains(SemVersion version)
            => (Minimum == null || version >= Minimum) && (Maximum == null || version <= Maximum) || (Additional?.Contains(version) ?? false);
    }
}