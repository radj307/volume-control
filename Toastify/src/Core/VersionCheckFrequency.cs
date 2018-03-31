using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Toastify.Common;

namespace Toastify.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VersionCheckFrequency
    {
        [ComboBoxItem("Every hour")]
        EveryHour = 1,

        [ComboBoxItem("Every 12 hours")]
        Every12Hours = 12,

        [ComboBoxItem("Every day")]
        EveryDay = 24,

        [ComboBoxItem("Every week")]
        EveryWeek = 168,

        [ComboBoxItem("Every month")]
        EveryMonth = 720
    }

    public static class VersionCheckFrequencyExtensions
    {
        public static TimeSpan ToTimeSpan(this VersionCheckFrequency vcf)
        {
            switch (vcf)
            {
                case VersionCheckFrequency.EveryHour:
                    return new TimeSpan(0, 1, 0, 0);

                case VersionCheckFrequency.Every12Hours:
                    return new TimeSpan(0, 12, 0, 0);

                case VersionCheckFrequency.EveryDay:
                    return new TimeSpan(1, 0, 0, 0);

                case VersionCheckFrequency.EveryWeek:
                    return new TimeSpan(7, 0, 0, 0);

                case VersionCheckFrequency.EveryMonth:
                    return new TimeSpan(30, 0, 0, 0);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}