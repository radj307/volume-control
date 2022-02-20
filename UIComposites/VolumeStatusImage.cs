namespace UIComposites
{
    public partial class VolumeStatusImage : UserControl
    {
        #region Members

        private Image _null;
        private Image _muted;
        private Image _low;
        private Image _med;
        private Image _high;
        private bool _useWhiteForeground = false;

        #endregion Members

        #region Properties

        private Image NullImage
        {
            get => _null;
            set => _null = value;
        }
        private Image MutedImage
        {
            get => _muted;
            set => _muted = value;
        }
        private Image LowImage
        {
            get => _low;
            set => _low = value;
        }
        private Image MediumImage
        {
            get => _med;
            set => _med = value;
        }
        private Image HighImage
        {
            get => _high;
            set => _high = value;
        }

        private Image ActiveImage
        {
            set
            {
                if (value != null && value != ImageBox.Image)
                    ImageBox.Image = value;
            }
        }

        /// <summary>
        /// When true, uses the white foreground images, otherwise uses the black foreground images.
        /// </summary>
        public bool UseWhiteForeground
        {
            get => _useWhiteForeground;
            set
            {
                if (_useWhiteForeground = value)
                {
                    _null = Properties.Resources.target_null_white;
                    _muted = Properties.Resources.target_0_white;
                    _low = Properties.Resources.target_1_white;
                    _med = Properties.Resources.target_2_white;
                    _high = Properties.Resources.target_3_white;
                }
                else // use black foreground
                {
                    _null = Properties.Resources.target_null;
                    _muted = Properties.Resources.target_0;
                    _low = Properties.Resources.target_1;
                    _med = Properties.Resources.target_2;
                    _high = Properties.Resources.target_3;
                }
            }
        }

        #endregion Properties

        #region Constructor

        public VolumeStatusImage()
        {
            // quiet compiler
            _null = null!;
            _muted = null!;
            _low = null!;
            _med = null!;
            _high = null!;

            // initialize
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

        public void UpdateActiveImage(float currentVolume, bool isMuted)
        {
            if (currentVolume < 0f || currentVolume > 1f)
                ActiveImage = NullImage;
            else if (currentVolume == 0f || isMuted)
                ActiveImage = MutedImage;
            else if (currentVolume < 0.3f)
                ActiveImage = LowImage;
            else if (currentVolume < 0.6f)
                ActiveImage = MediumImage;
            else if (currentVolume < 1f)
                ActiveImage = HighImage;
        }

        #endregion Methods
    }
}
