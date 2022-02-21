using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIComposites
{
    public partial class TargetSelector : UserControl
    {
        public string LabelText
        {
            get => label_Target.Text;
            set => label_Target.Text = value;
        }
        /// <summary>
        /// Fires the SelectionChangeCommitted event when the text in the textbox portion of the combo box changes.
        /// This is in contrast to the SelectedValue property, which occurs when an item is selected in the dropdown list.
        /// </summary>
        public AudioAPI.AudioSession SelectedItem
        {
            get => (AudioAPI.AudioSession)cmb_Target.SelectedItem;
            set => cmb_Target.SelectedItem = value;
        }
        public int SelectedIndex
        {
            get => cmb_Target.SelectedIndex;
            set => cmb_Target.SelectedIndex = value;
        }
        public new string Text
        {
            get => cmb_Target.Text;
            set => cmb_Target.Text = value;
        }
        public object DataSource
        {
            get => cmb_Target.DataSource;
            set => cmb_Target.DataSource = value;
        }
        public int ListSize => cmb_Target.Items.Count;
        public BorderStyle ComboBoxBorder
        {
            get => cmb_Panel.BorderStyle;
            set => cmb_Panel.BorderStyle = value;
        }

        public TargetSelector()
        {
            InitializeComponent();
        }

        public int IndexOf(string name)
        {
            return cmb_Target.FindString(name);
        }

        public new event EventHandler TextChanged
        {
            add => cmb_Target.TextChanged += value;
            remove => cmb_Target.TextChanged -= value;
        }
        public event EventHandler ReloadButtonPressed
        {
            add => b_Reload.Click += value;
            remove => b_Reload.Click -= value;
        }
    }
}
