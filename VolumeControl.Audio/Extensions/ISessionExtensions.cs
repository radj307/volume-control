using VolumeControl.Audio.Interfaces;
using VolumeControl.Core;

namespace VolumeControl.Audio.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="ISession"/> objects.
    /// </summary>
    public static class ISessionExtensions
    {
        /// <summary>
        /// Extension method that gets a <see cref="Config.TargetInfo"/> struct with this session's data.
        /// </summary>
        public static Config.TargetInfo GetTargetInfo(this ISession session) => new() { ProcessIdentifier = session.ProcessIdentifier, SessionInstanceIdentifier = session.SessionInstanceIdentifier };
    }
}
