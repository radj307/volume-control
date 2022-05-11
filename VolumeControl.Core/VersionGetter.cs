using System.Reflection;

namespace VolumeControl.Core
{
    public class VersionGetter
    {
        public VersionGetter() => Version = $"v{Assembly.GetExecutingAssembly().GetCustomAttribute<Core.Attributes.ExtendedVersion>()?.Version}";
        public string Version { get; set; }
    }
}
