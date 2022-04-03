namespace VolumeControl.Core.Controls
{
    partial class CenteredTextBox
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
            this.tb = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tb
            // 
            this.tb.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb.ForeColor = System.Drawing.Color.Black;
            this.tb.Location = new System.Drawing.Point(0, 3);
            this.tb.Name = "tb";
            this.tb.Size = new System.Drawing.Size(150, 16);
            this.tb.TabIndex = 0;
            this.tb.WordWrap = false;
            // 
            // CenteredTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tb);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "CenteredTextBox";
            this.Size = new System.Drawing.Size(148, 20);
            this.Resize += new System.EventHandler(this.CenteredTextBox_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox tb;
    }
}
