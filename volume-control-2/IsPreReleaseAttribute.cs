﻿namespace volume_control_2
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    internal sealed class IsPreReleaseAttribute : Attribute
    {
        public string IsPreRelease { get; }
        public IsPreReleaseAttribute(string isPreRelease) => IsPreRelease = isPreRelease;
    }
}
