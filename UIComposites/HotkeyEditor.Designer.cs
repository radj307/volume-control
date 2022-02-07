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
            this.Checkbox_Enabled.Location = new System.Drawing.Point(7, 35);
            this.Checkbox_Enabled.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_Enabled.Name = "Checkbox_Enabled";
            this.Checkbox_Enabled.Size = new System.Drawing.Size(68, 19);
            this.Checkbox_Enabled.TabIndex = 0;
            this.Checkbox_Enabled.Text = "Enabled";
            this.Checkbox_Enabled.UseVisualStyleBackColor = true;
            this.Checkbox_Enabled.CheckedChanged += HotkeyChanged;
            // 
            // Label_HotkeyName
            // 
            this.Label_HotkeyName.AutoSize = true;
            this.Label_HotkeyName.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Label_HotkeyName.Location = new System.Drawing.Point(4, 6);
            this.Label_HotkeyName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_HotkeyName.Name = "Label_HotkeyName";
            this.Label_HotkeyName.Size = new System.Drawing.Size(90, 15);
            this.Label_HotkeyName.TabIndex = 1;
            this.Label_HotkeyName.Text = "[ hotkey name ]";
            this.Label_HotkeyName.TextChanged += HotkeyChanged;
            // 
            // Combobox_KeySelector
            // 
            this.Combobox_KeySelector.FormattingEnabled = true;
            this.Combobox_KeySelector.Location = new System.Drawing.Point(115, 3);
            this.Combobox_KeySelector.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Combobox_KeySelector.Name = "Combobox_KeySelector";
            this.Combobox_KeySelector.Size = new System.Drawing.Size(220, 23);
            this.Combobox_KeySelector.TabIndex = 2;
            this.Combobox_KeySelector.TextChanged += HotkeyChanged;
            // 
            // Checkbox_ModifierKey_Ctrl
            // 
            this.Checkbox_ModifierKey_Ctrl.AutoSize = true;
            this.Checkbox_ModifierKey_Ctrl.Location = new System.Drawing.Point(177, 35);
            this.Checkbox_ModifierKey_Ctrl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_ModifierKey_Ctrl.Name = "Checkbox_ModifierKey_Ctrl";
            this.Checkbox_ModifierKey_Ctrl.Size = new System.Drawing.Size(45, 19);
            this.Checkbox_ModifierKey_Ctrl.TabIndex = 3;
            this.Checkbox_ModifierKey_Ctrl.Text = "Ctrl";
            this.Checkbox_ModifierKey_Ctrl.UseVisualStyleBackColor = true;
            this.Checkbox_ModifierKey_Ctrl.CheckedChanged += HotkeyChanged;
            // 
            // Checkbox_ModifierKey_Alt
            // 
            this.Checkbox_ModifierKey_Alt.AutoSize = true;
            this.Checkbox_ModifierKey_Alt.Location = new System.Drawing.Point(232, 35);
            this.Checkbox_ModifierKey_Alt.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_ModifierKey_Alt.Name = "Checkbox_ModifierKey_Alt";
            this.Checkbox_ModifierKey_Alt.Size = new System.Drawing.Size(41, 19);
            this.Checkbox_ModifierKey_Alt.TabIndex = 4;
            this.Checkbox_ModifierKey_Alt.Text = "Alt";
            this.Checkbox_ModifierKey_Alt.UseVisualStyleBackColor = true;
            this.Checkbox_ModifierKey_Alt.CheckedChanged += HotkeyChanged;
            // 
            // Checkbox_ModifierKey_Shift
            // 
            this.Checkbox_ModifierKey_Shift.AutoSize = true;
            this.Checkbox_ModifierKey_Shift.Location = new System.Drawing.Point(115, 35);
            this.Checkbox_ModifierKey_Shift.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_ModifierKey_Shift.Name = "Checkbox_ModifierKey_Shift";
            this.Checkbox_ModifierKey_Shift.Size = new System.Drawing.Size(50, 19);
            this.Checkbox_ModifierKey_Shift.TabIndex = 5;
            this.Checkbox_ModifierKey_Shift.Text = "Shift";
            this.Checkbox_ModifierKey_Shift.UseVisualStyleBackColor = true;
            this.Checkbox_ModifierKey_Shift.CheckedChanged += HotkeyChanged;
            // 
            // Checkbox_ModifierKey_Win
            // 
            this.Checkbox_ModifierKey_Win.AutoSize = true;
            this.Checkbox_ModifierKey_Win.Location = new System.Drawing.Point(284, 35);
            this.Checkbox_ModifierKey_Win.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Checkbox_ModifierKey_Win.Name = "Checkbox_ModifierKey_Win";
            this.Checkbox_ModifierKey_Win.Size = new System.Drawing.Size(47, 19);
            this.Checkbox_ModifierKey_Win.TabIndex = 6;
            this.Checkbox_ModifierKey_Win.Text = "Win";
            this.Checkbox_ModifierKey_Win.UseVisualStyleBackColor = true;
            this.Checkbox_ModifierKey_Win.CheckedChanged += HotkeyChanged;
            // 
            // HotkeyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.Size = new System.Drawing.Size(341, 57);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox Checkbox_Enabled;
        private System.Windows.Forms.Label Label_HotkeyName;
        private System.Windows.Forms.ComboBox Combobox_KeySelector;
        private System.Windows.Forms.CheckBox Checkbox_ModifierKey_Ctrl;
        private System.Windows.Forms.CheckBox Checkbox_ModifierKey_Alt;
        private System.Windows.Forms.CheckBox Checkbox_ModifierKey_Shift;
        private System.Windows.Forms.CheckBox Checkbox_ModifierKey_Win;
    }
}
