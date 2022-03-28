namespace CoreControls
{
    partial class HotkeyEditorForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new CoreControls.DoubleBufferedDataGridView();
            this.bsHotkeyBindingList = new System.Windows.Forms.BindingSource(this.components);
            this.bCancel = new System.Windows.Forms.Button();
            this.registeredDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.keyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.altDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ctrlDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.shiftDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.winDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsHotkeyBindingList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.AutoGenerateColumns = false;
            this.dgv.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgv.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.registeredDataGridViewCheckBoxColumn,
            this.keyDataGridViewTextBoxColumn,
            this.altDataGridViewCheckBoxColumn,
            this.ctrlDataGridViewCheckBoxColumn,
            this.shiftDataGridViewCheckBoxColumn,
            this.winDataGridViewCheckBoxColumn});
            this.dgv.DataSource = this.bsHotkeyBindingList;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.DoubleBuffered = true;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv.RowTemplate.Height = 25;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv.Size = new System.Drawing.Size(326, 182);
            this.dgv.TabIndex = 0;
            this.dgv.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgv_CurrentCellDirtyStateChanged);
            // 
            // bsHotkeyBindingList
            // 
            this.bsHotkeyBindingList.DataSource = typeof(Core.HotkeyBindingList);
            // 
            // bCancel
            // 
            this.bCancel.Enabled = false;
            this.bCancel.Location = new System.Drawing.Point(-1, -1);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(0, 0);
            this.bCancel.TabIndex = 1;
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // registeredDataGridViewCheckBoxColumn
            // 
            this.registeredDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.registeredDataGridViewCheckBoxColumn.DataPropertyName = "Registered";
            this.registeredDataGridViewCheckBoxColumn.HeaderText = "Enabled";
            this.registeredDataGridViewCheckBoxColumn.MinimumWidth = 40;
            this.registeredDataGridViewCheckBoxColumn.Name = "registeredDataGridViewCheckBoxColumn";
            this.registeredDataGridViewCheckBoxColumn.Width = 55;
            // 
            // keyDataGridViewTextBoxColumn
            // 
            this.keyDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.keyDataGridViewTextBoxColumn.DataPropertyName = "Key";
            this.keyDataGridViewTextBoxColumn.HeaderText = "Key";
            this.keyDataGridViewTextBoxColumn.Name = "keyDataGridViewTextBoxColumn";
            // 
            // altDataGridViewCheckBoxColumn
            // 
            this.altDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.altDataGridViewCheckBoxColumn.DataPropertyName = "Alt";
            this.altDataGridViewCheckBoxColumn.HeaderText = "Alt";
            this.altDataGridViewCheckBoxColumn.MinimumWidth = 40;
            this.altDataGridViewCheckBoxColumn.Name = "altDataGridViewCheckBoxColumn";
            this.altDataGridViewCheckBoxColumn.Width = 40;
            // 
            // ctrlDataGridViewCheckBoxColumn
            // 
            this.ctrlDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ctrlDataGridViewCheckBoxColumn.DataPropertyName = "Ctrl";
            this.ctrlDataGridViewCheckBoxColumn.HeaderText = "Ctrl";
            this.ctrlDataGridViewCheckBoxColumn.MinimumWidth = 40;
            this.ctrlDataGridViewCheckBoxColumn.Name = "ctrlDataGridViewCheckBoxColumn";
            this.ctrlDataGridViewCheckBoxColumn.Width = 40;
            // 
            // shiftDataGridViewCheckBoxColumn
            // 
            this.shiftDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.shiftDataGridViewCheckBoxColumn.DataPropertyName = "Shift";
            this.shiftDataGridViewCheckBoxColumn.HeaderText = "Shift";
            this.shiftDataGridViewCheckBoxColumn.MinimumWidth = 40;
            this.shiftDataGridViewCheckBoxColumn.Name = "shiftDataGridViewCheckBoxColumn";
            this.shiftDataGridViewCheckBoxColumn.Width = 40;
            // 
            // winDataGridViewCheckBoxColumn
            // 
            this.winDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.winDataGridViewCheckBoxColumn.DataPropertyName = "Win";
            this.winDataGridViewCheckBoxColumn.HeaderText = "Win";
            this.winDataGridViewCheckBoxColumn.MinimumWidth = 40;
            this.winDataGridViewCheckBoxColumn.Name = "winDataGridViewCheckBoxColumn";
            this.winDataGridViewCheckBoxColumn.Width = 40;
            // 
            // HotkeyEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(326, 182);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.dgv);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HotkeyEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Volume Control Hotkey Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HotkeyEditorForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsHotkeyBindingList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedDataGridView dgv;
        private BindingSource bsHotkeyBindingList;
        private Button bCancel;
        private DataGridViewCheckBoxColumn registeredDataGridViewCheckBoxColumn;
        private DataGridViewTextBoxColumn keyDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn altDataGridViewCheckBoxColumn;
        private DataGridViewCheckBoxColumn ctrlDataGridViewCheckBoxColumn;
        private DataGridViewCheckBoxColumn shiftDataGridViewCheckBoxColumn;
        private DataGridViewCheckBoxColumn winDataGridViewCheckBoxColumn;
    }
}