using System.ComponentModel;
using Common;

namespace UIComponents
{
    [ToolboxItem(true), DesignTimeVisible(true)]
    public partial class HotkeyEditor : UserControl
    {
        public HotkeyEditor()
        {
            InitializeComponent();
        }

        public Hotkey Hotkey
        {
            get
            {
                return new Hotkey((Keys)Enum.Parse(typeof(Keys), this.key.Text), this.mod_shift.Checked, this.mod_ctrl.Checked, this.mod_alt.Checked, this.mod_win.Checked);
            }
            set
            {
                key.Text = Enum.GetName(typeof(Keys), value.KeyCode);
                mod_shift.Checked = value.Shift;
                mod_ctrl.Checked = value.Control;
                mod_alt.Checked = value.Alt;
                mod_win.Checked = value.Windows;
            }
        }
    }
}
