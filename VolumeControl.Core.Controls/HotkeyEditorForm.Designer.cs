using VolumeControl.Core.Keyboard;

namespace VolumeControl.Core.Controls
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new VolumeControl.Core.Controls.DoubleBufferedDataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colKey = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.bsKeysList = new System.Windows.Forms.BindingSource(this.components);
            this.colAlt = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colCtrl = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colShift = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colWin = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bsHotkeyBindingList = new System.Windows.Forms.BindingSource(this.components);
            this.bCancel = new System.Windows.Forms.Button();
            this.dgvPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsKeysList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsHotkeyBindingList)).BeginInit();
            this.dgvPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv.AutoGenerateColumns = false;
            this.dgv.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colEnabled,
            this.colKey,
            this.colAlt,
            this.colCtrl,
            this.colShift,
            this.colWin});
            this.dgv.DataSource = this.bsHotkeyBindingList;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.DoubleBuffered = true;
            this.dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgv.Location = new System.Drawing.Point(5, 5);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv.RowTemplate.Height = 25;
            this.dgv.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv.ShowEditingIcon = false;
            this.dgv.Size = new System.Drawing.Size(419, 26);
            this.dgv.TabIndex = 0;
            this.dgv.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgv_CurrentCellDirtyStateChanged);
            this.dgv.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgv_DataError);
            this.dgv.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgv_RowsAdded);
            this.dgv.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgv_RowsRemoved);
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "Name";
            this.colName.MinimumWidth = 110;
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colEnabled
            // 
            this.colEnabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colEnabled.DataPropertyName = "Registered";
            this.colEnabled.HeaderText = "Active";
            this.colEnabled.MinimumWidth = 40;
            this.colEnabled.Name = "colEnabled";
            this.colEnabled.Width = 46;
            // 
            // colKey
            // 
            this.colKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colKey.DataPropertyName = "Key";
            this.colKey.DataSource = this.bsKeysList;
            this.colKey.DropDownWidth = 70;
            this.colKey.HeaderText = "Key";
            this.colKey.MaxDropDownItems = 10;
            this.colKey.MinimumWidth = 100;
            this.colKey.Name = "colKey";
            this.colKey.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colKey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // bsKeysList
            // 
            this.bsKeysList.AllowNew = false;
            this.bsKeysList.DataSource = typeof(VolumeControl.Core.Keyboard.ValidKeys);
            // 
            // colAlt
            // 
            this.colAlt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colAlt.DataPropertyName = "Alt";
            this.colAlt.HeaderText = "Alt";
            this.colAlt.MinimumWidth = 40;
            this.colAlt.Name = "colAlt";
            this.colAlt.Width = 40;
            // 
            // colCtrl
            // 
            this.colCtrl.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colCtrl.DataPropertyName = "Ctrl";
            this.colCtrl.HeaderText = "Ctrl";
            this.colCtrl.MinimumWidth = 40;
            this.colCtrl.Name = "colCtrl";
            this.colCtrl.Width = 40;
            // 
            // colShift
            // 
            this.colShift.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colShift.DataPropertyName = "Shift";
            this.colShift.HeaderText = "Shift";
            this.colShift.MinimumWidth = 40;
            this.colShift.Name = "colShift";
            this.colShift.Width = 40;
            // 
            // colWin
            // 
            this.colWin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colWin.DataPropertyName = "Win";
            this.colWin.HeaderText = "Win";
            this.colWin.MinimumWidth = 40;
            this.colWin.Name = "colWin";
            this.colWin.Width = 40;
            // 
            // bsHotkeyBindingList
            // 
            this.bsHotkeyBindingList.DataSource = typeof(HotkeyBindingList);
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
            // dgvPanel
            // 
            this.dgvPanel.Controls.Add(this.dgv);
            this.dgvPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPanel.Location = new System.Drawing.Point(0, 0);
            this.dgvPanel.Name = "dgvPanel";
            this.dgvPanel.Padding = new System.Windows.Forms.Padding(5);
            this.dgvPanel.Size = new System.Drawing.Size(429, 36);
            this.dgvPanel.TabIndex = 2;
            // 
            // HotkeyEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(429, 36);
            this.Controls.Add(this.dgvPanel);
            this.Controls.Add(this.bCancel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(445, 75);
            this.Name = "HotkeyEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Volume Control Hotkey Editor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HotkeyEditorForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsKeysList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsHotkeyBindingList)).EndInit();
            this.dgvPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedDataGridView dgv;
        private BindingSource bsHotkeyBindingList;
        private Button bCancel;
        private Panel dgvPanel;
        private BindingSource bsKeysList;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewCheckBoxColumn colEnabled;
        private DataGridViewComboBoxColumn colKey;
        private DataGridViewCheckBoxColumn colAlt;
        private DataGridViewCheckBoxColumn colCtrl;
        private DataGridViewCheckBoxColumn colShift;
        private DataGridViewCheckBoxColumn colWin;
    }
}