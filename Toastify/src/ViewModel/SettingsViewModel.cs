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

        public DelegateCommand SelectFileForSavingTrackCommand { get; }

        #endregion Commands

        /// <summary>
        /// Occurs when after a Save command.
        /// </summary>
        public event EventHandler SettingsSaved;

        public SettingsViewModel()
        {
            this.Settings = Settings.Instance.Clone();

            this.SaveCommand = new DelegateCommand(this.Save);
            this.DefaultCommand = new DelegateCommand(this.Default);
            this.SelectFileForSavingTrackCommand = new DelegateCommand(this.SelectFileForSavingTrack);
        }

        private void Save()
        {
            this.Settings.Save(true);
            this.SettingsSaved?.Invoke(this, EventArgs.Empty);
        }

        private void Default()
        {
            this.Settings.Default();
            this.Save();
        }

        private void SelectFileForSavingTrack()
        {
            var dialog = new OpenFileDialog()
            {
                FileName = Settings.Instance.SaveTrackToFilePath ?? string.Empty,
                CheckPathExists = true,
                CheckFileExists = false,
                ShowReadOnly = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                this.Settings.SaveTrackToFilePath = dialog.FileName;
        }
    }
}