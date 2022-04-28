using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Audio
{
    [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioSessionControl
    {
        /// <summary>
        /// The RegisterAudioSessionNotification method registers the client to receive notifications of session events, including changes in the stream state.
        /// </summary>
        /// <param name="NewNotifications">Pointer to a client-implemented IAudioSessionEvents interface. If the method succeeds, it calls the AddRef method on the client's IAudioSessionEvents interface.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// <list type="table">
        /// <item><term>E_POINTER</term><description>Parameter NewNotifications is NULL.</description></item>
        /// <item><term>AUDCLNT_E_DEVICE_INVALIDATED</term><description>The audio endpoint device has been unplugged, or the audio hardware or associated hardware resources have been reconfigured, disabled, removed, or otherwise made unavailable for use.</description></item>
        /// <item><term>AUDCLNT_E_SERVICE_NOT_RUNNING</term><description>The Windows audio service is not running.</description></item>
        /// </list>
        /// </returns>
        [PreserveSig]
        int RegisterAudioSessionNotifications(IAudioSessionEvents NewNotifications);
        /// <summary>
        /// The UnregisterAudioSessionNotification method deletes a previous registration by the client to receive notifications.
        /// </summary>
        /// <param name="NewNotifications">Pointer to a client-implemented IAudioSessionEvents interface. The client passed this same interface pointer to the session manager in a previous call to the IAudioSessionControl::RegisterAudioSessionNotification method. If the UnregisterAudioSessionNotification method succeeds, it calls the Release method on the client's IAudioSessionEvents interface.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.<list type="table">
        /// <item><term>E_POINTER</term><description>Parameter NewNotifications is NULL.</description></item>
        /// <item><term>E_NOTFOUND</term><description>The specified interface was not previously registered by the client or has already been removed.</description></item>
        /// </list></returns>
        [PreserveSig]
        int UnregisterAudioSessionNotifications(IAudioSessionEvents NewNotifications);
    }
}
