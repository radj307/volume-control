namespace UIComposites
{
    partial class TargetSelector
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
            this.components = new System.ComponentModel.Container();
            this.cmb_Target = new System.Windows.Forms.ComboBox();
            this.cmb_Target_BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label_Target = new System.Windows.Forms.Label();
            this.b_Reload = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.Panel();
            this.cmb_Panel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.cmb_Target_BindingSource)).BeginInit();
            this.panel.SuspendLayout();
            this.cmb_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmb_Target
            // 
            this.cmb_Target.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmb_Target.DataSource = this.cmb_Target_BindingSource;
            this.cmb_Target.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmb_Target.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Target.Location = new System.Drawing.Point(0, 0);
            this.cmb_Target.MaxDropDownItems = 12;
            this.cmb_Target.Name = "cmb_Target";
            this.cmb_Target.Size = new System.Drawing.Size(238, 23);
            this.cmb_Target.TabIndex = 0;
            // 
            // label_Target
            // 
            this.label_Target.AutoSize = true;
            this.label_Target.ForeColor = System.Drawing.Color.Black;
            this.label_Target.Location = new System.Drawing.Point(3, 3);
            this.label_Target.Name = "label_Target";
            this.label_Target.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.label_Target.Size = new System.Drawing.Size(39, 16);
            this.label_Target.TabIndex = 1;
            this.label_Target.Text = "Target";
            this.label_Target.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_Target.UseMnemonic = false;
            // 
            // b_Reload
            // 
            this.b_Reload.BackColor = System.Drawing.Color.Transparent;
            this.b_Reload.CausesValidation = false;
            this.b_Reload.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.b_Reload.ForeColor = System.Drawing.Color.Black;
            this.b_Reload.Location = new System.Drawing.Point(293, 1);
            this.b_Reload.Name = "b_Reload";
            this.b_Reload.Size = new System.Drawing.Size(53, 23);
            this.b_Reload.TabIndex = 1;
            this.b_Reload.Text = "Reload";
            this.b_Reload.UseVisualStyleBackColor = false;
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.Transparent;
            this.panel.Controls.Add(this.cmb_Panel);
            this.panel.Controls.Add(this.b_Reload);
            this.panel.Controls.Add(this.label_Target);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.ForeColor = System.Drawing.Color.Transparent;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Padding = new System.Windows.Forms.Padding(1);
            this.panel.Size = new System.Drawing.Size(350, 25);
            this.panel.TabIndex = 3;
            // 
            // cmb_Panel
            // 
            this.cmb_Panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cmb_Panel.Controls.Add(this.cmb_Target);
            this.cmb_Panel.Location = new System.Drawing.Point(48, 1);
            this.cmb_Panel.Name = "cmb_Panel";
            this.cmb_Panel.Size = new System.Drawing.Size(240, 23);
            this.cmb_Panel.TabIndex = 3;
            // 
            // TargetSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "TargetSelector";
            this.Size = new System.Drawing.Size(350, 25);
            ((System.ComponentModel.ISupportInitialize)(this.cmb_Target_BindingSource)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.cmb_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComboBox cmb_Target;
        private Label label_Target;
        private Button b_Reload;
        private Panel panel;
        public BindingSource cmb_Target_BindingSource;
        private Panel cmb_Panel;
    }
}
