using System.Reflection;

namespace VolumeControl.Core
{
    public class VersionGetter
    {
        public VersionGetter() => Version = string.Empty;
        public string Version { get; set; }
    }
}
