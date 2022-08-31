using System.ComponentModel;

namespace VolumeControl.Audio.Interfaces
{
    /// <summary>
    /// Represents an audio session.
    /// </summary>
    public interface ISession : IProcess, IAudioControl, INotifyPropertyChanged
    {
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
}
