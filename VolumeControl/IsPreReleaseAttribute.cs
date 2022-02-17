namespace VolumeControl
{
    [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    internal sealed class IsPreReleaseAttribute : System.Attribute
    {
        public string IsPreRelease { get; }
        public IsPreReleaseAttribute(string isPreRelease) => IsPreRelease = isPreRelease;
    }
}
