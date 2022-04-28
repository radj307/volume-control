namespace VolumeControl.Core.Controls
{
    partial class NumericUpDownWithLabel
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
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.label = new System.Windows.Forms.Label();
            this.nud = new System.Windows.Forms.NumericUpDown();
            this.nudPanel = new System.Windows.Forms.Panel();
            this.flp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud)).BeginInit();
            this.nudPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flp
            // 
            this.flp.BackColor = System.Drawing.Color.Transparent;
            this.flp.Controls.Add(this.label);
            this.flp.Controls.Add(this.nudPanel);
            this.flp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp.ForeColor = System.Drawing.Color.Transparent;
            this.flp.Location = new System.Drawing.Point(0, 0);
            this.flp.Margin = new System.Windows.Forms.Padding(0);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(100, 23);
            this.flp.TabIndex = 0;
            this.flp.WrapContents = false;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Margin = new System.Windows.Forms.Padding(0, 0, 1, 2);
            this.label.Name = "label";
            this.label.Padding = new System.Windows.Forms.Padding(0, 1, 0, 2);
            this.label.Size = new System.Drawing.Size(59, 21);
            this.label.TabIndex = 1;
            this.label.Text = "Label Text";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label.MarginChanged += new System.EventHandler(this.SizeToFit);
            this.label.TextChanged += new System.EventHandler(this.SizeToFit);
            // 
            // nud
            // 
            this.nud.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.nud.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nud.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nud.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.nud.Location = new System.Drawing.Point(2, 0);
            this.nud.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.nud.Name = "nud";
            this.nud.Size = new System.Drawing.Size(36, 23);
            this.nud.TabIndex = 0;
            this.nud.MarginChanged += new System.EventHandler(this.SizeToFit);
            this.nud.Resize += new System.EventHandler(this.SizeToFit);
            // 
            // nudPanel
            // 
            this.nudPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudPanel.Controls.Add(this.nud);
            this.nudPanel.Location = new System.Drawing.Point(60, 0);
            this.nudPanel.Margin = new System.Windows.Forms.Padding(0);
            this.nudPanel.Name = "nudPanel";
            this.nudPanel.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.nudPanel.Size = new System.Drawing.Size(40, 23);
            this.nudPanel.TabIndex = 2;
            // 
            // NumericUpDownWithLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.flp);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "NumericUpDownWithLabel";
            this.Size = new System.Drawing.Size(100, 23);
            this.Load += new System.EventHandler(this.NumericUpDownWithLabel_Load);
            this.flp.ResumeLayout(false);
            this.flp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud)).EndInit();
            this.nudPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel flp;
        private Label label;
        private NumericUpDown nud;
        private Panel nudPanel;
    }
}
