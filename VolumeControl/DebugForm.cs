namespace VolumeControl
{
    public partial class DebugForm : Form
    {
        public DebugForm(MainForm mainForm)
        {
            main = mainForm;
            InitializeComponent();

            splitContainer1.Panel1.Cursor = Cursors.Default;
            splitContainer1.Panel2.Cursor = Cursors.Default;
        }

        private readonly MainForm main;
        private Control[]? matchingControls = null;
        private bool requireFullNameMatch = true;

        public object SelectedObject
        {
            get => propertyGrid1.SelectedObject;
            set => propertyGrid1.SelectedObject = value;
        }
        public string LabelControlCount
        {
            get => label1.Text;
            set => label1.Text = value;
        }
        public int NumberControlIndex
        {
            get => Convert.ToInt32(numericUpDown1.Value);
            set => numericUpDown1.Value = Convert.ToDecimal(value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            matchingControls = main.GetMatchingControls(name, true, requireFullNameMatch);
            int len = matchingControls.Length;
            if (len != 0)
            {
                numericUpDown1.Maximum = len - 1;
                SelectedObject = matchingControls[0];
            }
            else
            {
                numericUpDown1.Maximum = 0;
                SelectedObject = null!;
            }
            LabelControlCount = len.ToString();
            numericUpDown1.Value = 0;
        }

        private void button2_Click(object sender, EventArgs e) => SelectedObject = main;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int index = NumberControlIndex;
            if (matchingControls != null && index < matchingControls.Length)
            {
                SelectedObject = matchingControls[index];
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) => requireFullNameMatch = checkBox1.Checked;
    }
}
