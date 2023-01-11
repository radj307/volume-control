using VolumeControl.Audio.Interfaces;
using VolumeControl.Core.Helpers;

namespace VolumeControl.Audio.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="ISession"/> objects.
    /// </summary>
    public static class ISessionExtensions
    {
        /// <summary>
        /// Extension method that gets a <see cref="TargetInfo"/> struct with this session's data.
        /// </summary>
        public static TargetInfo GetTargetInfo(this ISession session) => new() { ProcessIdentifier = session.ProcessIdentifier, SessionInstanceIdentifier = session.SessionInstanceIdentifier };
    }
}
