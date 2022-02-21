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
            this.Checkbox_Enabled = new System.Windows.Forms.CheckBox();
            this.Label_HotkeyName = new System.Windows.Forms.Label();
            this.Combobox_KeySelector = new System.Windows.Forms.ComboBox();
            this.Checkbox_ModifierKey_Ctrl = new System.Windows.Forms.CheckBox();
            this.Checkbox_ModifierKey_Alt = new System.Windows.Forms.CheckBox();
            this.Checkbox_ModifierKey_Shift = new System.Windows.Forms.CheckBox();
            this.Checkbox_ModifierKey_Win = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Checkbox_Enabled
            // 
            this.Checkbox_Enabled.AutoSize = true;
            this.Checkbox_Enabled.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Checkbox_Enabled.Location = new System.Drawing.Point(4, 34);
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
            this.Label_HotkeyName.Location = new System.Drawing.Point(4, 8);
            this.Label_HotkeyName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_HotkeyName.Name = "Label_HotkeyName";
            this.Label_HotkeyName.Size = new System.Drawing.Size(91, 16);
            this.Label_HotkeyName.TabIndex = 1;
            this.Label_HotkeyName.Text = "[ placeholder ]";
            // 
            // Combobox_KeySelector
            // 
            this.Combobox_KeySelector.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.Combobox_KeySelector.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Combobox_KeySelector.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Combobox_KeySelector.FormattingEnabled = true;
            this.Combobox_KeySelector.Location = new System.Drawing.Point(132, 6);
            this.Combobox_KeySelector.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Combobox_KeySelector.MaximumSize = new System.Drawing.Size(250, 0);
            this.Combobox_KeySelector.MinimumSize = new System.Drawing.Size(164, 0);
            this.Combobox_KeySelector.Name = "Combobox_KeySelector";
            this.Combobox_KeySelector.Size = new System.Drawing.Size(207, 23);
            this.Combobox_KeySelector.TabIndex = 7;
            // 
            // Checkbox_ModifierKey_Ctrl
            // 
            this.Checkbox_ModifierKey_Ctrl.AutoSize = true;
            this.Checkbox_ModifierKey_Ctrl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Checkbox_ModifierKey_Ctrl.Location = new System.Drawing.Point(190, 35);
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
            this.Checkbox_ModifierKey_Alt.Location = new System.Drawing.Point(243, 35);
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
            this.Checkbox_ModifierKey_Shift.Location = new System.Drawing.Point(132, 35);
            this.Checkbox_ModifierKey_Shift.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_ModifierKey_Shift.Name = "Checkbox_ModifierKey_Shift";
            this.Checkbox_ModifierKey_Shift.Size = new System.Drawing.Size(47, 19);
            this.Checkbox_ModifierKey_Shift.TabIndex = 5;
            this.Checkbox_ModifierKey_Shift.Text = "Shift";
            this.Checkbox_ModifierKey_Shift.UseMnemonic = false;
            this.Checkbox_ModifierKey_Shift.UseVisualStyleBackColor = false;
            this.Checkbox_ModifierKey_Shift.CheckedChanged += new System.EventHandler(this.Checkbox_ModifierKey_CheckedChanged);
            // 
            // Checkbox_ModifierKey_Win
            // 
            this.Checkbox_ModifierKey_Win.AutoSize = true;
            this.Checkbox_ModifierKey_Win.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Checkbox_ModifierKey_Win.Location = new System.Drawing.Point(292, 35);
            this.Checkbox_ModifierKey_Win.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_ModifierKey_Win.Name = "Checkbox_ModifierKey_Win";
            this.Checkbox_ModifierKey_Win.Size = new System.Drawing.Size(44, 19);
            this.Checkbox_ModifierKey_Win.TabIndex = 6;
            this.Checkbox_ModifierKey_Win.Text = "Win";
            this.Checkbox_ModifierKey_Win.UseMnemonic = false;
            this.Checkbox_ModifierKey_Win.UseVisualStyleBackColor = false;
            this.Checkbox_ModifierKey_Win.CheckedChanged += new System.EventHandler(this.Checkbox_ModifierKey_CheckedChanged);
            // 
            // HotkeyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.Checkbox_ModifierKey_Win);
            this.Controls.Add(this.Checkbox_ModifierKey_Shift);
            this.Controls.Add(this.Checkbox_ModifierKey_Alt);
            this.Controls.Add(this.Checkbox_ModifierKey_Ctrl);
            this.Controls.Add(this.Combobox_KeySelector);
            this.Controls.Add(this.Label_HotkeyName);
            this.Controls.Add(this.Checkbox_Enabled);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "HotkeyEditor";
            this.Size = new System.Drawing.Size(341, 58);
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
    }
}
