namespace VolumeControl.Core.Controls
{
    partial class RepeaterButton
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
            this.b = new System.Windows.Forms.Button();
            this.tRepeater = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // b
            // 
            this.b.BackColor = System.Drawing.SystemColors.Control;
            this.b.Dock = System.Windows.Forms.DockStyle.Fill;
            this.b.ForeColor = System.Drawing.SystemColors.ControlText;
            this.b.Location = new System.Drawing.Point(0, 0);
            this.b.Name = "b";
            this.b.Size = new System.Drawing.Size(50, 23);
            this.b.TabIndex = 0;
            this.b.UseVisualStyleBackColor = false;
            this.b.KeyDown += new System.Windows.Forms.KeyEventHandler(this.b_KeyDown);
            this.b.KeyUp += new System.Windows.Forms.KeyEventHandler(this.b_KeyUp);
            this.b.MouseDown += new System.Windows.Forms.MouseEventHandler(this.b_MouseDown);
            this.b.MouseUp += new System.Windows.Forms.MouseEventHandler(this.b_MouseUp);
            // 
            // tRepeater
            // 
            this.tRepeater.Tick += new System.EventHandler(this.tRepeater_Tick);
            // 
            // RepeaterButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.b);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "RepeaterButton";
            this.Size = new System.Drawing.Size(50, 23);
            this.ResumeLayout(false);

        }

        #endregion

        private Button b;
        private System.Windows.Forms.Timer tRepeater;
    }
}
