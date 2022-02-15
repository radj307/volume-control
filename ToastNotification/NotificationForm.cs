using UIComposites;

namespace TargetListForm
{
    public partial class NotificationForm : Form
    {
        private readonly CancelButtonHandler cancelHandler = new();
        private NotifState state = NotifState.WAIT;

        public string MessageText
        {
            get => Message.Text;
            set => Message.Text = value;
        }
        public Color ForegroundColor
        {
            get => Message.ForeColor;
            set => Message.ForeColor = value;
        }
        public Color BackgroundColor
        {
            get => BackColor;
            set => BackColor = value;
        }

        private int PosX
        {
            get => Location.X;
            set
            {
                Location = new Point(value, Location.Y);
            }
        }
        private int PosY
        {
            get => Location.Y;
            set
            {
                Location = new Point(Location.X, value);
            }
        }

        public NotificationForm()
        {
            InitializeComponent();

            cancelHandler.Action += delegate { NotifTimer.Interval = 1; };
            CancelButton = cancelHandler;
        }

        private void NotifTimer_Tick(object? sender, EventArgs e)
        {
            NotifTimer.Stop();
            NotifTimer.Enabled = false;
            state = NotifState.CLOSE;
        }

        private void FadeTimer_Tick(object? sender, EventArgs e)
        {
            switch (state)
            {
            case NotifState.WAIT:
                break;
            case NotifState.OPEN:
                Opacity += 0.1;
                break;
            case NotifState.CLOSE:
                Opacity -= 0.1;
                if (Opacity == 0.0)
                    HideNotification();
                break;
            }
        }

        private void Form_Click(object? sender, EventArgs e)
        {
            NotifTimer.Interval = 1;
        }

        private void HideNotification()
        {
            WindowState = FormWindowState.Minimized;
            Visible = false;
        }

        public void ShowNotification(string message, int timeout, Image? img, Color bg_color, Color fg_color, int paddingRight = 5, int paddingBottom = 20)
        {
            // make fully transparent, but visible
            Opacity = 0.0;

            // Start form position at the screen width so it starts off the right side of the main screen
            StartPosition = FormStartPosition.Manual;
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            Location = new Point(screenWidth, Screen.PrimaryScreen.WorkingArea.Height - Height - paddingBottom);
            int posXFinal = screenWidth - Width - paddingRight,
                posXDiff = screenWidth - posXFinal;

            // Set up notification content
            MessageText = message;
            NotifTimer.Interval = timeout;
            if (img != null)
                ImageBox.Image = img!;
            BackgroundColor = bg_color;
            ForegroundColor = fg_color;

            // begin fade in
            state = NotifState.OPEN;
            FadeTimer.Start();
            // begin slide in
            for (int i = 0; i < posXDiff; ++i)
                --PosX;

            // set the position manually if something went wrong
            if (PosX != posXFinal)
                PosX = posXFinal;

            NotifTimer.Start();
            state = NotifState.WAIT;
        }
    }
}
