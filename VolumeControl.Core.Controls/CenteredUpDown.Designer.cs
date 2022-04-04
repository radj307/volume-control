namespace VolumeControl.Core.Controls
{
    partial class CenteredUpDown
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
            this.tb = new VolumeControl.Core.Controls.CenteredTextBox();
            this.bUp = new VolumeControl.Core.Controls.RepeaterButton();
            this.bDown = new VolumeControl.Core.Controls.RepeaterButton();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb
            // 
            this.tb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb.EdgePadding = 3;
            this.tb.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.tb.Location = new System.Drawing.Point(0, 0);
            this.tb.Name = "tb";
            this.tb.PlaceholderText = "";
            this.tb.Size = new System.Drawing.Size(58, 23);
            this.tb.TabIndex = 0;
            this.tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // bUp
            // 
            this.bUp.ActivateButtons = new System.Windows.Forms.MouseButtons[] {
        System.Windows.Forms.MouseButtons.Left,
        System.Windows.Forms.MouseButtons.Right};
            this.bUp.ActivateKeys = new System.Windows.Forms.Keys[] {
        System.Windows.Forms.Keys.Return,
        System.Windows.Forms.Keys.Space};
            this.bUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bUp.FlatBorderColor = System.Drawing.Color.Empty;
            this.bUp.FlatBorderSize = 0;
            this.bUp.FlatMouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.bUp.FlatMouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.bUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bUp.Font = new System.Drawing.Font("Segoe UI", 5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.bUp.Location = new System.Drawing.Point(0, 0);
            this.bUp.Margin = new System.Windows.Forms.Padding(1);
            this.bUp.Name = "bUp";
            this.bUp.RepeatDelay = 400;
            this.bUp.RepeatInterval = 100;
            this.bUp.Size = new System.Drawing.Size(20, 11);
            this.bUp.TabIndex = 1;
            this.bUp.UseVisualStyleBackColor = false;
            // 
            // bDown
            // 
            this.bDown.ActivateButtons = new System.Windows.Forms.MouseButtons[] {
        System.Windows.Forms.MouseButtons.Left,
        System.Windows.Forms.MouseButtons.Right};
            this.bDown.ActivateKeys = new System.Windows.Forms.Keys[] {
        System.Windows.Forms.Keys.Return,
        System.Windows.Forms.Keys.Space};
            this.bDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.bDown.FlatBorderColor = System.Drawing.Color.Empty;
            this.bDown.FlatBorderSize = 0;
            this.bDown.FlatMouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.bDown.FlatMouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.bDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bDown.Font = new System.Drawing.Font("Segoe UI", 5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.bDown.Location = new System.Drawing.Point(0, 12);
            this.bDown.Margin = new System.Windows.Forms.Padding(1);
            this.bDown.Name = "bDown";
            this.bDown.RepeatDelay = 400;
            this.bDown.RepeatInterval = 100;
            this.bDown.Size = new System.Drawing.Size(20, 11);
            this.bDown.TabIndex = 2;
            this.bDown.UseVisualStyleBackColor = false;
            // 
            // splitContainer
            // 
            this.splitContainer.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.splitContainer.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.ForeColor = System.Drawing.Color.Transparent;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.splitContainer.Panel1.Controls.Add(this.tb);
            this.splitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.splitContainer.Panel2.Controls.Add(this.bDown);
            this.splitContainer.Panel2.Controls.Add(this.bUp);
            this.splitContainer.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitContainer.Panel2MinSize = 21;
            this.splitContainer.Size = new System.Drawing.Size(80, 23);
            this.splitContainer.SplitterDistance = 58;
            this.splitContainer.SplitterWidth = 1;
            this.splitContainer.TabIndex = 3;
            // 
            // CenteredUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitContainer);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.MaximumSize = new System.Drawing.Size(600, 25);
            this.MinimumSize = new System.Drawing.Size(60, 25);
            this.Name = "CenteredUpDown";
            this.Size = new System.Drawing.Size(80, 23);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CenteredTextBox tb;
        private RepeaterButton bUp;
        private RepeaterButton bDown;
        private SplitContainer splitContainer;
    }
}
