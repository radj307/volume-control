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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.vbCancel = new VolumeControl.Core.Controls.VirtualButton(this.components);
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
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            this.dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgv.AutoGenerateColumns = false;
            this.dgv.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
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
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.DoubleBuffered = true;
            this.dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.GridColor = System.Drawing.Color.Gainsboro;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            this.dgv.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgv.RowTemplate.Height = 25;
            this.dgv.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv.ShowCellErrors = false;
            this.dgv.ShowCellToolTips = false;
            this.dgv.ShowEditingIcon = false;
            this.dgv.ShowRowErrors = false;
            this.dgv.Size = new System.Drawing.Size(404, 19);
            this.dgv.TabIndex = 0;
            this.dgv.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgv_CellPainting);
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
            this.colEnabled.MinimumWidth = 45;
            this.colEnabled.Name = "colEnabled";
            this.colEnabled.ToolTipText = "Enables the associated hotkey";
            this.colEnabled.Width = 47;
            // 
            // colKey
            // 
            this.colKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colKey.DataPropertyName = "Key";
            this.colKey.DataSource = this.bsKeysList;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            this.colKey.DefaultCellStyle = dataGridViewCellStyle8;
            this.colKey.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.colKey.DropDownWidth = 70;
            this.colKey.FillWeight = 99F;
            this.colKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colKey.HeaderText = "Key";
            this.colKey.MaxDropDownItems = 10;
            this.colKey.MinimumWidth = 100;
            this.colKey.Name = "colKey";
            this.colKey.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colKey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colKey.ToolTipText = "Primary Key / Button";
            // 
            // bsKeysList
            // 
            this.bsKeysList.AllowNew = false;
            this.bsKeysList.DataSource = typeof(VolumeControl.Core.Keyboard.ValidKeys);
            // 
            // colAlt
            // 
            this.colAlt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.colAlt.DataPropertyName = "Alt";
            this.colAlt.HeaderText = "Alt";
            this.colAlt.MinimumWidth = 35;
            this.colAlt.Name = "colAlt";
            this.colAlt.ToolTipText = "Alt Modifier";
            this.colAlt.Width = 35;
            // 
            // colCtrl
            // 
            this.colCtrl.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.colCtrl.DataPropertyName = "Ctrl";
            this.colCtrl.HeaderText = "Ctrl";
            this.colCtrl.MinimumWidth = 35;
            this.colCtrl.Name = "colCtrl";
            this.colCtrl.ToolTipText = "Ctrl Modifier";
            this.colCtrl.Width = 35;
            // 
            // colShift
            // 
            this.colShift.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.colShift.DataPropertyName = "Shift";
            this.colShift.HeaderText = "Shift";
            this.colShift.MinimumWidth = 35;
            this.colShift.Name = "colShift";
            this.colShift.ToolTipText = "Shift Modifier";
            this.colShift.Width = 35;
            // 
            // colWin
            // 
            this.colWin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.colWin.DataPropertyName = "Win";
            this.colWin.HeaderText = "Win";
            this.colWin.MinimumWidth = 35;
            this.colWin.Name = "colWin";
            this.colWin.ToolTipText = "Windows Key Modifier";
            this.colWin.Width = 35;
            // 
            // bsHotkeyBindingList
            // 
            this.bsHotkeyBindingList.DataSource = typeof(VolumeControl.Core.Keyboard.HotkeyBindingList);
            // 
            // vbCancel
            // 
            this.vbCancel.Click += new VolumeControl.Core.Controls.VirtualClickEventHandler(this.vbCancel_Click);
            // 
            // HotkeyEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.CancelButton = this.vbCancel;
            this.ClientSize = new System.Drawing.Size(404, 19);
            this.Controls.Add(this.dgv);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HotkeyEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Hotkey Editor";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.HotkeyEditorForm_HelpButtonClicked);
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
        private BindingSource bsKeysList;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewCheckBoxColumn colEnabled;
        private DataGridViewComboBoxColumn colKey;
        private DataGridViewCheckBoxColumn colAlt;
        private DataGridViewCheckBoxColumn colCtrl;
        private DataGridViewCheckBoxColumn colShift;
        private DataGridViewCheckBoxColumn colWin;
        private VirtualButton vbCancel;
    }
}