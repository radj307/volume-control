namespace UIComposites
{
    partial class NumberUpDownWithLabel
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
            this.numUpDown = new System.Windows.Forms.NumericUpDown();
            this.panel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown)).BeginInit();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Dock = System.Windows.Forms.DockStyle.Left;
            this.label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label.Location = new System.Drawing.Point(1, 1);
            this.label.Name = "label";
            this.label.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.label.Size = new System.Drawing.Size(54, 21);
            this.label.TabIndex = 0;
            this.label.Text = "[ LABEL ]";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label.UseMnemonic = false;
            // 
            // numUpDown
            // 
            this.numUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numUpDown.Dock = System.Windows.Forms.DockStyle.Right;
            this.numUpDown.Location = new System.Drawing.Point(82, 1);
            this.numUpDown.Name = "numUpDown";
            this.numUpDown.Size = new System.Drawing.Size(50, 23);
            this.numUpDown.TabIndex = 1;
            this.numUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // panel
            // 
            this.panel.Controls.Add(this.numUpDown);
            this.panel.Controls.Add(this.label);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Padding = new System.Windows.Forms.Padding(1);
            this.panel.Size = new System.Drawing.Size(133, 27);
            this.panel.TabIndex = 2;
            // 
            // NumberUpDownWithLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "NumberUpDownWithLabel";
            this.Size = new System.Drawing.Size(133, 27);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public Label label;
        public NumericUpDown numUpDown;
        private Panel panel;
    }
}
