namespace AudioMixer
{
    public class VirtualButton : Button, IButtonControl
    {
        public VirtualButton(EventHandler handler)
        {
            Action = handler;
            Click += Action;
            DoubleClick += Action;
        }

        public EventHandler Action { get; set; }
    }
}
