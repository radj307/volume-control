using HotkeyLib;
using System.ComponentModel;

namespace UIComposites
{
    public partial class HotkeyEditor : UserControl
    {
        #region Members

        private readonly BindingSource keyBinding;

        #endregion Members

        #region Constructors

#       pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public HotkeyEditor()
        {
            InitializeComponent();
            keyBinding = new();
            keyBinding.DataSource = ValidKeysList.KeysList;
            Combobox_KeySelector.AutoCompleteSource = AutoCompleteSource.ListItems;
            AutoCompleteStringCollection autocomplete = new();
            foreach (Keys key in ValidKeysList.KeysList)
            {
                autocomplete.Add(key.ToString());
            }
            Combobox_KeySelector.AutoCompleteCustomSource = autocomplete;
            Combobox_KeySelector.DataSource = keyBinding;
        }
#       pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Enable or disable this hotkey editor's "Enabled" checkbox.
        /// </summary>
        /// <param name="enable">When true, box is checked.</param>
        public void SetHotkeyIsEnabled(bool enable) => Checkbox_Enabled.Checked = enable;

        /// <summary>
        /// Set the hotkey name label.
        /// </summary>
        /// <param name="label">String to use as the label text.</param>
        public void SetLabel(string label) => Label_HotkeyName.Text = label;


        #endregion Methods

        #region Properties

        public bool HotkeyIsEnabled => Checkbox_Enabled.Checked;

        public string Label
        {
            get => Label_HotkeyName.Text;
            set => Label_HotkeyName.Text = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Hotkey Hotkey
        {
            get => new(
                    (Keys)Enum.Parse(typeof(Keys), Combobox_KeySelector.Text, true),
                    Checkbox_ModifierKey_Shift.Checked,
                    Checkbox_ModifierKey_Ctrl.Checked,
                    Checkbox_ModifierKey_Alt.Checked,
                    Checkbox_ModifierKey_Win.Checked
                );
            set
            {
                Combobox_KeySelector.Text = Enum.GetName(typeof(Keys), value.KeyCode);
                Checkbox_ModifierKey_Shift.Checked = value.Shift;
                Checkbox_ModifierKey_Ctrl.Checked = value.Control;
                Checkbox_ModifierKey_Alt.Checked = value.Alt;
                Checkbox_ModifierKey_Win.Checked = value.Windows;
            }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Custom Event
        /// Triggers when any key modifiers are changed.
        /// </summary>
        private EventHandler onModifierChanged;
        public event EventHandler ModifierChanged
        {
            add
            {
                onModifierChanged += value;
            }
            remove
            {
#               pragma warning disable CS8601 // Possible null reference assignment.
                onModifierChanged -= value;
#               pragma warning restore CS8601 // Possible null reference assignment.
            }
        }
        /// <summary>
        /// Trigger a ModifierChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnModifierChanged(EventArgs e) => onModifierChanged?.Invoke(this, e);
        /// <summary>
        /// Called when any modifier checkbox is updated, triggers a ModifierChanged event.
        /// </summary>
        private void Checkbox_ModifierKey_CheckedChanged(object sender, EventArgs e) => OnModifierChanged(e);

        #endregion Events
    }
}
