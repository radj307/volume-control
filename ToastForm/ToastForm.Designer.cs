namespace VolumeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToastForm));
            this.ListDisplay = new System.Windows.Forms.ListView();
            this.name = new System.Windows.Forms.ColumnHeader();
            this.StateImages = new System.Windows.Forms.ImageList(this.components);
            this.NotifyTimer = new System.Windows.Forms.Timer(this.components);
            this.DisplayPanel = new System.Windows.Forms.Panel();
            this.VolumeStateImageCache = new System.Windows.Forms.ImageList(this.components);
            this.DisplayPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ListDisplay
            // 
            this.ListDisplay.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.ListDisplay.AutoArrange = false;
            this.ListDisplay.BackColor = System.Drawing.SystemColors.Menu;
            this.ListDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ListDisplay.CheckBoxes = true;
            this.ListDisplay.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name});
            this.ListDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListDisplay.FullRowSelect = true;
            this.ListDisplay.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.ListDisplay.HideSelection = true;
            this.ListDisplay.Location = new System.Drawing.Point(2, 2);
            this.ListDisplay.MultiSelect = false;
            this.ListDisplay.Name = "ListDisplay";
            this.ListDisplay.Scrollable = false;
            this.ListDisplay.ShowGroups = false;
            this.ListDisplay.Size = new System.Drawing.Size(196, 46);
            this.ListDisplay.StateImageList = this.StateImages;
            this.ListDisplay.TabIndex = 0;
            this.ListDisplay.UseCompatibleStateImageBehavior = false;
            this.ListDisplay.View = System.Windows.Forms.View.Details;
            this.ListDisplay.ItemActivate += new System.EventHandler(this.ListDisplay_ItemActivate);
            this.ListDisplay.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListDisplay_ItemCheck);
            this.ListDisplay.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListDisplay_ItemChecked);
            // 
            // name
            // 
            this.name.Text = "";
            this.name.Width = 200;
            // 
            // StateImages
            // 
            this.StateImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth4Bit;
            this.StateImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("StateImages.ImageStream")));
            this.StateImages.TransparentColor = System.Drawing.Color.Transparent;
            this.StateImages.Images.SetKeyName(0, "Unchecked");
            this.StateImages.Images.SetKeyName(1, "Checked");
            // 
            // NotifyTimer
            // 
            this.NotifyTimer.Interval = 500;
            this.NotifyTimer.Tick += new System.EventHandler(this.Hide);
            // 
            // DisplayPanel
            // 
            this.DisplayPanel.BackColor = System.Drawing.SystemColors.Window;
            this.DisplayPanel.Controls.Add(this.ListDisplay);
            this.DisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DisplayPanel.ForeColor = System.Drawing.Color.Transparent;
            this.DisplayPanel.Location = new System.Drawing.Point(0, 0);
            this.DisplayPanel.Name = "DisplayPanel";
            this.DisplayPanel.Padding = new System.Windows.Forms.Padding(2);
            this.DisplayPanel.Size = new System.Drawing.Size(200, 50);
            this.DisplayPanel.TabIndex = 1;
            // 
            // VolumeStateImageCache
            // 
            this.VolumeStateImageCache.ColorDepth = System.Windows.Forms.ColorDepth.Depth4Bit;
            this.VolumeStateImageCache.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("VolumeStateImageCache.ImageStream")));
            this.VolumeStateImageCache.TransparentColor = System.Drawing.Color.Transparent;
            this.VolumeStateImageCache.Images.SetKeyName(0, "tgt.png");
            this.VolumeStateImageCache.Images.SetKeyName(1, "0.png");
            this.VolumeStateImageCache.Images.SetKeyName(2, "1.png");
            this.VolumeStateImageCache.Images.SetKeyName(3, "2.png");
            this.VolumeStateImageCache.Images.SetKeyName(4, "3.png");
            // 
            // ToastForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(200, 50);
            this.ControlBox = false;
            this.Controls.Add(this.DisplayPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 50);
            this.Name = "ToastForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ToastForm";
            this.TopMost = true;
            this.DisplayPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public ListView ListDisplay;
        public ColumnHeader name;
        public System.Windows.Forms.Timer NotifyTimer;
        private Panel DisplayPanel;
        private ImageList StateImages;
        private ImageList VolumeStateImageCache;
    }
}