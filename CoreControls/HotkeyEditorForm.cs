using Core;
using Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreControls
{
    public partial class HotkeyEditorForm : Form
    {
        public HotkeyEditorForm(AudioProcessAPI api)
        {
            API = api;
            _hotkeys = new();
            InitializeComponent();
            bsHotkeyBindingList.DataSource = _hotkeys;
        }

        private readonly HotkeyBindingList _hotkeys;
        public AudioProcessAPI API;

        public void AddHotkey(Control owner, string hkstr, VolumeControlSubject subject, VolumeControlAction action)
        {
            _hotkeys.Add(new(owner, hkstr, subject, action));
        }

        public new void Hide()
        {
            Visible = false;
            WindowState = FormWindowState.Minimized;
        }
        public new void Show()
        {
            WindowState = FormWindowState.Normal;
            Visible = true;
        }

        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentCell is DataGridViewCheckBoxCell)
                dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void HotkeyEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void bCancel_Click(object sender, EventArgs e)
            => Hide();
    }
}
