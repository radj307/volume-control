using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace HotkeyLib
{
    public interface IHotkey
    {
        Keys KeyCode { get; set; }
        bool Shift { get; set; }
        bool Ctrl { get; set; }
        bool Alt { get; set; }
        bool Win { get; set; }
    }

    [TypeConverter(typeof(HotkeyConverter))]
    public class Hotkey : IMessageFilter, IHotkey
    {
        #region Interop

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, Keys vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);

        private const uint WM_HOTKEY = 0x312;

        private const uint MOD_ALT = 0x1;
        private const uint MOD_CONTROL = 0x2;
        private const uint MOD_SHIFT = 0x4;
        private const uint MOD_WIN = 0x8;

        private const uint ERROR_HOTKEY_ALREADY_REGISTERED = 1409;

        #endregion

        private static int currentID;
        private const int maximumID = 0xBFFF;

        private Keys keyCode;
        private bool shift;
        private bool control;
        private bool alt;
        private bool windows;

        [XmlIgnore]
        private int id;
        [XmlIgnore]
        private bool registered;
        [XmlIgnore]
        private Control windowControl;

        public event HandledEventHandler Pressed;

        public Hotkey() : this(Keys.None, false, false, false, false) { }

        public Hotkey(string keystr, HandledEventHandler? pressed_action = null)
        {
            Pressed = null!;
            windowControl = null!;

            Shift = keystr.Contains("Shift+", StringComparison.OrdinalIgnoreCase);
            Ctrl = keystr.Contains("Control+", StringComparison.OrdinalIgnoreCase);
            Alt = keystr.Contains("Alt+", StringComparison.OrdinalIgnoreCase);
            Win = keystr.Contains("Windows+", StringComparison.OrdinalIgnoreCase);

            int lastpos = keystr.LastIndexOf('+');
            try
            {
                if (lastpos == -1)
                    keyCode = (Keys)Enum.Parse(typeof(Keys), keystr, true);
                else
                    keyCode = (Keys)Enum.Parse(typeof(Keys), keystr.AsSpan(lastpos + 1), true);
            }
            catch (Exception)
            {
                keyCode = Keys.None;
            }

            if (pressed_action != null)
                Pressed = pressed_action;

            Application.AddMessageFilter(this);
        }

        public Hotkey(Keys keyCode, bool shift, bool control, bool alt, bool windows)
        {
            Pressed = null!;
            windowControl = null!;

            // Assign properties
            KeyCode = keyCode;
            Shift = shift;
            Ctrl = control;
            Alt = alt;
            Win = windows;

            // Register us as a message filter
            Application.AddMessageFilter(this);
        }

        public Hotkey(string keystr)
        {
            Pressed = null!;
            windowControl = null!;

            shift = keystr.Contains("Shift+", StringComparison.OrdinalIgnoreCase);
            control = keystr.Contains("Control+", StringComparison.OrdinalIgnoreCase);
            alt = keystr.Contains("Alt+", StringComparison.OrdinalIgnoreCase);
            windows = keystr.Contains("Windows+", StringComparison.OrdinalIgnoreCase);
            int lastpos = keystr.LastIndexOf('+');
            if (lastpos == -1)
                keyCode = (Keys)Enum.Parse(typeof(Keys), keystr);
            else
                keyCode = (Keys)Enum.Parse(typeof(Keys), keystr.AsSpan(lastpos + 1));

            Application.AddMessageFilter(this);
        }

        ~Hotkey()
        {
            // Unregister the hotkey if necessary
            if (Registered)
            { Unregister(); }
        }

        public void Reset(string keystr)
        {
            if (keystr.Length > 0 && keystr != ToString())
            {
                Shift = keystr.Contains("Shift+", StringComparison.OrdinalIgnoreCase);
                Ctrl = keystr.Contains("Control+", StringComparison.OrdinalIgnoreCase);
                Alt = keystr.Contains("Alt+", StringComparison.OrdinalIgnoreCase);
                Win = keystr.Contains("Windows+", StringComparison.OrdinalIgnoreCase);

                int lastpos = keystr.LastIndexOf('+');
                try
                {
                    if (lastpos == -1)
                        keyCode = (Keys)Enum.Parse(typeof(Keys), keystr, true);
                    else
                        keyCode = (Keys)Enum.Parse(typeof(Keys), keystr.AsSpan(lastpos + 1), true);
                }
                catch (Exception)
                {
                    keyCode = Keys.None;
                }

                Reregister();
            }
        }
        public void Reset(Hotkey o)
        {
            Shift = o.Shift;
            Ctrl = o.Ctrl;
            Alt = o.Alt;
            Win = o.Win;
            KeyCode = o.KeyCode;

            Reregister();
        }
        public Hotkey Clone() => new(keyCode, shift, control, alt, windows);

        public bool GetCanRegister(Control windowControl)
        {
            // Handle any exceptions: they mean "no, you can't register" :)
            try
            {
                // Attempt to register
                if (!Register(windowControl))
                { return false; }

                // Unregister and say we managed it
                Unregister();
                return true;
            }
            catch (Win32Exception)
            { return false; }
            catch (NotSupportedException)
            { return false; }
        }

        public bool Register(Control windowControl)
        {
            // Check that we have not registered
            if (registered)
                throw new NotSupportedException("You cannot register a hotkey that is already registered");

            // We can't register an empty hotkey
            if (Empty)
            {
                return false;
                //throw new NotSupportedException("You cannot register an empty hotkey");
            }

            // Get an ID for the hotkey and increase current ID
            id = currentID;
            currentID++;

            // Translate modifier keys into unmanaged version
            uint modifiers = (Alt ? MOD_ALT : 0) | (Ctrl ? MOD_CONTROL : 0) |
                            (Shift ? MOD_SHIFT : 0) | (Win ? MOD_WIN : 0);

            // Register the hotkey
            if (RegisterHotKey(windowControl.Handle, id, modifiers, keyCode) == 0)
            {
                // Is the error that the hotkey is registered?
                if (Marshal.GetLastWin32Error() == ERROR_HOTKEY_ALREADY_REGISTERED)
                { return false; }
                else
                { throw new Win32Exception(); }
            }

            // Save the control reference and register state
            registered = true;
            this.windowControl = windowControl;

            // We successfully registered
            return true;
        }

        public bool TryRegister(Control windowControl)
        {
            if (registered)
                return false;
            return Register(windowControl);
        }

        public void Unregister()
        {
            // Check that we have registered
            if (!registered)
                throw new NotSupportedException("You cannot unregister a hotkey that is not registered");

            // It's possible that the control itself has died: in that case, no need to unregister!
            if (!windowControl.IsDisposed)
            {
                // Clean up after ourselves
                if (UnregisterHotKey(windowControl.Handle, id) == 0)
                    throw new Win32Exception();
            }

            // Clear the control reference and register state
            registered = false;
            windowControl = null!;
        }

        public bool TryUnregister()
        {
            if (!registered)
                return false;
            Unregister();
            return true;
        }

        private void Reregister()
        {
            // Only do something if the key is already registered
            if (!registered)
            { return; }

            // Save control reference
            Control windowControl = this.windowControl;

            // Unregister and then reregister again
            Unregister();
            Register(windowControl);
        }

        public bool PreFilterMessage(ref Message message)
        {
            // Only process WM_HOTKEY messages
            if (message.Msg != WM_HOTKEY)
            { return false; }

            // Check that the ID is our key and we are registerd
            if (registered && (message.WParam.ToInt32() == id))
            {
                // Fire the event and pass on the event if our handlers didn't handle it
                return OnPressed();
            }
            else
            { return false; }
        }

        private bool OnPressed()
        {
            // Fire the event if we can
            HandledEventArgs handledEventArgs = new(false);
            Pressed?.Invoke(this, handledEventArgs);

            // Return whether we handled the event or not
            return handledEventArgs.Handled;
        }

        public override string ToString()
        {
            // We can be empty
            if (Empty)
            { return "(none)"; }

            // Build key name
            string keyName = Enum.GetName(typeof(Keys), keyCode) ?? "";
            switch (keyCode)
            {
            case Keys.D0:
            case Keys.D1:
            case Keys.D2:
            case Keys.D3:
            case Keys.D4:
            case Keys.D5:
            case Keys.D6:
            case Keys.D7:
            case Keys.D8:
            case Keys.D9:
                // Strip the first character
                keyName = keyName[1..];
                break;
            default:
                // Leave everything alone
                break;
            }

            // Build modifiers
            string modifiers = "";
            if (shift)
            { modifiers += "Shift+"; }
            if (control)
            { modifiers += "Control+"; }
            if (alt)
            { modifiers += "Alt+"; }
            if (windows)
            { modifiers += "Windows+"; }

            // Return result
            return modifiers + keyName;
        }

        public bool Empty => keyCode == Keys.None;

        public bool Registered => registered;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Keys KeyCode
        {
            get => keyCode;
            set
            {
                // Save and reregister
                keyCode = value;
                Reregister();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Shift
        {
            get => shift;
            set
            {
                // Save and reregister
                shift = value;
                Reregister();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Ctrl
        {
            get => control;
            set
            {
                // Save and reregister
                control = value;
                Reregister();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Alt
        {
            get => alt;
            set
            {
                // Save and reregister
                alt = value;
                Reregister();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Win
        {
            get => windows;
            set
            {
                // Save and reregister
                windows = value;
                Reregister();
            }
        }
    }
    public class HotkeyConverter : TypeConverter
    {
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value != null && destinationType == typeof(string))
                return value.ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) => (value as Hotkey) ?? base.ConvertFrom(context, culture, value);
    }
}
