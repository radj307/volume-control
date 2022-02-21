namespace UIComposites
{
    partial class ToastSettings
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
            this.panel = new System.Windows.Forms.Panel();
            this.groupbox = new System.Windows.Forms.GroupBox();
            this.num_Timeout = new UIComposites.NumberUpDownWithLabel();
            this.cb_EnableNotification = new System.Windows.Forms.CheckBox();
            this.panel.SuspendLayout();
            this.groupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.Transparent;
            this.panel.Controls.Add(this.groupbox);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.ForeColor = System.Drawing.Color.Transparent;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Padding = new System.Windows.Forms.Padding(1);
            this.panel.Size = new System.Drawing.Size(180, 75);
            this.panel.TabIndex = 0;
            // 
            // groupbox
            // 
            this.groupbox.BackColor = System.Drawing.SystemColors.Window;
            this.groupbox.Controls.Add(this.num_Timeout);
            this.groupbox.Controls.Add(this.cb_EnableNotification);
            this.groupbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupbox.Location = new System.Drawing.Point(1, 1);
            this.groupbox.Name = "groupbox";
            this.groupbox.Size = new System.Drawing.Size(178, 73);
            this.groupbox.TabIndex = 0;
            this.groupbox.TabStop = false;
            this.groupbox.Text = "Target Switch Settings";
            // 
            // num_Timeout
            // 
            this.num_Timeout.AutoSize = true;
            this.num_Timeout.BackColor = System.Drawing.Color.Transparent;
            this.num_Timeout.ForeColor = System.Drawing.Color.Transparent;
            this.num_Timeout.Increment = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.num_Timeout.LabelText = "Timeout (ms)";
            this.num_Timeout.Location = new System.Drawing.Point(5, 43);
            this.num_Timeout.MaxValue = new decimal(new int[] {
            120000,
            0,
            0,
            0});
            this.num_Timeout.MinValue = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.num_Timeout.Name = "num_Timeout";
            this.num_Timeout.NumberUpDownWidth = 85;
            this.num_Timeout.Size = new System.Drawing.Size(170, 27);
            this.num_Timeout.TabIndex = 1;
            this.num_Timeout.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // cb_EnableNotification
            // 
            this.cb_EnableNotification.AutoSize = true;
            this.cb_EnableNotification.Location = new System.Drawing.Point(6, 20);
            this.cb_EnableNotification.Name = "cb_EnableNotification";
            this.cb_EnableNotification.Size = new System.Drawing.Size(157, 19);
            this.cb_EnableNotification.TabIndex = 0;
            this.cb_EnableNotification.Text = "Enable Toast Notification";
            this.cb_EnableNotification.UseVisualStyleBackColor = true;
            // 
            // ToastSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Controls.Add(this.panel);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "ToastSettings";
            this.Size = new System.Drawing.Size(180, 75);
            this.panel.ResumeLayout(false);
            this.groupbox.ResumeLayout(false);
            this.groupbox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel;
        private GroupBox groupbox;
        private NumberUpDownWithLabel num_Timeout;
        private CheckBox cb_EnableNotification;
    }
}
