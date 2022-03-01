using AudioAPI.Forms;

namespace AudioMixer
{
    partial class MixerForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MixerForm));
            this.grid = new System.Windows.Forms.DataGridView();
            this.gridBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ReloadTimer = new System.Windows.Forms.Timer(this.components);
            this.panel = new System.Windows.Forms.Panel();
            this.dgvColumn_PID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvColumn_ProcessName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvColumn_Volume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvColumn_Muted = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBindingSource)).BeginInit();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToResizeRows = false;
            this.grid.AutoGenerateColumns = false;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvColumn_PID,
            this.dgvColumn_ProcessName,
            this.dgvColumn_Volume,
            this.dgvColumn_Muted});
            this.grid.DataSource = this.gridBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid.DefaultCellStyle = dataGridViewCellStyle2;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.GridColor = System.Drawing.SystemColors.ControlLight;
            this.grid.Location = new System.Drawing.Point(5, 5);
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.grid.RowHeadersWidth = 25;
            this.grid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.grid.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.grid.RowTemplate.Height = 25;
            this.grid.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.grid.ShowEditingIcon = false;
            this.grid.Size = new System.Drawing.Size(374, 51);
            this.grid.TabIndex = 0;
            this.grid.CurrentCellDirtyStateChanged += new System.EventHandler(this.grid_CurrentCellDirtyStateChanged);
            this.grid.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.grid_RowsAdded);
            // 
            // gridBindingSource
            // 
            this.gridBindingSource.DataSource = typeof(AudioAPI.AudioProcessList);
            // 
            // ReloadTimer
            // 
            this.ReloadTimer.Interval = 500;
            this.ReloadTimer.Tick += new System.EventHandler(this.ReloadTimer_Tick);
            // 
            // panel
            // 
            this.panel.Controls.Add(this.grid);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Padding = new System.Windows.Forms.Padding(5);
            this.panel.Size = new System.Drawing.Size(384, 61);
            this.panel.TabIndex = 1;
            // 
            // dgvColumn_PID
            // 
            this.dgvColumn_PID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvColumn_PID.DataPropertyName = "PID";
            this.dgvColumn_PID.HeaderText = "PID";
            this.dgvColumn_PID.Name = "dgvColumn_PID";
            this.dgvColumn_PID.ReadOnly = true;
            this.dgvColumn_PID.Width = 50;
            // 
            // dgvColumn_ProcessName
            // 
            this.dgvColumn_ProcessName.DataPropertyName = "ProcessName";
            this.dgvColumn_ProcessName.HeaderText = "Process Name";
            this.dgvColumn_ProcessName.Name = "dgvColumn_ProcessName";
            this.dgvColumn_ProcessName.ReadOnly = true;
            // 
            // dgvColumn_Volume
            // 
            this.dgvColumn_Volume.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvColumn_Volume.DataPropertyName = "VolumePercent";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dgvColumn_Volume.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvColumn_Volume.HeaderText = "Volume";
            this.dgvColumn_Volume.MaxInputLength = 4;
            this.dgvColumn_Volume.MinimumWidth = 20;
            this.dgvColumn_Volume.Name = "dgvColumn_Volume";
            this.dgvColumn_Volume.Width = 72;
            // 
            // dgvColumn_Muted
            // 
            this.dgvColumn_Muted.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvColumn_Muted.DataPropertyName = "Muted";
            this.dgvColumn_Muted.FalseValue = "false";
            this.dgvColumn_Muted.HeaderText = "Muted";
            this.dgvColumn_Muted.Name = "dgvColumn_Muted";
            this.dgvColumn_Muted.TrueValue = "true";
            this.dgvColumn_Muted.Width = 48;
            // 
            // MixerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(384, 61);
            this.Controls.Add(this.panel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MixerForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Audio Mixer";
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBindingSource)).EndInit();
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView grid;
        private BindingSource gridBindingSource;
        private System.Windows.Forms.Timer ReloadTimer;
        private Panel panel;
        private DataGridViewTextBoxColumn dgvColumn_PID;
        private DataGridViewTextBoxColumn dgvColumn_ProcessName;
        private DataGridViewTextBoxColumn dgvColumn_Volume;
        private DataGridViewCheckBoxColumn dgvColumn_Muted;
    }
}