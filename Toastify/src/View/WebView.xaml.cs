using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Toastify.View
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class WebView : Window
    {
        #region Non-Public Properties

        private bool IsBusy
        {
            get { return this.BusyIndicator.IsBusy; }
            set
            {
                this.BusyIndicator.Dispatcher.Invoke(() => this.BusyIndicator.IsBusy = value);
                this.WebBrowser.Dispatcher.Invoke(() => this.WebBrowser.Visibility = value ? Visibility.Hidden : Visibility.Visible);
            }
        }

        #endregion

        #region Public Properties

        public Dictionary<string, string> Headers { get; set; }

        public List<string> AllowedHosts { get; set; }

        #endregion

        public WebView() : this(null)
        {
        }

        public WebView(Dictionary<string, string> headers)
        {
            this.InitializeComponent();

            // Disable context menu
            if (this.WebBrowser.ContextMenu != null)
                this.WebBrowser.ContextMenu.IsEnabled = false;

            if (headers != null && headers.Count > 0)
            {
                this.Headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (var kvp in headers)
                {
                    if (kvp.Key != null)
                        this.Headers[kvp.Key] = kvp.Value;
                }
            }
        }

        public void SetSize(Size size)
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }

        public void NavigateTo(string url, Dictionary<string, string> headers = null)
        {
            Uri uri = new Uri(url);
            this.WebBrowser.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                this.WebBrowser.Navigate(uri, null, null, this.GetAdditionalHeaders(headers));
            }));
        }

        public void NavigateToRawHtml(string html)
        {
            this.WebBrowser.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                this.WebBrowser.NavigateToString(html);
            }));
        }

        private string GetAdditionalHeaders(Dictionary<string, string> additionalHeaders)
        {
            Dictionary<string, string> h = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (this.Headers != null && this.Headers.Count > 0)
            {
                foreach (var kvp in this.Headers)
                {
                    if (kvp.Key != null)
                        h[kvp.Key] = kvp.Value;
                }
            }

            if (additionalHeaders != null && additionalHeaders.Count > 0)
            {
                foreach (var kvp in additionalHeaders)
                {
                    if (kvp.Key != null)
                        h[kvp.Key] = kvp.Value;
                }
            }

            return h.Count > 0 ? string.Join(Environment.NewLine, h.Select(kvp => $"{kvp.Key}: {kvp.Value}")) : null;
        }

        private void WebView_OnClosing(object sender, EventArgs e)
        {
            try
            {
                this.BusyIndicator?.Dispatcher?.InvokeShutdown();
                this.WebBrowser?.Dispatcher?.InvokeShutdown();
                this.WebBrowser?.Dispose();
            }
            catch
            {
                // ignore
            }
        }

        private void WebBrowser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                this.WebBrowser.Dispatcher.Invoke(() =>
                {
                    // Disable context menu
                    this.WebBrowser.InvokeScript("eval", "window.document.oncontextmenu = function() { return false; }");

                    // Disable back/forward navigation
                    this.WebBrowser.InvokeScript("eval", "history.pushState(null, document.title, location.href);" +
                                                         "window.addEventListener('popstate', function(event) {" +
                                                         "    history.pushState(null, document.title, location.href);" +
                                                         "});");
                }, DispatcherPriority.Normal);
            }
            catch
            {
                // ignore
            }
        }

        private void WebBrowser_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (this.AllowedHosts != null && this.AllowedHosts.Count > 0)
            {
                if (!this.AllowedHosts.Contains(e.Uri?.Host ?? string.Empty))
                    e.Cancel = true;
            }

            this.IsBusy = !e.Cancel;
        }

        private void WebBrowser_OnNavigated(object sender, NavigationEventArgs e)
        {
            this.IsBusy = false;
        }
    }
}