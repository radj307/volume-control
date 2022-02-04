using System.ComponentModel;

namespace UIComponents
{
    partial class HotkeyEditor : IComponent
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
            this.hotkey = new System.Windows.Forms.ComboBox();
            this.keysConverterBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.keysConverterBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mod_shift = new System.Windows.Forms.CheckBox();
            this.mod_ctrl = new System.Windows.Forms.CheckBox();
            this.mod_alt = new System.Windows.Forms.CheckBox();
            this.mod_win = new System.Windows.Forms.CheckBox();
            this.EnableHotkey = new System.Windows.Forms.CheckBox();
            this.hotkey_label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.keysConverterBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.keysConverterBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // hotkey
            // 
            this.hotkey.DataSource = this.keysConverterBindingSource1;
            this.hotkey.Dock = System.Windows.Forms.DockStyle.Left;
            this.hotkey.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hotkey.FormattingEnabled = true;
            this.hotkey.Location = new System.Drawing.Point(137, 0);
            this.hotkey.Name = "hotkey";
            this.hotkey.Size = new System.Drawing.Size(168, 24);
            this.hotkey.TabIndex = 1;
            // 
            // keysConverterBindingSource1
            // 
            this.keysConverterBindingSource1.DataSource = typeof(System.Windows.Forms.KeysConverter);
            // 
            // keysConverterBindingSource
            // 
            this.keysConverterBindingSource.DataSource = typeof(System.Windows.Forms.KeysConverter);
            // 
            // mod_shift
            // 
            this.mod_shift.AutoSize = true;
            this.mod_shift.Dock = System.Windows.Forms.DockStyle.Left;
            this.mod_shift.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mod_shift.Location = new System.Drawing.Point(305, 0);
            this.mod_shift.Name = "mod_shift";
            this.mod_shift.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.mod_shift.Size = new System.Drawing.Size(55, 24);
            this.mod_shift.TabIndex = 2;
            this.mod_shift.Text = "Shift";
            this.mod_shift.UseVisualStyleBackColor = true;
            // 
            // mod_ctrl
            // 
            this.mod_ctrl.AutoSize = true;
            this.mod_ctrl.Dock = System.Windows.Forms.DockStyle.Left;
            this.mod_ctrl.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mod_ctrl.Location = new System.Drawing.Point(360, 0);
            this.mod_ctrl.Name = "mod_ctrl";
            this.mod_ctrl.Size = new System.Drawing.Size(46, 24);
            this.mod_ctrl.TabIndex = 3;
            this.mod_ctrl.Text = "Ctrl";
            this.mod_ctrl.UseVisualStyleBackColor = true;
            // 
            // mod_alt
            // 
            this.mod_alt.AutoSize = true;
            this.mod_alt.Dock = System.Windows.Forms.DockStyle.Left;
            this.mod_alt.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mod_alt.Location = new System.Drawing.Point(406, 0);
            this.mod_alt.Name = "mod_alt";
            this.mod_alt.Size = new System.Drawing.Size(41, 24);
            this.mod_alt.TabIndex = 4;
            this.mod_alt.Text = "Alt";
            this.mod_alt.UseVisualStyleBackColor = true;
            // 
            // mod_win
            // 
            this.mod_win.AutoSize = true;
            this.mod_win.Dock = System.Windows.Forms.DockStyle.Left;
            this.mod_win.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mod_win.Location = new System.Drawing.Point(447, 0);
            this.mod_win.Name = "mod_win";
            this.mod_win.Size = new System.Drawing.Size(46, 24);
            this.mod_win.TabIndex = 5;
            this.mod_win.Text = "Win";
            this.mod_win.UseVisualStyleBackColor = true;
            // 
            // EnableHotkey
            // 
            this.EnableHotkey.AutoSize = true;
            this.EnableHotkey.Dock = System.Windows.Forms.DockStyle.Left;
            this.EnableHotkey.Location = new System.Drawing.Point(0, 0);
            this.EnableHotkey.Name = "EnableHotkey";
            this.EnableHotkey.Padding = new System.Windows.Forms.Padding(120, 2, 2, 0);
            this.EnableHotkey.Size = new System.Drawing.Size(137, 24);
            this.EnableHotkey.TabIndex = 6;
            this.EnableHotkey.UseVisualStyleBackColor = true;
            // 
            // hotkey_label
            // 
            this.hotkey_label.AutoSize = true;
            this.hotkey_label.Location = new System.Drawing.Point(0, 0);
            this.hotkey_label.Name = "hotkey_label";
            this.hotkey_label.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.hotkey_label.Size = new System.Drawing.Size(93, 20);
            this.hotkey_label.TabIndex = 7;
            this.hotkey_label.Text = "[Hotkey Name]";
            this.hotkey_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HotkeyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Controls.Add(this.mod_win);
            this.Controls.Add(this.mod_alt);
            this.Controls.Add(this.mod_ctrl);
            this.Controls.Add(this.mod_shift);
            this.Controls.Add(this.hotkey);
            this.Controls.Add(this.hotkey_label);
            this.Controls.Add(this.EnableHotkey);
            this.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "HotkeyEditor";
            this.Size = new System.Drawing.Size(494, 24);
            ((System.ComponentModel.ISupportInitialize)(this.keysConverterBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.keysConverterBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ComboBox hotkey;
        private BindingSource keysConverterBindingSource;
        private CheckBox mod_shift;
        private CheckBox mod_ctrl;
        private CheckBox mod_alt;
        private CheckBox mod_win;
        private BindingSource keysConverterBindingSource1;
        private CheckBox EnableHotkey;
        private Label hotkey_label;
    }
}
