using Audio.Interfaces;

namespace Audio.Helpers
{
    /// <summary>
    /// Extension methods for any class that implements the <see cref="IVolumeControl"/> interface.
    /// </summary>
    public static class VolumeControlExtensions
    {
        /// <summary>
        /// Increases the value of the <see cref="IVolumeControl.NativeVolume"/> property by the given <paramref name="amount"/>.
        /// </summary>
        /// <param name="vc"><see cref="IVolumeControl"/> instance.</param>
        /// <param name="amount">The amount to change the volume by.</param>
        public static void IncreaseNativeVolume(this IVolumeControl vc, float amount)
            => vc.NativeVolume += amount;
        /// <summary>
        /// Decreases the value of the <see cref="IVolumeControl.NativeVolume"/> property by the given <paramref name="amount"/>.
        /// </summary>
        /// <param name="vc"><see cref="IVolumeControl"/> instance.</param>
        /// <param name="amount">The amount to change the volume by.</param>
        public static void DecreaseNativeVolume(this IVolumeControl vc, float amount)
            => vc.NativeVolume -= amount;
        /// <summary>
        /// Increases the value of the <see cref="IVolumeControl.Volume"/> property by the given <paramref name="amount"/>.
        /// </summary>
        /// <param name="vc"><see cref="IVolumeControl"/> instance.</param>
        /// <param name="amount">The amount to change the volume by.</param>
        public static void IncreaseVolume(this IVolumeControl vc, int amount)
            => vc.Volume += amount;
        /// <summary>
        /// Decreases the value of the <see cref="IVolumeControl.Volume"/> property by the given <paramref name="amount"/>.
        /// </summary>
        /// <param name="vc"><see cref="IVolumeControl"/> instance.</param>
        /// <param name="amount">The amount to change the volume by.</param>
        public static void DecreaseVolume(this IVolumeControl vc, int amount)
            => vc.Volume -= amount;
        /// <summary>
        /// Sets the <see cref="IVolumeControl.Mute"/> property to the given <paramref name="muteState"/>.
        /// </summary>
        /// <param name="vc"><see cref="IVolumeControl"/> instance.</param>
        /// <param name="muteState"><see langword="true"/> to mute; <see langword="false"/> to unmute.</param>
        public static void SetMute(this IVolumeControl vc, bool muteState)
            => vc.Mute = muteState;
        /// <summary>
        /// Toggles the <see cref="IVolumeControl.Mute"/> property.
        /// </summary>
        /// <param name="vc"><see cref="IVolumeControl"/> instance.</param>
        public static void ToggleMute(this IVolumeControl vc)
            => vc.Mute = !vc.Mute;
    }
}