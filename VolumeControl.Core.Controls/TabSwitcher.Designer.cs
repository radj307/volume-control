namespace VolumeControl.Core.Controls
{
    partial class TabSwitcher
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
            this.HeaderParent = new System.Windows.Forms.FlowLayoutPanel();
            this.flpItemTemplate = new System.Windows.Forms.Label();
            this.PageParentEnabled = new System.Windows.Forms.Panel();
            this.PageContainer = new System.Windows.Forms.Panel();
            this.PageParentDisabled = new System.Windows.Forms.Panel();
            this.HeaderParent.SuspendLayout();
            this.PageContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // HeaderParent
            // 
            this.HeaderParent.BackColor = System.Drawing.Color.Transparent;
            this.HeaderParent.Controls.Add(this.flpItemTemplate);
            this.HeaderParent.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderParent.Location = new System.Drawing.Point(0, 0);
            this.HeaderParent.Margin = new System.Windows.Forms.Padding(0);
            this.HeaderParent.Name = "HeaderParent";
            this.HeaderParent.Padding = new System.Windows.Forms.Padding(1);
            this.HeaderParent.Size = new System.Drawing.Size(150, 25);
            this.HeaderParent.TabIndex = 0;
            // 
            // flpItemTemplate
            // 
            this.flpItemTemplate.Location = new System.Drawing.Point(4, 1);
            this.flpItemTemplate.Name = "flpItemTemplate";
            this.flpItemTemplate.Size = new System.Drawing.Size(0, 22);
            this.flpItemTemplate.TabIndex = 0;
            // 
            // PageParentEnabled
            // 
            this.PageParentEnabled.BackColor = System.Drawing.Color.Transparent;
            this.PageParentEnabled.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PageParentEnabled.Location = new System.Drawing.Point(0, 0);
            this.PageParentEnabled.Margin = new System.Windows.Forms.Padding(0);
            this.PageParentEnabled.Name = "PageParentEnabled";
            this.PageParentEnabled.Size = new System.Drawing.Size(150, 125);
            this.PageParentEnabled.TabIndex = 0;
            // 
            // PageContainer
            // 
            this.PageContainer.BackColor = System.Drawing.Color.Transparent;
            this.PageContainer.Controls.Add(this.PageParentEnabled);
            this.PageContainer.Controls.Add(this.PageParentDisabled);
            this.PageContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PageContainer.Location = new System.Drawing.Point(0, 25);
            this.PageContainer.Margin = new System.Windows.Forms.Padding(0);
            this.PageContainer.Name = "PageContainer";
            this.PageContainer.Size = new System.Drawing.Size(150, 125);
            this.PageContainer.TabIndex = 1;
            // 
            // PageParentDisabled
            // 
            this.PageParentDisabled.BackColor = System.Drawing.Color.Transparent;
            this.PageParentDisabled.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PageParentDisabled.Location = new System.Drawing.Point(0, 0);
            this.PageParentDisabled.Margin = new System.Windows.Forms.Padding(0);
            this.PageParentDisabled.Name = "PageParentDisabled";
            this.PageParentDisabled.Size = new System.Drawing.Size(150, 125);
            this.PageParentDisabled.TabIndex = 0;
            this.PageParentDisabled.Visible = false;
            // 
            // TabSwitcher
            // 
            this.Controls.Add(this.PageContainer);
            this.Controls.Add(this.HeaderParent);
            this.Name = "TabSwitcher";
            this.HeaderParent.ResumeLayout(false);
            this.PageContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Label flpItemTemplate;
        internal Panel PageParentEnabled;
        internal FlowLayoutPanel HeaderParent;
        private Panel PageContainer;
        internal Panel PageParentDisabled;
    }
}
