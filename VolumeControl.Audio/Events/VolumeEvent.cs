using VolumeControl.Audio.Interfaces;

namespace VolumeControl.Audio.Events
{
    /// <summary>
    /// Event arguments for the <see cref="VolumeEventHandler"/> event handler type.
    /// </summary>
    public class VolumeEventArgs : EventArgs
    {
        /// <inheritdoc cref="VolumeEventArgs"/>
        /// <param name="target">The subject of the volume event.</param>
        /// <param name="volume">The volume level of the <paramref name="target"/>.</param>
        /// <param name="muted">The mute state of the <paramref name="target"/>.</param>
        public VolumeEventArgs(ITargetable target, int volume, bool muted)
        {
            Target = target;
            Volume = volume;
            Muted = muted;
        }
        /// <summary>The subject of the event.</summary>
        public ITargetable Target { get; }
        /// <summary>The volume level of this instances <see cref="Target"/>.</summary>
        public int Volume { get; }
        /// <summary>The mute state of this instances <see cref="Target"/>.</summary>
        public bool Muted { get; }
    }
    /// <summary>
    /// Handler delegate for volume change events.
    /// </summary>
    /// <param name="sender">Event trigger source.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void VolumeEventHandler(object? sender, VolumeEventArgs e);
}
