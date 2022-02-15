using UIComposites;
namespace TargetListForm
{
    partial class NotificationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificationForm));
            this.NotifTimer = new System.Windows.Forms.Timer(this.components);
            this.Message = new System.Windows.Forms.Label();
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.FadeTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // NotifTimer
            // 
            this.NotifTimer.Enabled = true;
            this.NotifTimer.Interval = 1000;
            this.NotifTimer.Tick += new System.EventHandler(this.NotifTimer_Tick);
            // 
            // Message
            // 
            resources.ApplyResources(this.Message, "Message");
            this.Message.Name = "Message";
            // 
            // ImageBox
            // 
            resources.ApplyResources(this.ImageBox, "ImageBox");
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.TabStop = false;
            // 
            // FadeTimer
            // 
            this.FadeTimer.Enabled = true;
            this.FadeTimer.Interval = 1;
            this.FadeTimer.Tick += new System.EventHandler(this.FadeTimer_Tick);
            // 
            // NotificationForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ControlBox = false;
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.Message);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "NotificationForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.Click += new System.EventHandler(this.Form_Click);
            this.DoubleClick += new System.EventHandler(this.Form_Click);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer NotifTimer;
        private Label Message;
        private PictureBox ImageBox;
        private System.Windows.Forms.Timer FadeTimer;
    }
}