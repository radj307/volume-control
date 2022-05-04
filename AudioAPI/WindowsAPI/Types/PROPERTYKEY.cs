namespace AudioAPI.WindowsAPI.Types
{
    public struct PROPERTYKEY
    {
        /// <summary>
        /// A unique GUID for the property.
        /// </summary>
        public Guid formatId;
        /// <summary>
        /// A property identifier (PID). This parameter is not used as in SHCOLUMNID. It is recommended that you set this value to PID_FIRST_USABLE. Any value greater than or equal to 2 is acceptable.
        /// </summary>
        /// <remarks><b>Note:</b> Values of 0 and 1 are reserved and should not be used.</remarks>
        public int propertyId;
    }
}
