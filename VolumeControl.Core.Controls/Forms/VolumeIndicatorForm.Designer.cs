namespace VolumeControl.Core.Controls.Forms
{
    partial class VolumeIndicatorForm
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
            this.tTimeout = new System.Windows.Forms.Timer(this.components);
            this.flp = new System.Windows.Forms.FlowLayoutPanel();
            this.cbMuted = new System.Windows.Forms.CheckBox();
            this.tbLevel = new System.Windows.Forms.TrackBar();
            this.outerPanel = new System.Windows.Forms.Panel();
            this.flp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbLevel)).BeginInit();
            this.outerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tTimeout
            // 
            this.tTimeout.Interval = 3000;
            this.tTimeout.Tick += new System.EventHandler(this.tTimeout_Tick);
            // 
            // flp
            // 
            this.flp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.flp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flp.Controls.Add(this.cbMuted);
            this.flp.Controls.Add(this.tbLevel);
            this.flp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.flp.Location = new System.Drawing.Point(2, 2);
            this.flp.Name = "flp";
            this.flp.Size = new System.Drawing.Size(23, 152);
            this.flp.TabIndex = 0;
            this.flp.WrapContents = false;
            // 
            // cbMuted
            // 
            this.cbMuted.AutoSize = true;
            this.cbMuted.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.cbMuted.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbMuted.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbMuted.Location = new System.Drawing.Point(3, 133);
            this.cbMuted.Name = "cbMuted";
            this.cbMuted.Size = new System.Drawing.Size(15, 14);
            this.cbMuted.TabIndex = 0;
            this.cbMuted.UseVisualStyleBackColor = false;
            this.cbMuted.CheckedChanged += new System.EventHandler(this.cbMuted_CheckedChanged);
            this.cbMuted.Paint += new System.Windows.Forms.PaintEventHandler(this.cbPaint);
            // 
            // tbLevel
            // 
            this.tbLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLevel.Location = new System.Drawing.Point(1, -1);
            this.tbLevel.Margin = new System.Windows.Forms.Padding(1);
            this.tbLevel.Maximum = 100;
            this.tbLevel.Name = "tbLevel";
            this.tbLevel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbLevel.Size = new System.Drawing.Size(19, 130);
            this.tbLevel.TabIndex = 1;
            this.tbLevel.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbLevel.Value = 50;
            this.tbLevel.ValueChanged += new System.EventHandler(this.tbLevel_ValueChanged);
            // 
            // outerPanel
            // 
            this.outerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.outerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.outerPanel.Controls.Add(this.flp);
            this.outerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerPanel.Location = new System.Drawing.Point(0, 0);
            this.outerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.outerPanel.Name = "outerPanel";
            this.outerPanel.Padding = new System.Windows.Forms.Padding(2);
            this.outerPanel.Size = new System.Drawing.Size(29, 158);
            this.outerPanel.TabIndex = 1;
            // 
            // VolumeIndicatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(29, 158);
            this.ControlBox = false;
            this.Controls.Add(this.outerPanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VolumeIndicatorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Volume Indicator";
            this.TopMost = true;
            this.flp.ResumeLayout(false);
            this.flp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbLevel)).EndInit();
            this.outerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer tTimeout;
        private FlowLayoutPanel flp;
        private CheckBox cbMuted;
        private TrackBar tbLevel;
        private Panel outerPanel;
    }
}