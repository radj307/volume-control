using Audio.Interfaces;

namespace Audio.Helpers
{
    /// <summary>
    /// Extension methods for any class that implements the <see cref="IAudioControl"/> interface.
    /// </summary>
    public static class IAudioControlExtensions
    {
        /// <summary>
        /// Increases the value of the <see cref="IAudioControl.NativeVolume"/> property by the given <paramref name="amount"/>.
        /// </summary>
        /// <param name="vc"><see cref="IAudioControl"/> instance.</param>
        /// <param name="amount">The amount to change the volume by.</param>
        public static void IncreaseNativeVolume(this IAudioControl vc, float amount)
            => vc.NativeVolume += amount;
        /// <summary>
        /// Decreases the value of the <see cref="IAudioControl.NativeVolume"/> property by the given <paramref name="amount"/>.
        /// </summary>
        /// <param name="vc"><see cref="IAudioControl"/> instance.</param>
        /// <param name="amount">The amount to change the volume by.</param>
        public static void DecreaseNativeVolume(this IAudioControl vc, float amount)
            => vc.NativeVolume -= amount;
        /// <summary>
        /// Increases the value of the <see cref="IAudioControl.Volume"/> property by the given <paramref name="amount"/>.
        /// </summary>
        /// <param name="vc"><see cref="IAudioControl"/> instance.</param>
        /// <param name="amount">The amount to change the volume by.</param>
        public static void IncreaseVolume(this IAudioControl vc, int amount)
            => vc.Volume += amount;
        /// <summary>
        /// Decreases the value of the <see cref="IAudioControl.Volume"/> property by the given <paramref name="amount"/>.
        /// </summary>
        /// <param name="vc"><see cref="IAudioControl"/> instance.</param>
        /// <param name="amount">The amount to change the volume by.</param>
        public static void DecreaseVolume(this IAudioControl vc, int amount)
            => vc.Volume -= amount;
        /// <summary>
        /// Sets the <see cref="IAudioControl.Mute"/> property to the given <paramref name="muteState"/>.
        /// </summary>
        /// <param name="vc"><see cref="IAudioControl"/> instance.</param>
        /// <param name="muteState"><see langword="true"/> to mute; <see langword="false"/> to unmute.</param>
        public static void SetMute(this IAudioControl vc, bool muteState)
            => vc.Mute = muteState;
        /// <summary>
        /// Toggles the <see cref="IAudioControl.Mute"/> property.
        /// </summary>
        /// <param name="vc"><see cref="IAudioControl"/> instance.</param>
        public static void ToggleMute(this IAudioControl vc)
            => vc.Mute = !vc.Mute;
    }
}