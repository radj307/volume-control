﻿namespace VolumeControl.Core.Controls
{
    public class DoubleBufferedDataGridView : DataGridView
    {
        public DoubleBufferedDataGridView()
        {
            base.DoubleBuffered = true;
        }

        public new bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        public void ResetDataSource(object dataSource, Action? refreshFunction = null, bool setToNullFirst = false)
        {
            SuspendLayout();
            if (setToNullFirst)
                DataSource = null;
            refreshFunction?.Invoke();
            DataSource = dataSource;
            ResumeLayout();
            Refresh();
        }
    }
}