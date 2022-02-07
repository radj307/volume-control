using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;

namespace AudioAPI.WindowsAPI.Audio
{
    [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioSessionEnumerator
    {
        /// <summary>
        /// Get the number of audio sessions currently running.
        /// </summary>
        /// <param name="sessionCount">Output integer to hold the number of sessions.</param>
        /// <returns>int</returns>
        [PreserveSig]
        int GetCount(out int sessionCount);
        /// <summary>
        /// Get the specified audio session using its index.
        /// </summary>
        /// <param name="sessionCount">The target session number.</param>
        /// <param name="session">Output IAudioSessionControl2 instance to hold the target session.</param>
        /// <returns>int</returns>
        [PreserveSig]
        int GetSession(int sessionCount, out IAudioSessionControl2 session);

        /// <summary>
        /// Get a list of all current audio sessions.
        /// </summary>
        /// <returns>List<IAudioSessionControl2></returns>
        public List<IAudioSessionControl2> GetAllSessions()
        {
            GetCount(out int count);
            List<IAudioSessionControl2> list = new();

            for (int i = 0; i < count; ++i)
            {
                GetSession(i, out IAudioSessionControl2 session);
                list.Add(session);
            }

            return list;
        }
    }
}
