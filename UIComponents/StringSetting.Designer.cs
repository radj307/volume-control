namespace UIComponents
{
    partial class StringSetting
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
            this.label = new System.Windows.Forms.Label();
            this.value = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Lucida Sans Unicode", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label.Location = new System.Drawing.Point(3, 5);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(126, 18);
            this.label.TabIndex = 0;
            this.label.Text = "[LABEL]             ";
            // 
            // value
            // 
            this.value.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.value.Location = new System.Drawing.Point(135, 3);
            this.value.Name = "value";
            this.value.Size = new System.Drawing.Size(438, 23);
            this.value.TabIndex = 1;
            // 
            // StringSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.value);
            this.Controls.Add(this.label);
            this.Name = "StringSetting";
            this.Size = new System.Drawing.Size(576, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label;
        private TextBox value;
    }
}
