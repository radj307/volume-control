using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ToastifyAPI.Native.Enums;

namespace ToastifyAPI.Native.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LowLevelMouseHookStruct
    {
        /// <summary>
        /// The x- and y-coordinates of the cursor, in per-monitor-aware screen coordinates.
        /// </summary>
        public Point pt;

        /// <summary> Mouse data. </summary>
        /// <remarks>
        /// If the message is <see cref="WindowsMessagesFlags.WM_MOUSEWHEEL"/>, the high-order word of this member
        /// is the wheel delta. The low-order word is reserved.
        /// A positive value indicates that the wheel was rotated forward, away from the user;
        /// a negative value indicates that the wheel was rotated backward, toward the user.
        /// One wheel click is defined as WHEEL_DELTA, which is 120.
        /// 
        /// If the message is <see cref="WindowsMessagesFlags.WM_XBUTTONDOWN"/>, <see cref="WindowsMessagesFlags.WM_XBUTTONUP"/>,
        /// <see cref="WindowsMessagesFlags.WM_XBUTTONDBLCLK"/>, <see cref="WindowsMessagesFlags.WM_NCXBUTTONDOWN"/>, 
        /// <see cref="WindowsMessagesFlags.WM_NCXBUTTONUP"/>, or <see cref="WindowsMessagesFlags.WM_NCXBUTTONDBLCLK"/>,
        /// the high-order word specifies which X button was pressed or released, and the low-order word is reserved.
        /// This value can be one or more of the following values. Otherwise, mouseData is not used.
        /// 
        /// <list type="bullet">
        /// <item>
        ///   <term> XBUTTON1 </term>
        ///   <description> 0x0001 </description>
        /// </item>
        /// <item>
        ///   <term> XBUTTON2 </term>
        ///   <description> 0x0002 </description>
        /// </item>
        /// </list>
        /// </remarks>
        public int mouseData;

        /// <summary>
        /// The event-injected flags.
        /// <para>
        /// An application can use the following values to test the flags.
        /// Testing LLMHF_INJECTED (bit 0 set) will tell you whether the event was injected. If it was, then testing LLMHF_LOWER_IL_INJECTED (bit 1 set)
        /// will tell you whether or not the event was injected from a process running at lower integrity level.
        /// </para>
        /// </summary>
        public int flags;

        /// <summary>
        /// The time stamp for this message.
        /// </summary>
        public int time;

        /// <summary>
        /// Additional information associated with the message.
        /// </summary>
        public UIntPtr dwExtraInfo;
    }
}