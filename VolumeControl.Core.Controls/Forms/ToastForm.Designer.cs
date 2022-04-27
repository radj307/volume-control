namespace VolumeControl.Core.Controls
{
    partial class ToastForm
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
            this.listPanelInner = new System.Windows.Forms.Panel();
            this.listBox = new System.Windows.Forms.ListBox();
            this.bsAudioProcessAPI = new System.Windows.Forms.BindingSource(this.components);
            this.listStateImages = new System.Windows.Forms.ImageList(this.components);
            this.tTimeout = new System.Windows.Forms.Timer(this.components);
            this.ilLocked = new System.Windows.Forms.ImageList(this.components);
            this.listPanelOuter = new System.Windows.Forms.Panel();
            this.colorPanel = new System.Windows.Forms.Panel();
            this.listPanelInner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsAudioProcessAPI)).BeginInit();
            this.listPanelOuter.SuspendLayout();
            this.colorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // listPanelInner
            // 
            this.listPanelInner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.listPanelInner.Controls.Add(this.listBox);
            this.listPanelInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listPanelInner.Location = new System.Drawing.Point(3, 3);
            this.listPanelInner.Name = "listPanelInner";
            this.listPanelInner.Padding = new System.Windows.Forms.Padding(2);
            this.listPanelInner.Size = new System.Drawing.Size(92, 92);
            this.listPanelInner.TabIndex = 0;
            this.listPanelInner.MouseEnter += new System.EventHandler(this.ToastForm_MouseEnter);
            this.listPanelInner.MouseLeave += new System.EventHandler(this.ToastForm_MouseLeave);
            // 
            // listBox
            // 
            this.listBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox.DataSource = this.bsAudioProcessAPI;
            this.listBox.DisplayMember = "ProcessName";
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.listBox.ForeColor = System.Drawing.Color.White;
            this.listBox.ItemHeight = 17;
            this.listBox.Location = new System.Drawing.Point(2, 2);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(88, 88);
            this.listBox.TabIndex = 0;
            this.listBox.ValueMember = "ProcessName";
            this.listBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseClick);
            this.listBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
            this.listBox.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.listBox_AddedRemoved);
            this.listBox.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.listBox_AddedRemoved);
            this.listBox.MouseEnter += new System.EventHandler(this.ToastForm_MouseEnter);
            this.listBox.MouseLeave += new System.EventHandler(this.ToastForm_MouseLeave);
            // 
            // bsAudioProcessAPI
            // 
            this.bsAudioProcessAPI.DataMember = "ProcessList";
            this.bsAudioProcessAPI.DataSource = typeof(VolumeControl.Core.AudioProcessAPI);
            // 
            // listStateImages
            // 
            this.listStateImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.listStateImages.ImageSize = new System.Drawing.Size(16, 16);
            this.listStateImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tTimeout
            // 
            this.tTimeout.Interval = 3000;
            this.tTimeout.Tick += new System.EventHandler(this.tTimeout_Tick);
            // 
            // ilLocked
            // 
            this.ilLocked.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilLocked.ImageSize = new System.Drawing.Size(16, 16);
            this.ilLocked.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // listPanelOuter
            // 
            this.listPanelOuter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.listPanelOuter.Controls.Add(this.colorPanel);
            this.listPanelOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listPanelOuter.ForeColor = System.Drawing.Color.Transparent;
            this.listPanelOuter.Location = new System.Drawing.Point(0, 0);
            this.listPanelOuter.Name = "listPanelOuter";
            this.listPanelOuter.Padding = new System.Windows.Forms.Padding(1);
            this.listPanelOuter.Size = new System.Drawing.Size(100, 100);
            this.listPanelOuter.TabIndex = 1;
            this.listPanelOuter.MouseEnter += new System.EventHandler(this.ToastForm_MouseEnter);
            this.listPanelOuter.MouseLeave += new System.EventHandler(this.ToastForm_MouseLeave);
            // 
            // colorPanel
            // 
            this.colorPanel.BackColor = System.Drawing.Color.LimeGreen;
            this.colorPanel.Controls.Add(this.listPanelInner);
            this.colorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colorPanel.Location = new System.Drawing.Point(1, 1);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Padding = new System.Windows.Forms.Padding(3);
            this.colorPanel.Size = new System.Drawing.Size(98, 98);
            this.colorPanel.TabIndex = 1;
            this.colorPanel.MouseEnter += new System.EventHandler(this.ToastForm_MouseEnter);
            this.colorPanel.MouseLeave += new System.EventHandler(this.ToastForm_MouseLeave);
            // 
            // ToastForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(100, 100);
            this.ControlBox = false;
            this.Controls.Add(this.listPanelOuter);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MdiChildrenMinimizedAnchorBottom = false;
            this.MinimizeBox = false;
            this.Name = "ToastForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "VolumeControl Toast Notification";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ToastForm_Load);
            this.MouseEnter += new System.EventHandler(this.ToastForm_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ToastForm_MouseLeave);
            this.listPanelInner.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bsAudioProcessAPI)).EndInit();
            this.listPanelOuter.ResumeLayout(false);
            this.colorPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel listPanelInner;
        private ImageList listStateImages;
        private System.Windows.Forms.Timer tTimeout;
        private BindingSource bsAudioProcessAPI;
        private ListBox listBox;
        private ImageList ilLocked;
        private Panel listPanelOuter;
        private Panel colorPanel;
    }
}