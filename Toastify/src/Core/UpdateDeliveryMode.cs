using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Toastify.Common;

namespace Toastify.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UpdateDeliveryMode
    {
        [ComboBoxItem("Notify only", "A toast will be displayed when a new version is available, but it won't be downloaded")]
        NotifyUpdate,

        [ComboBoxItem("Download automatically", "The new version will be downloaded automatically; the user is notified when the update is ready to be installed")]
        AutoDownload
    }
}