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
            this.ListDisplay = new System.Windows.Forms.ListView();
            this.name = new System.Windows.Forms.ColumnHeader();
            this.NotifyTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ListDisplay
            // 
            this.ListDisplay.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.ListDisplay.AllowColumnReorder = true;
            this.ListDisplay.CheckBoxes = true;
            this.ListDisplay.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name});
            this.ListDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListDisplay.FullRowSelect = true;
            this.ListDisplay.HideSelection = true;
            this.ListDisplay.Location = new System.Drawing.Point(0, 0);
            this.ListDisplay.MultiSelect = false;
            this.ListDisplay.Name = "ListDisplay";
            this.ListDisplay.Scrollable = false;
            this.ListDisplay.ShowGroups = false;
            this.ListDisplay.Size = new System.Drawing.Size(200, 200);
            this.ListDisplay.TabIndex = 0;
            this.ListDisplay.UseCompatibleStateImageBehavior = false;
            this.ListDisplay.View = System.Windows.Forms.View.Details;
            this.ListDisplay.ItemActivate += new System.EventHandler(this.ListDisplay_ItemActivate);
            this.ListDisplay.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListDisplay_ItemCheck);
            this.ListDisplay.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListDisplay_ItemChecked);
            // 
            // name
            // 
            this.name.Text = "Application Name";
            this.name.Width = 200;
            // 
            // NotifyTimer
            // 
            this.NotifyTimer.Enabled = true;
            this.NotifyTimer.Interval = 500;
            // 
            // ToastForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(200, 200);
            this.ControlBox = false;
            this.Controls.Add(this.ListDisplay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "ToastForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ToastForm";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion
        public ListView ListDisplay;
        public ColumnHeader name;
        public System.Windows.Forms.Timer NotifyTimer;
    }
}