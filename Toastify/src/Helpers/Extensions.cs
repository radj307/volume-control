using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using log4net;
using Toastify.Common;
using Toastify.Core;
using Toastify.Model;
using ToastifyAPI.Helpers;

namespace Toastify.Helpers
{
    public static class Extensions
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Extensions));

        #region Static Members

        public static IList<T> Clone<T>(this IEnumerable<T> enumerableToClone) where T : ICloneable
        {
            return enumerableToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static IEnumerable<T> DistinctAndSortByToastifyAction<T>(this IEnumerable<T> hotkeys) where T : Hotkey
        {
            if (hotkeys == null)
                return null;

            bool Equals(T h1, T h2) => h1?.Action?.Equals(h2?.Action) ?? h2?.Action == null;
            int GetHashCode(T h) => h?.Action?.GetHashCode() ?? 0;

            return (from h in hotkeys.Distinct(Equals, GetHashCode)
                    let toastifyAction = h.Action as ToastifyAction
                    where h.Action != null && (toastifyAction == null || toastifyAction.ToastifyActionEnum != ToastifyActionEnum.None)
                    orderby h.HumanReadableAction
                    select h).ToList();
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
            object[] attributes = value.GetType().GetField(value.ToString(CultureInfo.InvariantCulture)).GetCustomAttributes(typeof(EnumReadableNameAttribute), false);
            EnumReadableNameAttribute attribute = attributes.Any()
                ? (EnumReadableNameAttribute)attributes.First()
                : new EnumReadableNameAttribute(value.ToString(CultureInfo.InvariantCulture));

            return attribute.Name;
        }

        public static IntPtr GetHandle(this Window window)
        {
            var wndHelper = new WindowInteropHelper(window);
            return wndHelper.Handle;
        }

        public static bool IsModifierKey(this Key key)
        {
            return key.GetModifierKey() != ModifierKeys.None;
        }

        public static ModifierKeys GetModifierKey(this Key key)
        {
            var modifiers = ModifierKeys.None;

            if (key == Key.LeftCtrl || key == Key.RightCtrl)
                modifiers = ModifierKeys.Control;
            else if (key == Key.LeftAlt || key == Key.RightAlt)
                modifiers = ModifierKeys.Alt;
            else if (key == Key.LeftShift || key == Key.RightShift)
                modifiers = ModifierKeys.Shift;
            else if (key == Key.LWin || key == Key.RWin)
                modifiers = ModifierKeys.Windows;
            return modifiers;
        }

        public static string GetReadableName(this ModifierKeys modifierKeys)
        {
            var modifiers = new List<string>();

            if ((modifierKeys & ModifierKeys.Control) != ModifierKeys.None)
                modifiers.Add("Ctrl");
            if ((modifierKeys & ModifierKeys.Shift) != ModifierKeys.None)
                modifiers.Add("Shift");
            if ((modifierKeys & ModifierKeys.Alt) != ModifierKeys.None)
                modifiers.Add("Alt");
            if ((modifierKeys & ModifierKeys.Windows) != ModifierKeys.None)
                modifiers.Add("Win");

            return string.Join("+", modifiers);
        }

        public static T Clamp<T>(this T value, T min, T max) where T : IComparable
        {
            T ret = value;
            if (value.CompareTo(min) < 0)
                ret = min;
            else if (value.CompareTo(max) > 0)
                ret = max;
            return ret;
        }

        public static T Clamp<T>(this T value, Range<T> range) where T : IComparable
        {
            return value.Clamp(range.Min, range.Max);
        }

        public static T Clamp<T>(this T value, Range<T>? range) where T : IComparable
        {
            return range.HasValue ? value.Clamp(range.Value) : value;
        }

        public static string ToPlainString(this SecureString secureString)
        {
            if (secureString == null)
                return null;
            if (secureString.Length == 0)
                return string.Empty;

            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(valuePtr);
            }
            catch (Exception ex)
            {
                logger.Error("Unknown error", ex);
                return null;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        #endregion
    }
}