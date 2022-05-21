using Semver;
using System.Runtime.CompilerServices;

namespace VolumeControl.Core
{
    /// <summary>Abstract base class that all other addon attributes inherit from.<br/><b>Unless you're implementing a new addon type, this isn't the attribute you're looking for.</b></summary>
    /// <remarks>An addon type for the Volume Control program.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public abstract class BaseAddonAttribute : Attribute, IBaseAddonAttribute
    {
        /// <inheritdoc cref="BaseAddonAttribute"/>
        /// <param name="className">The string to set the <see cref="AddonName"/> property to. It is recommended to use the <see cref="CallerMemberNameAttribute"/> attribute to retrieve this.</param>
        protected BaseAddonAttribute(string addonName) => AddonName = addonName;
        /// <summary>This is used when indexing this addon, as well as in the log.</summary>
        /// <remarks>It is recommended to let this set itself using the class name provided by <see cref="CallerMemberNameAttribute"/>, however you can change this to something consistent if you want.</remarks>
        public string AddonName { get; }
        /// <summary>
        /// Specifies the minimum version of Volume Control that this addon is compatible with.<br/>
        /// <br/>
        /// <br/>
        /// When this is set to null, no minimum version limit is applied.<br/>
        /// Note that only versions from 5.0.0 and up will actually load addons, so that is the absolute minimum version.
        /// </summary>
        /// <remarks>Defaults to null.</remarks>
        public SemVersion? MinimumVersion { get; set; }
        /// <summary>
        /// <br/>
        /// When this is set to null, no maximum version limit is applied.
        /// </summary>
        /// <remarks>Defaults to null.</remarks>
        public SemVersion? MaximumVersion { get; set; }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        public int CompareTo(SemVersion? other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            int compMin = other.CompareByPrecedence(MinimumVersion);
            int compMax = other.CompareByPrecedence(MaximumVersion);
            if (compMin >= 0 && compMax <= 0)
                return 0;
            else if (compMin < 0)
                return -1;
            else
                return 1;
        }
    }
}