using System.Windows;
using Toastify.Common;

namespace Toastify.ViewModel
{
    public class ToastViewModel : ObservableObject
    {
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

        public Visibility SongProgressBarVisibility
        {
            get { return Visibility.Hidden; /*Settings.Instance.ShowSongProgressBar ? SongProgressBarVisibility.Visible : SongProgressBarVisibility.Hidden;*/ }
        }

        public double SongProgressBarWidth
        {
            get { return this._songProgressBarWidth; }
            set { this.RaiseAndSetIfChanged(ref this._songProgressBarWidth, value); }
        }
    }
}