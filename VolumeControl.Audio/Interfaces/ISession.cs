using System.ComponentModel;
using VolumeControl.Core;

namespace VolumeControl.Audio.Interfaces
{
    /// <summary>
    /// Represents an audio session.
    /// </summary>
    public interface ISession : IProcess, ITargetable, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the volume of this process using the <see cref="int"/> type in the range <b>( 0 - 100 )</b>
        /// </summary>
        /// <remarks>Values are clamped before actually being applied, so out-of-bounds values are allowed.</remarks>
        /// <exception cref="InvalidOperationException">Cannot perform operations on a disposed-of object!</exception>
        int Volume { get; set; }
        /// <summary>
        /// Gets or sets the volume of this process using the native <see cref="float"/> type, in the range <b>( 0.0 <i>(0%)</i> - 1.0 <i>(100%)</i> )</b>
        /// </summary>
        /// <remarks>Values are clamped before actually being applied, so out-of-bounds values are allowed.</remarks>
        /// <exception cref="InvalidOperationException">Cannot perform operations on a disposed-of object!</exception>
        float NativeVolume { get; set; }
        /// <summary>
        /// Gets or sets whether the session is muted or not.
        /// </summary>
        bool Muted { get; set; }
        /// <summary>
        /// Gets the session's name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Each audio session is identified by an identifier string. This session identifier string is not unique across all instances. If there are two instances of the application playing, both instances will have the same session identifier. The identifier retrieved by GetSessionIdentifier is different from the session instance identifier, which is unique across all sessions. To get the session instance identifier, see <see cref="SessionInstanceIdentifier"/>
        /// </summary>
        string SessionIdentifier { get; }
        /// <summary>
        /// Each audio session instance is identified by a unique string. This string represents a particular instance of the audio session and, unlike the session identifier, is unique across all instances. If there are two instances of the application playing, they will have different session instance identifiers. The identifier retrieved by GetSessionInstanceIdentifier is different from the session identifier, which is shared by all session instances. To get the session identifier, see <see cref="SessionIdentifier"/>.
        /// </summary>
        string SessionInstanceIdentifier { get; }
    }
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
