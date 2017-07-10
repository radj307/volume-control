namespace Toastify.Common
{
    public class EnumComboBoxItem
    {
        public object Value { get; set; }
        public string Description { get; set; }
        public string Tooltip { get; set; }

        public EnumComboBoxItem(object value, string description, string tooltip = null)
        {
            this.Value = value;
            this.Description = string.IsNullOrWhiteSpace(description) ? value.ToString() : description;
            this.Tooltip = tooltip;
        }
    }
}