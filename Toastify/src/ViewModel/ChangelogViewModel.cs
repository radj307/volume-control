using Toastify.Common;

namespace Toastify.ViewModel
{
    public class ChangelogViewModel : ObservableObject
    {
        private const string NO_RELEASE_BODY_MD = "### No relevant changes";
        private const string LOADING_MD = "#### Loading...";

        private string _releaseBodyMarkdown = LOADING_MD;

        public string ReleaseBodyMarkdown
        {
            get { return string.IsNullOrWhiteSpace(this._releaseBodyMarkdown) ? NO_RELEASE_BODY_MD : this._releaseBodyMarkdown; }
            set { this.RaiseAndSetIfChanged(ref this._releaseBodyMarkdown, value); }
        }
    }
}