using System.ComponentModel;

namespace VolumeControl
{
    [ToolboxItem(true), DesignTimeVisible(true)]
    public partial class HotkeyEditor : UserControl
    {
        public HotkeyEditor()
        {
            InitializeComponent();
        }

        private void key_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
