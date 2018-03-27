using System;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Toastify.Core
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MouseAction
    {
        MWheelUp = MouseButton.Middle + 120,
        MWheelDown = MouseButton.Middle - 120,
        XButton1 = MouseButton.XButton1,
        XButton2 = MouseButton.XButton2,
    }
}