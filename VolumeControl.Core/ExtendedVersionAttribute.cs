namespace VolumeControl.Core
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class ExtendedVersionAttribute : Attribute
    {
        public string ExtendedVersion { get; }
        public ExtendedVersionAttribute(string extver) => ExtendedVersion = extver;
    }
}
