namespace VolumeControl.Core.Input.Exceptions
{
    public class HotkeyRegistrationException : Exception
    {
        public HotkeyRegistrationException(int hresult, string message) : base(message)
        {
            HResult = hresult;
        }
    }
}
