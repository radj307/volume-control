using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace HotkeyLib
{
    [TypeConverter(typeof(HotkeyConverter))]
    public class Hotkey : IMessageFilter
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

        public Hotkey() : this(Keys.None, false, false, false, false)
        {
            // No work done here!
        }

        public Hotkey(string keystr, HandledEventHandler? pressed_action = null)
        {
            Pressed = null!;
            windowControl = null!;

            Shift = keystr.Contains("Shift+", StringComparison.OrdinalIgnoreCase);
            Control = keystr.Contains("Control+", StringComparison.OrdinalIgnoreCase);
            Alt = keystr.Contains("Alt+", StringComparison.OrdinalIgnoreCase);
            Windows = keystr.Contains("Windows+", StringComparison.OrdinalIgnoreCase);

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
            this.KeyCode = keyCode;
            this.Shift = shift;
            this.Control = control;
            this.Alt = alt;
            this.Windows = windows;

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
                keyCode = (Keys)Enum.Parse(typeof(Keys), keystr.Substring(lastpos + 1));

            Application.AddMessageFilter(this);
        }

        ~Hotkey()
        {
            // Unregister the hotkey if necessary
            if (this.Registered)
            { this.Unregister(); }
        }

        public void Reset(string keystr)
        {
            if (keystr.Length > 0 && keystr != this.ToString())
            {
                Shift = keystr.Contains("Shift+", StringComparison.OrdinalIgnoreCase);
                Control = keystr.Contains("Control+", StringComparison.OrdinalIgnoreCase);
                Alt = keystr.Contains("Alt+", StringComparison.OrdinalIgnoreCase);
                Windows = keystr.Contains("Windows+", StringComparison.OrdinalIgnoreCase);

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
        public Hotkey Clone()
        {
            // Clone the whole object
            return new Hotkey(this.keyCode, this.shift, this.control, this.alt, this.windows);
        }

        public bool GetCanRegister(Control windowControl)
        {
            // Handle any exceptions: they mean "no, you can't register" :)
            try
            {
                // Attempt to register
                if (!this.Register(windowControl))
                { return false; }

                // Unregister and say we managed it
                this.Unregister();
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
            if (this.registered)
            { throw new NotSupportedException("You cannot register a hotkey that is already registered"); }

            // We can't register an empty hotkey
            if (this.Empty)
            { throw new NotSupportedException("You cannot register an empty hotkey"); }

            // Get an ID for the hotkey and increase current ID
            this.id = currentID;
            currentID = currentID + 1 % maximumID;

            // Translate modifier keys into unmanaged version
            uint modifiers = (this.Alt ? MOD_ALT : 0) | (this.Control ? MOD_CONTROL : 0) |
                            (this.Shift ? MOD_SHIFT : 0) | (this.Windows ? MOD_WIN : 0);

            // Register the hotkey
            if (RegisterHotKey(windowControl.Handle, this.id, modifiers, keyCode) == 0)
            {
                // Is the error that the hotkey is registered?
                if (Marshal.GetLastWin32Error() == ERROR_HOTKEY_ALREADY_REGISTERED)
                { return false; }
                else
                { throw new Win32Exception(); }
            }

            // Save the control reference and register state
            this.registered = true;
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
            if (!this.registered)
            { throw new NotSupportedException("You cannot unregister a hotkey that is not registered"); }

            // It's possible that the control itself has died: in that case, no need to unregister!
            if (!this.windowControl.IsDisposed)
            {
                // Clean up after ourselves
                if (UnregisterHotKey(this.windowControl.Handle, this.id) == 0)
                { throw new Win32Exception(); }
            }

            // Clear the control reference and register state
            this.registered = false;
            this.windowControl = null!;
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
            if (!this.registered)
            { return; }

            // Save control reference
            Control windowControl = this.windowControl;

            // Unregister and then reregister again
            this.Unregister();
            this.Register(windowControl);
        }

        public bool PreFilterMessage(ref Message message)
        {
            // Only process WM_HOTKEY messages
            if (message.Msg != WM_HOTKEY)
            { return false; }

            // Check that the ID is our key and we are registerd
            if (this.registered && (message.WParam.ToInt32() == this.id))
            {
                // Fire the event and pass on the event if our handlers didn't handle it
                return this.OnPressed();
            }
            else
            { return false; }
        }

        private bool OnPressed()
        {
            // Fire the event if we can
            HandledEventArgs handledEventArgs = new HandledEventArgs(false);
            if (this.Pressed != null)
            { this.Pressed(this, handledEventArgs); }

            // Return whether we handled the event or not
            return handledEventArgs.Handled;
        }

        public override string ToString()
        {
            // We can be empty
            if (this.Empty)
            { return "(none)"; }

            // Build key name
            string keyName = Enum.GetName(typeof(Keys), keyCode) ?? "";
            switch (this.keyCode)
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
                keyName = keyName.Substring(1);
                break;
            default:
                // Leave everything alone
                break;
            }

            // Build modifiers
            string modifiers = "";
            if (this.shift)
            { modifiers += "Shift+"; }
            if (this.control)
            { modifiers += "Control+"; }
            if (this.alt)
            { modifiers += "Alt+"; }
            if (this.windows)
            { modifiers += "Windows+"; }

            // Return result
            return modifiers + keyName;
        }

        public bool Empty
        {
            get { return this.keyCode == Keys.None; }
        }

        public bool Registered
        {
            get { return this.registered; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Keys KeyCode
        {
            get { return this.keyCode; }
            set
            {
                // Save and reregister
                this.keyCode = value;
                this.Reregister();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Shift
        {
            get { return this.shift; }
            set
            {
                // Save and reregister
                this.shift = value;
                this.Reregister();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Control
        {
            get { return this.control; }
            set
            {
                // Save and reregister
                this.control = value;
                this.Reregister();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Alt
        {
            get { return this.alt; }
            set
            {
                // Save and reregister
                this.alt = value;
                this.Reregister();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Windows
        {
            get { return this.windows; }
            set
            {
                // Save and reregister
                this.windows = value;
                this.Reregister();
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
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            return (value as Hotkey) ?? base.ConvertFrom(context, culture, value);
        }
    }
}
