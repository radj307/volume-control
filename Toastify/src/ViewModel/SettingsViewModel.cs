using System;
using System.Windows.Forms;
using System.Windows.Media;
using Toastify.Common;
using Toastify.Helpers;
using Toastify.Model;

namespace Toastify.ViewModel
{
    public class SettingsViewModel : ObservableObject
    {
        public Settings Settings { get; }

        public int CurrentTabIndex { get; set; }

        public int CurrentToastTabIndex { get; set; }

        #region Toast

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

        #region Commands

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand DefaultCommand { get; }

        public DelegateCommand DefaultAllCommand { get; }

        public DelegateCommand SelectFileForSavingTrackCommand { get; }

        #endregion Commands

        /// <summary>
        /// Occurs when after a Save command.
        /// </summary>
        public event EventHandler SettingsSaved;

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
            }
        }

        private void Save()
        {
            this.Settings.SetAsCurrentAndSave();
            this.SettingsSaved?.Invoke(this, EventArgs.Empty);
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
                    }
                    break;
            }
        }

        private void DefaultAll()
        {
            this.Settings.Default();
        }

        private void SelectFileForSavingTrack()
        {
            var dialog = new OpenFileDialog()
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