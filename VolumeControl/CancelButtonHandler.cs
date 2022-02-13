namespace VolumeControl
{
    internal class CancelButtonHandler : IButtonControl
    {
        private DialogResult _result;
        private EventHandler? _action = null;

        public EventHandler? Action
        {
            get => _action;
            set => _action = value;
        }

        public DialogResult DialogResult
        {
            get => _result;
            set => _result = value;
        }

        public void NotifyDefault(bool value) { }
        public void PerformClick()
        {
            _action?.Invoke(this, EventArgs.Empty);
        }
    }
}
