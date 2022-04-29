namespace VolumeControl.Core.Controls.Forms
{
    partial class SettingsMenuForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.vbCancel = new VirtualButton.VButton(this.components);
            this.tabSwitcher1 = new VolumeControl.Core.Controls.TabSwitcher();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // vbCancel
            // 
            this.vbCancel.Click += new VirtualButton.VirtualClickEventHandler(this.Minimize);
            // 
            // tabSwitcher1
            // 
            this.tabSwitcher1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.tabSwitcher1.ForeColor = System.Drawing.Color.White;
            this.tabSwitcher1.Location = new System.Drawing.Point(0, 105);
            this.tabSwitcher1.Name = "tabSwitcher1";
            this.tabSwitcher1.PagePadding = new System.Windows.Forms.Padding(0);
            this.tabSwitcher1.SelectedIndex = 0;
            this.tabSwitcher1.Size = new System.Drawing.Size(335, 209);
            this.tabSwitcher1.TabIndex = 0;
            this.tabSwitcher1.TabMargin = new(1);
            this.tabSwitcher1.TabStyle = System.Windows.Forms.FlatStyle.Standard;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Generate Tab";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SettingsMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.CancelButton = this.vbCancel;
            this.ClientSize = new System.Drawing.Size(335, 314);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabSwitcher1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsMenuForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SettingsMenuForm";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.HandleFormHelpButton);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HandleFormClosing);
            this.Resize += new System.EventHandler(this.HandleFormResize);
            this.ResumeLayout(false);

        }

        #endregion

        private VirtualButton.VButton vbCancel;
        private TabSwitcher tabSwitcher1;
        private Button button1;
    }
}