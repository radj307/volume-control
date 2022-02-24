namespace UIComposites
{
    partial class HotkeyEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Checkbox_Enabled = new System.Windows.Forms.CheckBox();
            this.Label_HotkeyName = new System.Windows.Forms.Label();
            this.Combobox_KeySelector = new System.Windows.Forms.ComboBox();
            this.keyListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Checkbox_ModifierKey_Ctrl = new System.Windows.Forms.CheckBox();
            this.Checkbox_ModifierKey_Alt = new System.Windows.Forms.CheckBox();
            this.Checkbox_ModifierKey_Shift = new System.Windows.Forms.CheckBox();
            this.Checkbox_ModifierKey_Win = new System.Windows.Forms.CheckBox();
            this.cmb_Panel = new System.Windows.Forms.Panel();
            this.CheckboxImages = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.keyListBindingSource)).BeginInit();
            this.cmb_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Checkbox_Enabled
            // 
            this.Checkbox_Enabled.AutoSize = true;
            this.Checkbox_Enabled.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Checkbox_Enabled.Location = new System.Drawing.Point(4, 32);
            this.Checkbox_Enabled.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_Enabled.Name = "Checkbox_Enabled";
            this.Checkbox_Enabled.Size = new System.Drawing.Size(65, 19);
            this.Checkbox_Enabled.TabIndex = 0;
            this.Checkbox_Enabled.Text = "Enabled";
            this.Checkbox_Enabled.UseMnemonic = false;
            this.Checkbox_Enabled.UseVisualStyleBackColor = false;
            // 
            // Label_HotkeyName
            // 
            this.Label_HotkeyName.AutoSize = true;
            this.Label_HotkeyName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Label_HotkeyName.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Label_HotkeyName.Location = new System.Drawing.Point(4, 6);
            this.Label_HotkeyName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_HotkeyName.Name = "Label_HotkeyName";
            this.Label_HotkeyName.Size = new System.Drawing.Size(91, 16);
            this.Label_HotkeyName.TabIndex = 1;
            this.Label_HotkeyName.Text = "[ placeholder ]";
            // 
            // Combobox_KeySelector
            // 
            this.Combobox_KeySelector.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Combobox_KeySelector.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Combobox_KeySelector.Cursor = System.Windows.Forms.Cursors.Default;
            this.Combobox_KeySelector.DataSource = this.keyListBindingSource;
            this.Combobox_KeySelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Combobox_KeySelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Combobox_KeySelector.FormattingEnabled = true;
            this.Combobox_KeySelector.Location = new System.Drawing.Point(0, 0);
            this.Combobox_KeySelector.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Combobox_KeySelector.MaximumSize = new System.Drawing.Size(250, 0);
            this.Combobox_KeySelector.MinimumSize = new System.Drawing.Size(164, 0);
            this.Combobox_KeySelector.Name = "Combobox_KeySelector";
            this.Combobox_KeySelector.Size = new System.Drawing.Size(193, 23);
            this.Combobox_KeySelector.TabIndex = 1;
            // 
            // Checkbox_ModifierKey_Ctrl
            // 
            this.Checkbox_ModifierKey_Ctrl.AutoSize = true;
            this.Checkbox_ModifierKey_Ctrl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Checkbox_ModifierKey_Ctrl.Location = new System.Drawing.Point(199, 32);
            this.Checkbox_ModifierKey_Ctrl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_ModifierKey_Ctrl.Name = "Checkbox_ModifierKey_Ctrl";
            this.Checkbox_ModifierKey_Ctrl.Size = new System.Drawing.Size(42, 19);
            this.Checkbox_ModifierKey_Ctrl.TabIndex = 3;
            this.Checkbox_ModifierKey_Ctrl.Text = "Ctrl";
            this.Checkbox_ModifierKey_Ctrl.UseMnemonic = false;
            this.Checkbox_ModifierKey_Ctrl.UseVisualStyleBackColor = false;
            this.Checkbox_ModifierKey_Ctrl.CheckedChanged += new System.EventHandler(this.Checkbox_ModifierKey_CheckedChanged);
            // 
            // Checkbox_ModifierKey_Alt
            // 
            this.Checkbox_ModifierKey_Alt.AutoSize = true;
            this.Checkbox_ModifierKey_Alt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Checkbox_ModifierKey_Alt.Location = new System.Drawing.Point(249, 32);
            this.Checkbox_ModifierKey_Alt.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_ModifierKey_Alt.Name = "Checkbox_ModifierKey_Alt";
            this.Checkbox_ModifierKey_Alt.Size = new System.Drawing.Size(38, 19);
            this.Checkbox_ModifierKey_Alt.TabIndex = 4;
            this.Checkbox_ModifierKey_Alt.Text = "Alt";
            this.Checkbox_ModifierKey_Alt.UseMnemonic = false;
            this.Checkbox_ModifierKey_Alt.UseVisualStyleBackColor = false;
            this.Checkbox_ModifierKey_Alt.CheckedChanged += new System.EventHandler(this.Checkbox_ModifierKey_CheckedChanged);
            // 
            // Checkbox_ModifierKey_Shift
            // 
            this.Checkbox_ModifierKey_Shift.AutoSize = true;
            this.Checkbox_ModifierKey_Shift.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Checkbox_ModifierKey_Shift.Location = new System.Drawing.Point(144, 32);
            this.Checkbox_ModifierKey_Shift.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_ModifierKey_Shift.Name = "Checkbox_ModifierKey_Shift";
            this.Checkbox_ModifierKey_Shift.Size = new System.Drawing.Size(47, 19);
            this.Checkbox_ModifierKey_Shift.TabIndex = 2;
            this.Checkbox_ModifierKey_Shift.Text = "Shift";
            this.Checkbox_ModifierKey_Shift.UseMnemonic = false;
            this.Checkbox_ModifierKey_Shift.UseVisualStyleBackColor = false;
            this.Checkbox_ModifierKey_Shift.CheckedChanged += new System.EventHandler(this.Checkbox_ModifierKey_CheckedChanged);
            // 
            // Checkbox_ModifierKey_Win
            // 
            this.Checkbox_ModifierKey_Win.AutoSize = true;
            this.Checkbox_ModifierKey_Win.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Checkbox_ModifierKey_Win.Location = new System.Drawing.Point(295, 32);
            this.Checkbox_ModifierKey_Win.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_ModifierKey_Win.Name = "Checkbox_ModifierKey_Win";
            this.Checkbox_ModifierKey_Win.Size = new System.Drawing.Size(44, 19);
            this.Checkbox_ModifierKey_Win.TabIndex = 5;
            this.Checkbox_ModifierKey_Win.Text = "Win";
            this.Checkbox_ModifierKey_Win.UseMnemonic = false;
            this.Checkbox_ModifierKey_Win.UseVisualStyleBackColor = false;
            this.Checkbox_ModifierKey_Win.CheckedChanged += new System.EventHandler(this.Checkbox_ModifierKey_CheckedChanged);
            // 
            // cmb_Panel
            // 
            this.cmb_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cmb_Panel.Controls.Add(this.Combobox_KeySelector);
            this.cmb_Panel.ForeColor = System.Drawing.Color.Transparent;
            this.cmb_Panel.Location = new System.Drawing.Point(144, 3);
            this.cmb_Panel.Name = "cmb_Panel";
            this.cmb_Panel.Size = new System.Drawing.Size(195, 23);
            this.cmb_Panel.TabIndex = 8;
            // 
            // CheckboxImages
            // 
            this.CheckboxImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.CheckboxImages.ImageSize = new System.Drawing.Size(16, 16);
            this.CheckboxImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // HotkeyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.cmb_Panel);
            this.Controls.Add(this.Checkbox_ModifierKey_Win);
            this.Controls.Add(this.Checkbox_ModifierKey_Shift);
            this.Controls.Add(this.Checkbox_ModifierKey_Alt);
            this.Controls.Add(this.Checkbox_ModifierKey_Ctrl);
            this.Controls.Add(this.Label_HotkeyName);
            this.Controls.Add(this.Checkbox_Enabled);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "HotkeyEditor";
            this.Size = new System.Drawing.Size(342, 56);
            ((System.ComponentModel.ISupportInitialize)(this.keyListBindingSource)).EndInit();
            this.cmb_Panel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox Combobox_KeySelector;
        private System.Windows.Forms.CheckBox Checkbox_ModifierKey_Ctrl;
        private System.Windows.Forms.CheckBox Checkbox_ModifierKey_Alt;
        private System.Windows.Forms.CheckBox Checkbox_ModifierKey_Shift;
        private System.Windows.Forms.CheckBox Checkbox_ModifierKey_Win;
        public CheckBox Checkbox_Enabled;
        private Label Label_HotkeyName;
        private Panel cmb_Panel;
        private BindingSource keyListBindingSource;
        private ImageList CheckboxImages;
    }
}
