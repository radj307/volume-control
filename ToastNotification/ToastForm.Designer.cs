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
            this.index = new System.Windows.Forms.ColumnHeader();
            this.TimeoutTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ListDisplay
            // 
            this.ListDisplay.AllowColumnReorder = true;
            this.ListDisplay.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.index});
            this.ListDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListDisplay.Location = new System.Drawing.Point(0, 0);
            this.ListDisplay.MultiSelect = false;
            this.ListDisplay.Name = "ListDisplay";
            this.ListDisplay.Size = new System.Drawing.Size(284, 161);
            this.ListDisplay.TabIndex = 0;
            this.ListDisplay.UseCompatibleStateImageBehavior = false;
            this.ListDisplay.View = System.Windows.Forms.View.Details;
            // 
            // name
            // 
            this.name.Text = "Application Name";
            this.name.Width = 120;
            // 
            // index
            // 
            this.index.Text = "Index";
            this.index.Width = 50;
            // 
            // TimeoutTimer
            // 
            this.TimeoutTimer.Enabled = true;
            this.TimeoutTimer.Interval = 6000;
            // 
            // ToastForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(284, 161);
            this.Controls.Add(this.ListDisplay);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "ToastForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ToastForm";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion
        private ColumnHeader index;
        public ListView ListDisplay;
        public ColumnHeader name;
        public System.Windows.Forms.Timer TimeoutTimer;
    }
}