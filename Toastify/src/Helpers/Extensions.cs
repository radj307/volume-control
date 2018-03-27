using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Toastify.Common;
using ToastifyAPI;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;

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

        public static IntPtr GetHandle(this Window window)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(window);
            return wndHelper.Handle;
        }

        public static bool IsModifierKey(this Key key)
        {
            return key.GetModifierKey() != ModifierKeys.None;
        }

        public static ModifierKeys GetModifierKey(this Key key)
        {
            ModifierKeys modifiers = ModifierKeys.None;

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
    }
}