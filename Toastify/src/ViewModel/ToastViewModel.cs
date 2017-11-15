using Toastify.Common;
using Toastify.Core;
using Toastify.Model;

namespace Toastify.ViewModel
{
    public class ToastViewModel : ObservableObject
    {
        private string _trackName;
        private string _artistName;

        private string _title1;
        private string _title2;
        private double _songProgressBarWidth;

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
            get
            {
                return this._trackName;
            }
            set
            {
                this._trackName = value;
                if (Settings.Instance.ToastTitlesOrder == ToastTitlesOrder.TrackByArtist)
                    this.Title1 = $"{(string.IsNullOrWhiteSpace(value) ? "???": value)}";
                else
                    this.Title2 = string.IsNullOrWhiteSpace(value) ? string.Empty : $"\x201C{value}\x201D";
            }
        }

        public string ArtistName
        {
            get
            {
                return this._artistName;
            }
            set
            {
                this._artistName = value;
                if (Settings.Instance.ToastTitlesOrder == ToastTitlesOrder.ArtistOfTrack)
                    this.Title1 = $"{(string.IsNullOrWhiteSpace(value) ? "???" : value)}:";
                else
                    this.Title2 = string.IsNullOrWhiteSpace(value) ? string.Empty : $"by {value}";
            }
        }

        public double SongProgressBarWidth
        {
            get { return this._songProgressBarWidth; }
            set { this.RaiseAndSetIfChanged(ref this._songProgressBarWidth, value); }
        }
    }
}