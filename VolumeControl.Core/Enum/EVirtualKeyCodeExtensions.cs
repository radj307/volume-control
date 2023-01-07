using InputSimulatorEx.Native;

namespace VolumeControl.Core.Enum
{
    /// <summary>
    /// Extends the <see cref="EVirtualKeyCode"/> enumeration.
    /// </summary>
    public static class EVirtualKeyCodeExtensions
    {
        /// <summary>
        /// Get the equivalent <see cref="VirtualKeyCode"/> from a <see cref="EVirtualKeyCode"/>.
        /// </summary>
        /// <param name="vk">An <see cref="EVirtualKeyCode"/> enumeration value.</param>
        /// <returns>The equivalent <see cref="VirtualKeyCode"/> type.</returns>
        public static VirtualKeyCode GetVirtualKeyCodeEx(this EVirtualKeyCode vk) => (VirtualKeyCode)(short)vk;
    }
}
