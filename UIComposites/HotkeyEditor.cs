using System.Windows.Forms;
using HotkeyLib;

namespace UIComposites
{
    public partial class HotkeyEditor : UserControl
    {
        #region Constructors

        public HotkeyEditor()
        {
            InitializeComponent();
            Label_HotkeyName = new Label();
            IsEnabled = false;
        }

        #endregion Constructors

        #region Helpers

        private Hotkey GetHotkey()
        {
            return new(Key, Shift, Ctrl, Alt, Win);
        }

        private void SetFromHotkey(Hotkey hk)
        {
            Key = hk.KeyCode;
            Shift = hk.Shift;
            Ctrl = hk.Control;
            Alt = hk.Alt;
            Win = hk.Windows;
        }

        #endregion Helpers

        #region Events

        public event EventHandler HotkeyChanged;

        #endregion Events

        #region Properties

        public string Label
        {
            get { return Label_HotkeyName.Text; }
            set { Label_HotkeyName.Text = value; }
        }

        public Keys Key
        {
            get
            {
                try
                {
                    return (Keys)Enum.Parse(typeof(Keys), Combobox_KeySelector.Text, true);
                }
                catch (Exception)
                {
                    return Keys.None;
                }
            }
            set
            {
                try
                {
                    Combobox_KeySelector.Text = Enum.GetName(value);
                }
                catch (Exception) { }
            }
        }

        public bool IsEnabled
        {
            get { return Checkbox_Enabled.Checked; }
            set
            {
                Checkbox_Enabled.Checked = value;
            }
        }

        public bool Shift
        {
            get { return Checkbox_ModifierKey_Shift.Checked; }
            set
            {
                Checkbox_ModifierKey_Shift.Checked = value;
            }
        }
        public bool Ctrl
        {
            get { return Checkbox_ModifierKey_Ctrl.Checked; }
            set
            {
                Checkbox_ModifierKey_Ctrl.Checked = value;
            }
        }
        public bool Alt
        {
            get { return Checkbox_ModifierKey_Alt.Checked; }
            set
            {
                Checkbox_ModifierKey_Alt.Checked = value;
            }
        }
        public bool Win
        {
            get { return Checkbox_ModifierKey_Win.Checked; }
            set
            {
                Checkbox_ModifierKey_Win.Checked = value;
            }
        }

        public Hotkey Hotkey
        {
            get { return GetHotkey(); }
            set
            {
                SetFromHotkey(value);
            }
        }

        #endregion Properties
    }
}
