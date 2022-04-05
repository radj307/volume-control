namespace AudioAPI
{
    /// <summary>
    /// Contains constant definitions for various windows SDK return codes related to the Windows Audio API.
    /// </summary>
    public static class ReturnCodes
    {
        public const UInt32 S_OK = 0;
        public const UInt32 AUDCLNT_E_DEVICE_INVALIDATED = 2290679812;
        public const UInt32 AUDCLNT_E_SERVICE_NOT_RUNNING = 2290679824;
    }
}
