using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Toastify.Common;

namespace Toastify.Helpers
{
    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static bool CheckCancellation(this BackgroundWorker backgroundWorker, DoWorkEventArgs doWorkEventArgs)
        {
            if (backgroundWorker.CancellationPending)
                doWorkEventArgs.Cancel = true;
            return doWorkEventArgs.Cancel;
        }

        public static string GetReadableName<TEnum>(this TEnum value)
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            var attributes = value.GetType().GetField(value.ToString(CultureInfo.InvariantCulture)).GetCustomAttributes(typeof(EnumReadableNameAttribute), false);
            var attribute = attributes.Any()
                ? (EnumReadableNameAttribute)attributes.First()
                : new EnumReadableNameAttribute(value.ToString(CultureInfo.InvariantCulture));

            return attribute.Name;
        }
    }
}