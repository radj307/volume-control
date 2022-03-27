namespace CoreControls
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
            this.cbEnabled = new System.Windows.Forms.CheckBox();
            this.cbShift = new System.Windows.Forms.CheckBox();
            this.cbCtrl = new System.Windows.Forms.CheckBox();
            this.cbAlt = new System.Windows.Forms.CheckBox();
            this.cbWin = new System.Windows.Forms.CheckBox();
            this.cmbKey = new System.Windows.Forms.ComboBox();
            this.keyListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Label_Name = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.keyListBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // cbEnabled
            // 
            this.cbEnabled.AutoSize = true;
            this.cbEnabled.Location = new System.Drawing.Point(3, 6);
            this.cbEnabled.Name = "cbEnabled";
            this.cbEnabled.Size = new System.Drawing.Size(15, 14);
            this.cbEnabled.TabIndex = 0;
            this.cbEnabled.UseVisualStyleBackColor = true;
            this.cbEnabled.CheckedChanged += new System.EventHandler(this.cbEnabled_CheckedChanged);
            // 
            // cbShift
            // 
            this.cbShift.AutoSize = true;
            this.cbShift.Location = new System.Drawing.Point(254, 3);
            this.cbShift.Name = "cbShift";
            this.cbShift.Size = new System.Drawing.Size(50, 19);
            this.cbShift.TabIndex = 1;
            this.cbShift.Text = "Shift";
            this.cbShift.UseVisualStyleBackColor = true;
            this.cbShift.CheckedChanged += new System.EventHandler(this.cbShift_CheckedChanged);
            // 
            // cbCtrl
            // 
            this.cbCtrl.AutoSize = true;
            this.cbCtrl.Location = new System.Drawing.Point(310, 3);
            this.cbCtrl.Name = "cbCtrl";
            this.cbCtrl.Size = new System.Drawing.Size(45, 19);
            this.cbCtrl.TabIndex = 2;
            this.cbCtrl.Text = "Ctrl";
            this.cbCtrl.UseVisualStyleBackColor = true;
            this.cbCtrl.CheckedChanged += new System.EventHandler(this.cbCtrl_CheckedChanged);
            // 
            // cbAlt
            // 
            this.cbAlt.AutoSize = true;
            this.cbAlt.Location = new System.Drawing.Point(361, 3);
            this.cbAlt.Name = "cbAlt";
            this.cbAlt.Size = new System.Drawing.Size(41, 19);
            this.cbAlt.TabIndex = 3;
            this.cbAlt.Text = "Alt";
            this.cbAlt.UseVisualStyleBackColor = true;
            this.cbAlt.CheckedChanged += new System.EventHandler(this.cbAlt_CheckedChanged);
            // 
            // cbWin
            // 
            this.cbWin.AutoSize = true;
            this.cbWin.Location = new System.Drawing.Point(408, 3);
            this.cbWin.Name = "cbWin";
            this.cbWin.Size = new System.Drawing.Size(47, 19);
            this.cbWin.TabIndex = 4;
            this.cbWin.Text = "Win";
            this.cbWin.UseVisualStyleBackColor = true;
            this.cbWin.CheckedChanged += new System.EventHandler(this.cbWin_CheckedChanged);
            // 
            // cmbKey
            // 
            this.cmbKey.DataSource = this.keyListBindingSource;
            this.cmbKey.FormattingEnabled = true;
            this.cmbKey.Location = new System.Drawing.Point(127, 1);
            this.cmbKey.Name = "cmbKey";
            this.cmbKey.Size = new System.Drawing.Size(121, 23);
            this.cmbKey.TabIndex = 5;
            this.cmbKey.SelectionChangeCommitted += new System.EventHandler(this.cmbKey_SelectedItemChanged);
            // 
            // keyListBindingSource
            // 
            this.keyListBindingSource.DataSource = typeof(HotkeyLib.KeyList);
            // 
            // Label_Name
            // 
            this.Label_Name.AutoSize = true;
            this.Label_Name.Location = new System.Drawing.Point(24, 5);
            this.Label_Name.Name = "Label_Name";
            this.Label_Name.Size = new System.Drawing.Size(90, 15);
            this.Label_Name.TabIndex = 6;
            this.Label_Name.Text = "[ hotkey name ]";
            // 
            // HotkeyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Label_Name);
            this.Controls.Add(this.cmbKey);
            this.Controls.Add(this.cbWin);
            this.Controls.Add(this.cbAlt);
            this.Controls.Add(this.cbCtrl);
            this.Controls.Add(this.cbShift);
            this.Controls.Add(this.cbEnabled);
            this.Name = "HotkeyEditor";
            this.Size = new System.Drawing.Size(455, 25);
            ((System.ComponentModel.ISupportInitialize)(this.keyListBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private CheckBox cbEnabled;
        private CheckBox cbShift;
        private CheckBox cbCtrl;
        private CheckBox cbAlt;
        private CheckBox cbWin;
        private ComboBox cmbKey;
        private Label Label_Name;
        private BindingSource keyListBindingSource;
    }
}