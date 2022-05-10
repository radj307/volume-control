using AudioAPI.Interfaces;

namespace AudioAPI.Objects.Virtual
{
    /// <summary>
    /// Implements <see cref="IProcess"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="Virtual"/> returns true.<br/>
    /// <see cref="ProcessName"/> returns <see cref="string.Empty"/>.<br/>
    /// <see cref="PID"/> returns -1.<br/>
    /// </remarks>
    public class VirtualAudioSession : IProcess
    {
        /// <inheritdoc/>
        /// <remarks>This object is always virtual.</remarks>
        public bool Virtual => true;
        /// <inheritdoc/>
        public string ProcessName => string.Empty;
        /// <inheritdoc/>
        public int PID => -1;
        /// <inheritdoc/>
        public bool Equals(IProcess? other) => Virtual.Equals(other?.Virtual) && PID.Equals(other?.PID);
    }
}
