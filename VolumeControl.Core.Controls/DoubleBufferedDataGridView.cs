using System.ComponentModel;

namespace VolumeControl.Core.Controls
{
    public class DoubleBufferedDataGridView : DataGridView
    {
        public DoubleBufferedDataGridView() : base()
        {
            base.DoubleBuffered = true;

            CellMouseDown += (delegate (object sender, DataGridViewCellMouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {
                    int row = e.RowIndex, col = e.ColumnIndex;
                    if (row >= 0 && row < Rows.Count && col >= 0 && col < ColumnCount && Rows[e.RowIndex].Cells[e.ColumnIndex].ContentBounds.Contains(e.Location))
                        NotifyCellContentMouseDown(new(col, row));
                }
            })!;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public new bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        /// <summary>
        /// Triggered when the left or right mouse button is pressed down while over a cell's content.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // ensure this is shown in the designer
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [Browsable(true)]
        public event DataGridViewCellEventHandler? CellContentMouseDown;
        private void NotifyCellContentMouseDown(DataGridViewCellEventArgs e) => CellContentMouseDown?.Invoke(this, e);

        /// <summary>
        /// Set the data grid view's data binding source to null.
        /// </summary>
        /// <returns>The previous DataSource.</returns>
        public object UnsetDataSource()
        {
            var ds = DataSource;
            DataSource = null;
            return ds;
        }
        /// <summary>
        /// Set the data grid view's data binding source to the given object.
        /// </summary>
        /// <param name="datasource">A valid DataSource object.</param>
        public void SetDataSource(object datasource)
        {
            DataSource = datasource;
        }
    }
}
