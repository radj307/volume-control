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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new VolumeControl.Core.Controls.DoubleBufferedDataGridView();
            this.bsKeysList = new System.Windows.Forms.BindingSource(this.components);
            this.bsHotkeyBindingList = new System.Windows.Forms.BindingSource(this.components);
            this.bCancel = new System.Windows.Forms.Button();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colKey = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colAlt = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colCtrl = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colShift = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colWin = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsKeysList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsHotkeyBindingList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            this.dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.AutoGenerateColumns = false;
            this.dgv.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colEnabled,
            this.colKey,
            this.colAlt,
            this.colCtrl,
            this.colShift,
            this.colWin});
            this.dgv.DataSource = this.bsHotkeyBindingList;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.DoubleBuffered = true;
            this.dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.GridColor = System.Drawing.Color.Gainsboro;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            this.dgv.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgv.RowTemplate.Height = 25;
            this.dgv.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv.ShowEditingIcon = false;
            this.dgv.Size = new System.Drawing.Size(414, 20);
            this.dgv.TabIndex = 0;
            this.dgv.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgv_CellPainting);
            this.dgv.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgv_CurrentCellDirtyStateChanged);
            this.dgv.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgv_DataError);
            this.dgv.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgv_RowsAdded);
            this.dgv.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgv_RowsRemoved);
            // 
            // bsKeysList
            // 
            this.bsKeysList.AllowNew = false;
            this.bsKeysList.DataSource = typeof(VolumeControl.Core.Keyboard.ValidKeys);
            // 
            // bsHotkeyBindingList
            // 
            this.bsHotkeyBindingList.DataSource = typeof(VolumeControl.Core.Keyboard.HotkeyBindingList);
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
            this.colEnabled.Width = 47;
            // 
            // colKey
            // 
            this.colKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colKey.DataPropertyName = "Key";
            this.colKey.DataSource = this.bsKeysList;
            this.colKey.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.colKey.DropDownWidth = 70;
            this.colKey.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.colKey.HeaderText = "Key";
            this.colKey.MaxDropDownItems = 10;
            this.colKey.MinimumWidth = 100;
            this.colKey.Name = "colKey";
            this.colKey.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colKey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
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
            // HotkeyEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(414, 20);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.bCancel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HotkeyEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Hotkey Editor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HotkeyEditorForm_FormClosing);
            this.Load += new System.EventHandler(this.HotkeyEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsKeysList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsHotkeyBindingList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedDataGridView dgv;
        private BindingSource bsHotkeyBindingList;
        private Button bCancel;
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