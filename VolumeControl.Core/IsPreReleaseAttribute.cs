namespace VolumeControl.Core
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class IsPreReleaseAttribute : Attribute
    {
        public string IsPreRelease { get; }
        public IsPreReleaseAttribute(string isPreRelease) => IsPreRelease = isPreRelease;
    }
}
