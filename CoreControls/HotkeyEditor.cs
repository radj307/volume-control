using Core;
using System.ComponentModel;

namespace CoreControls
{
    public partial class HotkeyEditor : UserControl
    {
        public HotkeyEditor()
        {
            _hkBind = new(this);

            InitializeComponent();
        }

        #region Members
        private readonly HotkeyBinding _hkBind;
        #endregion Members

        #region Properties
        public event HandledEventHandler HotkeyPressed
        {
            add => _hkBind.Pressed += value;
            remove => _hkBind.Pressed -= value;
        }
        public bool HotkeyEnabled
        {
            get => cbEnabled.Checked && _hkBind.Registered;
            set => cbEnabled.Checked = _hkBind.Registered = value;
        }
        public string HotkeyName
        {
            get => Label_Name.Text;
            set => Label_Name.Text = value;
        }
        public Keys HotkeyKey
        {
            get => _hkBind.Hotkey.KeyCode;
            set => cmbKey.SelectedItem = _hkBind.Hotkey.KeyCode = value;
        }
        public bool HotkeyModShift
        {
            get => cbShift.Checked && _hkBind.Hotkey.Shift;
            set => cbShift.Checked = _hkBind.Hotkey.Shift = value;
        }
        public bool HotkeyModCtrl
        {
            get => cbCtrl.Checked && _hkBind.Hotkey.Ctrl;
            set => cbCtrl.Checked = _hkBind.Hotkey.Ctrl = value;
        }
        public bool HotkeyModAlt
        {
            get => cbAlt.Checked && _hkBind.Hotkey.Alt;
            set => cbAlt.Checked = _hkBind.Hotkey.Alt = value;
        }
        public bool HotkeyModWin
        {
            get => cbWin.Checked && _hkBind.Hotkey.Win;
            set => cbWin.Checked = _hkBind.Hotkey.Win = value;
        }
        #endregion Properties

        #region EventHandlers
        private void cmbKey_SelectedItemChanged(object? sender, EventArgs e)
            => _hkBind.Hotkey.KeyCode = (Keys)cmbKey.SelectedItem;
        private void cbEnabled_CheckedChanged(object? sender, EventArgs e)
            => Label_Name.Enabled = cmbKey.Enabled = cbShift.Enabled = cbCtrl.Enabled = cbAlt.Enabled = cbWin.Enabled = _hkBind.Registered = cbEnabled.Checked;
        private void cbShift_CheckedChanged(object? sender, EventArgs e)
            => _hkBind.Hotkey.Shift = cbShift.Checked;
        private void cbCtrl_CheckedChanged(object? sender, EventArgs e)
            => _hkBind.Hotkey.Ctrl = cbCtrl.Checked;
        private void cbAlt_CheckedChanged(object? sender, EventArgs e)
            => _hkBind.Hotkey.Alt = cbAlt.Checked;
        private void cbWin_CheckedChanged(object? sender, EventArgs e)
            => _hkBind.Hotkey.Win = cbWin.Checked;
        #endregion EventHandlers
    }
}