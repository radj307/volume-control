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
        }

        #endregion Constructors

        #region Methods

        public void SetHotkeyIsEnabled(bool enable) => Checkbox_Enabled.Checked = enable;

        public void SetLabel(string label) => Label_HotkeyName.Text = label;


        #endregion Methods

        #region Properties

        public bool HotkeyIsEnabled
        {
            get => Checkbox_Enabled.Checked;
        }

        public string Label
        {
            get => Label_HotkeyName.Text;
        }

        public Hotkey Hotkey
        {
            get
            {
                return new Hotkey(
                    (Keys)Enum.Parse(typeof(Keys), Combobox_KeySelector.Text, true),
                    Checkbox_ModifierKey_Shift.Checked,
                    Checkbox_ModifierKey_Ctrl.Checked,
                    Checkbox_ModifierKey_Alt.Checked,
                    Checkbox_ModifierKey_Win.Checked
                );
            }
            set
            {
                Combobox_KeySelector.Text = Enum.GetName(typeof(Keys), value.KeyCode);
                Checkbox_ModifierKey_Shift.Checked = value.Shift;
                Checkbox_ModifierKey_Ctrl.Checked = value.Control;
                Checkbox_ModifierKey_Alt.Checked = value.Alt;
                Checkbox_ModifierKey_Win.Checked = value.Windows;
            }
        }

        #endregion Properties

        private EventHandler onModifierChanged;
        public event EventHandler ModifierChanged
        {
            add
            {
                onModifierChanged += value;
            }
            remove
            {
                onModifierChanged -= value;
            }
        }
        protected virtual void OnModifierChanged(EventArgs e)
        {
            onModifierChanged?.Invoke(this, e);
        }

        private void Checkbox_ModifierKey_Shift_CheckedChanged(object sender, EventArgs e)
        {
            OnModifierChanged(e);
        }

        private void Checkbox_ModifierKey_Ctrl_CheckedChanged(object sender, EventArgs e)
        {
            OnModifierChanged(e);
        }

        private void Checkbox_ModifierKey_Alt_CheckedChanged(object sender, EventArgs e)
        {
            OnModifierChanged(e);
        }

        private void Checkbox_ModifierKey_Win_CheckedChanged(object sender, EventArgs e)
        {
            OnModifierChanged(e);
        }
    }
}
