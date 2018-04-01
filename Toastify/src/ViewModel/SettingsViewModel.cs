using System;
using System.Security;
using System.Windows.Forms;
using System.Windows.Media;
using Toastify.Common;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Model;
using ToastifyAPI.Native.Enums;

namespace Toastify.ViewModel
{
    public class SettingsViewModel : ObservableObject
    {
        private const string templateDoubleUpDownAltIncrement = "Hold Ctrl while scrolling or while changing the value using the up/down buttons to increment/decrement by {0} units.";

        private Settings _settings;

        public Settings Settings
        {
            get { return this._settings; }
            private set { this.RaiseAndSetIfChanged(ref this._settings, value); }
        }

        public int CurrentTabIndex { get; set; }

        public int CurrentToastTabIndex { get; set; }

        public string DoubleUpDownAltIncrement10Tooltip { get; } = string.Format(templateDoubleUpDownAltIncrement, 10);

        public string DoubleUpDownAltIncrement1000Tooltip { get; } = string.Format(templateDoubleUpDownAltIncrement, 1000);

        public ImageSource InfoIcon
        {
            get { return ToastifyAPI.Win32API.GetStockIconImage(ShStockIconId.SIID_INFO, true); }
        }

        public ImageSource WarningIcon
        {
            get { return ToastifyAPI.Win32API.GetStockIconImage(ShStockIconId.SIID_WARNING, true); }
        }

        #region Toast

        public int DisplayTimeDefault { get { return Settings.Default.DisplayTime; } }
        public int DisplayTimeMin { get { return Settings.Default.DisplayTime.Range?.Min ?? 0; } }
        public int DisplayTimeMax { get { return Settings.Default.DisplayTime.Range?.Max ?? int.MaxValue; } }

        #region Size & Position

        public double ToastWidthDefault { get { return Settings.Default.ToastWidth; } }
        public double ToastWidthMin { get { return 200.0; } }
        public double ToastWidthMax { get { return ScreenHelper.CalculateMaxToastWidth(); } }

        public double ToastHeightDefault { get { return Settings.Default.ToastHeight; } }
        public double ToastHeightMin { get { return 70.0; } }
        public double ToastHeightMax { get { return ScreenHelper.CalculateMaxToastHeight(); } }

        public double PositionLeftDefault { get { return Settings.Default.PositionLeft; } }
        public double PositionLeftMin { get { return ScreenHelper.GetTotalWorkingArea().Left; } }
        public double PositionLeftMax { get { return ScreenHelper.GetTotalWorkingArea().Right - this.Settings.ToastWidth; } }

        public double PositionTopDefault { get { return Settings.Default.PositionTop; } }
        public double PositionTopMin { get { return ScreenHelper.GetTotalWorkingArea().Top; } }
        public double PositionTopMax { get { return ScreenHelper.GetTotalWorkingArea().Bottom - this.Settings.ToastHeight; } }

        #endregion Size & Position

        public Color ToastColorTop
        {
            get
            {
                return ColorHelper.HexToColor(this.Settings.ToastColorTop);
            }
            set
            {
                this.Settings.ToastColorTop = ColorHelper.ColorToHex(value);
                this.NotifyPropertyChanged();
            }
        }

        public Color ToastColorBottom
        {
            get
            {
                return ColorHelper.HexToColor(this.Settings.ToastColorBottom);
            }
            set
            {
                this.Settings.ToastColorBottom = ColorHelper.ColorToHex(value);
                this.NotifyPropertyChanged();
            }
        }

        public Color ToastBorderColor
        {
            get
            {
                return ColorHelper.HexToColor(this.Settings.ToastBorderColor);
            }
            set
            {
                this.Settings.ToastBorderColor = ColorHelper.ColorToHex(value);
                this.NotifyPropertyChanged();
            }
        }

        public Color ToastTitle1Color
        {
            get
            {
                return ColorHelper.HexToColor(this.Settings.ToastTitle1Color);
            }
            set
            {
                this.Settings.ToastTitle1Color = ColorHelper.ColorToHex(value);
                this.NotifyPropertyChanged();
            }
        }

        public Color ToastTitle2Color
        {
            get
            {
                return ColorHelper.HexToColor(this.Settings.ToastTitle2Color);
            }
            set
            {
                this.Settings.ToastTitle2Color = ColorHelper.ColorToHex(value);
                this.NotifyPropertyChanged();
            }
        }

        public Color SongProgressBarBackgroundColor
        {
            get
            {
                return ColorHelper.HexToColor(this.Settings.SongProgressBarBackgroundColor);
            }
            set
            {
                this.Settings.SongProgressBarBackgroundColor = ColorHelper.ColorToHex(value);
                this.NotifyPropertyChanged();
            }
        }

        public Color SongProgressBarForegroundColor
        {
            get
            {
                return ColorHelper.HexToColor(this.Settings.SongProgressBarForegroundColor);
            }
            set
            {
                this.Settings.SongProgressBarForegroundColor = ColorHelper.ColorToHex(value);
                this.NotifyPropertyChanged();
            }
        }

        #endregion Toast

        public SecureString ProxyPassword { private get; set; }

        #region Commands

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand DefaultCommand { get; }

        public DelegateCommand DefaultAllCommand { get; }

        public DelegateCommand SelectFileForSavingTrackCommand { get; }

        #endregion Commands

        /// <summary>
        /// Occurs when after a Save command.
        /// </summary>
        public event EventHandler<SettingsSavedEventArgs> SettingsSaved;

        public SettingsViewModel()
        {
            this.Settings = Settings.Temporary;

            this.SaveCommand = new DelegateCommand(this.Save);
            this.DefaultCommand = new DelegateCommand(this.Default);
            this.DefaultAllCommand = new DelegateCommand(this.DefaultAll);
            this.SelectFileForSavingTrackCommand = new DelegateCommand(this.SelectFileForSavingTrack);

            this.Settings.PropertyChanged += this.Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.Settings.ToastColorTop):
                    this.NotifyPropertyChanged(nameof(this.ToastColorTop));
                    break;

                case nameof(this.Settings.ToastColorBottom):
                    this.NotifyPropertyChanged(nameof(this.ToastColorBottom));
                    break;

                case nameof(this.Settings.ToastBorderColor):
                    this.NotifyPropertyChanged(nameof(this.ToastBorderColor));
                    break;

                case nameof(this.Settings.ToastTitle1Color):
                    this.NotifyPropertyChanged(nameof(this.ToastTitle1Color));
                    break;

                case nameof(this.Settings.ToastTitle2Color):
                    this.NotifyPropertyChanged(nameof(this.ToastTitle2Color));
                    break;

                case nameof(this.Settings.SongProgressBarBackgroundColor):
                    this.NotifyPropertyChanged(nameof(this.SongProgressBarBackgroundColor));
                    break;

                case nameof(this.Settings.SongProgressBarForegroundColor):
                    this.NotifyPropertyChanged(nameof(this.SongProgressBarForegroundColor));
                    break;

                default:
                    break;
            }
        }

        private void Save()
        {
            if (this.Settings.UseProxy)
            {
                // Proxy Password
                if (string.IsNullOrEmpty(this.Settings.ProxyConfig.Username))
                {
                    // If no username has been entered, remove the saved password
                    Security.SaveProxyPassword(new SecureString());
                }
                else if (!string.IsNullOrEmpty(this.ProxyPassword?.ToPlainString()))
                {
                    // Otherwise, if the password box is not empty, save the new password
                    Security.SaveProxyPassword(this.ProxyPassword);
                }
                else
                {
                    // If the username field is not empty and the password's is,
                    // then keep the previous password.
                }
            }

            this.Settings.SetAsCurrentAndSave();

            // Get a new clone of the current settings,
            // since Current and the original Temporary are now the same instance;
            this.Settings = Settings.Temporary;

            this.SettingsSaved?.Invoke(this, new SettingsSavedEventArgs(this.Settings));
        }

        private void Default()
        {
            switch (this.CurrentTabIndex)
            {
                case 0:
                    this.Settings.SetDefaultGeneral();
                    break;

                case 1:
                    this.Settings.SetDefaultHotkeys();
                    break;

                case 2:
                    switch (this.CurrentToastTabIndex)
                    {
                        case 0:
                            this.Settings.SetDefaultToastGeneral();
                            break;

                        case 1:
                            this.Settings.SetDefaultToastColors();
                            break;

                        default:
                            throw new InvalidOperationException($"Unexpected value for CurrentToastTabIndex = {this.CurrentToastTabIndex}");
                    }
                    break;

                case 3:
                    this.Settings.SetDefaultAdvanced();
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected value for CurrentTabIndex = {this.CurrentTabIndex}");
            }
        }

        private void DefaultAll()
        {
            this.Settings.SetDefault();
        }

        private void SelectFileForSavingTrack()
        {
            var dialog = new OpenFileDialog
            {
                FileName = this.Settings.SaveTrackToFilePath ?? string.Empty,
                CheckPathExists = true,
                CheckFileExists = false,
                ShowReadOnly = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                this.Settings.SaveTrackToFilePath = dialog.FileName;
        }
    }
}