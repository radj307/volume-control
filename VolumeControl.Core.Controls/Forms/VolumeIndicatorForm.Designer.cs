﻿namespace VolumeControl.Core.Controls.Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VolumeIndicatorForm));
            this.cbMuted = new System.Windows.Forms.CheckBox();
            this.cbil = new System.Windows.Forms.ImageList(this.components);
            this.tbLevel = new System.Windows.Forms.TrackBar();
            this.outerPanel = new System.Windows.Forms.Panel();
            this.InnerPanel = new System.Windows.Forms.Panel();
            this.labelVolume = new System.Windows.Forms.Label();
            this.tTimeout = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tbLevel)).BeginInit();
            this.outerPanel.SuspendLayout();
            this.InnerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbMuted
            // 
            this.cbMuted.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMuted.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbMuted.AutoSize = true;
            this.cbMuted.BackColor = System.Drawing.Color.Transparent;
            this.cbMuted.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbMuted.Checked = true;
            this.cbMuted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMuted.FlatAppearance.BorderSize = 0;
            this.cbMuted.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.cbMuted.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cbMuted.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cbMuted.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbMuted.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.cbMuted.ImageIndex = 1;
            this.cbMuted.ImageList = this.cbil;
            this.cbMuted.Location = new System.Drawing.Point(2, 165);
            this.cbMuted.Name = "cbMuted";
            this.cbMuted.Size = new System.Drawing.Size(22, 25);
            this.cbMuted.TabIndex = 0;
            this.cbMuted.Text = " ";
            this.cbMuted.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbMuted.UseMnemonic = false;
            this.cbMuted.UseVisualStyleBackColor = false;
            this.cbMuted.CheckedChanged += new System.EventHandler(this.cbMuted_CheckedChanged);
            this.cbMuted.CheckStateChanged += new System.EventHandler(this.cbMuted_CheckStateChanged);
            // 
            // cbil
            // 
            this.cbil.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.cbil.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("cbil.ImageStream")));
            this.cbil.TransparentColor = System.Drawing.Color.Transparent;
            this.cbil.Images.SetKeyName(0, "unmute-white.png");
            this.cbil.Images.SetKeyName(1, "mute-white.png");
            // 
            // tbLevel
            // 
            this.tbLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLevel.AutoSize = false;
            this.tbLevel.Location = new System.Drawing.Point(0, 0);
            this.tbLevel.Margin = new System.Windows.Forms.Padding(1);
            this.tbLevel.Maximum = 100;
            this.tbLevel.Name = "tbLevel";
            this.tbLevel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbLevel.Size = new System.Drawing.Size(26, 150);
            this.tbLevel.TabIndex = 1;
            this.tbLevel.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbLevel.Value = 100;
            this.tbLevel.ValueChanged += new System.EventHandler(this.tbLevel_ValueChanged);
            // 
            // outerPanel
            // 
            this.outerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.outerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.outerPanel.Controls.Add(this.InnerPanel);
            this.outerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerPanel.Location = new System.Drawing.Point(0, 0);
            this.outerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.outerPanel.Name = "outerPanel";
            this.outerPanel.Padding = new System.Windows.Forms.Padding(2);
            this.outerPanel.Size = new System.Drawing.Size(32, 198);
            this.outerPanel.TabIndex = 1;
            // 
            // InnerPanel
            // 
            this.InnerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.InnerPanel.Controls.Add(this.cbMuted);
            this.InnerPanel.Controls.Add(this.labelVolume);
            this.InnerPanel.Controls.Add(this.tbLevel);
            this.InnerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InnerPanel.Location = new System.Drawing.Point(2, 2);
            this.InnerPanel.Name = "InnerPanel";
            this.InnerPanel.Size = new System.Drawing.Size(26, 192);
            this.InnerPanel.TabIndex = 1;
            // 
            // labelVolume
            // 
            this.labelVolume.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVolume.BackColor = System.Drawing.Color.Transparent;
            this.labelVolume.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.labelVolume.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Polite;
            this.labelVolume.Location = new System.Drawing.Point(0, 150);
            this.labelVolume.Name = "labelVolume";
            this.labelVolume.Size = new System.Drawing.Size(26, 15);
            this.labelVolume.TabIndex = 1;
            this.labelVolume.Text = "100";
            this.labelVolume.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelVolume.UseMnemonic = false;
            // 
            // tTimeout
            // 
            this.tTimeout.Interval = 3000;
            this.tTimeout.Tick += new System.EventHandler(this.tTimeout_Tick);
            // 
            // VolumeIndicatorForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(32, 198);
            this.ControlBox = false;
            this.Controls.Add(this.outerPanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(32, 198);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(32, 198);
            this.Name = "VolumeIndicatorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Volume Indicator";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.tbLevel)).EndInit();
            this.outerPanel.ResumeLayout(false);
            this.InnerPanel.ResumeLayout(false);
            this.InnerPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private CheckBox cbMuted;
        private TrackBar tbLevel;
        private Panel outerPanel;
        private Panel InnerPanel;
        private Label labelVolume;
        private System.Windows.Forms.Timer tTimeout;
        private ImageList cbil;
    }
}