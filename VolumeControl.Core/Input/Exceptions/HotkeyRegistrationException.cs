namespace VolumeControl.Core.Input.Exceptions
{
    /// <summary>
    /// Represents an error that occurs while registering or unregistering a hotkey.
    /// </summary>
    public class HotkeyRegistrationException : Exception
    {
        internal HotkeyRegistrationException(int hresult, string message) : base(message)
        {
            HResult = hresult;
        }
    }
}
