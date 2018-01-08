using System.ComponentModel;
using Toastify.Common;
using Toastify.Core;
using Toastify.Model;

namespace Toastify.ViewModel
{
    public class ToastViewModel : ObservableObject
    {
        #region Private fields

        private Settings _settings;

        private string _trackName;
        private string _artistName;

        private string _title1;
        private string _title2;
        private double _songProgressBarWidth;

        #endregion Private fields

        #region Public properties

        public Settings Settings
        {
            get { return this._settings; }
            set
            {
                if (this._settings != value)
                {
                    if (this._settings != null)
                        this._settings.PropertyChanged -= this.Settings_PropertyChanged;

                    this._settings = value;

                    if (this._settings != null)
                    {
                        this._settings.PropertyChanged -= this.Settings_PropertyChanged;
                        this._settings.PropertyChanged += this.Settings_PropertyChanged;
                    }

                    // Refresh Title1 and Title2
                    this.TrackName = this._trackName;
                    this.ArtistName = this._artistName;
                }
            }
        }

        public string Title1
        {
            get { return this._title1; }
            set { this.RaiseAndSetIfChanged(ref this._title1, value); }
        }

        public string Title2
        {
            get { return this._title2; }
            set { this.RaiseAndSetIfChanged(ref this._title2, value); }
        }

        public string TrackName
        {
            get { return this._trackName; }
            set { this.SetTrackName(value); }
        }

        public string ArtistName
        {
            get { return this._artistName; }
            set { this.SetArtistName(value); }
        }

        public double SongProgressBarWidth
        {
            get { return this._songProgressBarWidth; }
            set { this.RaiseAndSetIfChanged(ref this._songProgressBarWidth, value); }
        }

        #endregion Public properties

        public ToastViewModel()
        {
            this.Settings = Settings.Current;
        }

        public void SetTrackName(string trackName)
        {
            this._trackName = trackName;
            if (this.Settings.ToastTitlesOrder == ToastTitlesOrder.TrackByArtist)
                this.Title1 = $"{(string.IsNullOrWhiteSpace(trackName) ? "???" : trackName)}";
            else
                this.Title2 = string.IsNullOrWhiteSpace(trackName) ? string.Empty : $"\x201C{trackName}\x201D";
        }

        public void SetArtistName(string artistName)
        {
            this._artistName = artistName;
            if (this.Settings.ToastTitlesOrder == ToastTitlesOrder.ArtistOfTrack)
                this.Title1 = $"{(string.IsNullOrWhiteSpace(artistName) ? "???" : artistName)}:";
            else
                this.Title2 = string.IsNullOrWhiteSpace(artistName) ? string.Empty : $"by {artistName}";
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.Settings.ShowSongProgressBar):
                case nameof(this.Settings.ToastTitlesOrder):
                case nameof(this.Settings.ToastColorTop):
                case nameof(this.Settings.ToastColorBottom):
                case nameof(this.Settings.ToastColorTopOffset):
                case nameof(this.Settings.ToastColorBottomOffset):
                case nameof(this.Settings.ToastBorderColor):
                case nameof(this.Settings.ToastBorderThickness):
                case nameof(this.Settings.ToastBorderCornerRadiusTopLeft):
                case nameof(this.Settings.ToastBorderCornerRadiusTopRight):
                case nameof(this.Settings.ToastBorderCornerRadiusBottomLeft):
                case nameof(this.Settings.ToastBorderCornerRadiusBottomRight):
                case nameof(this.Settings.ToastWidth):
                case nameof(this.Settings.ToastHeight):
                case nameof(this.Settings.PositionLeft):
                case nameof(this.Settings.PositionTop):
                case nameof(this.Settings.ToastTitle1Color):
                case nameof(this.Settings.ToastTitle1FontSize):
                case nameof(this.Settings.ToastTitle1DropShadow):
                case nameof(this.Settings.ToastTitle1ShadowDepth):
                case nameof(this.Settings.ToastTitle1ShadowBlur):
                case nameof(this.Settings.ToastTitle2Color):
                case nameof(this.Settings.ToastTitle2FontSize):
                case nameof(this.Settings.ToastTitle2DropShadow):
                case nameof(this.Settings.ToastTitle2ShadowDepth):
                case nameof(this.Settings.ToastTitle2ShadowBlur):
                case nameof(this.Settings.SongProgressBarBackgroundColor):
                case nameof(this.Settings.SongProgressBarForegroundColor):
                    this.NotifyPropertyChanged(e.PropertyName);
                    break;
            }
        }
    }
}