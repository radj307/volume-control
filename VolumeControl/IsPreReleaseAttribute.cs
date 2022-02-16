namespace VolumeControl
{
    [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    sealed class IsPreReleaseAttribute : System.Attribute
    {
        public string IsPreRelease { get; }
        public IsPreReleaseAttribute(string isPreRelease)
        {
            this.IsPreRelease = isPreRelease;
        }
    }
}
