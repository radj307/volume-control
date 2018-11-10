using JetBrains.Annotations;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Cache;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Toastify.Common;
using Toastify.Core;
using Toastify.DI;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.Services;
using Toastify.ViewModel;
using ToastifyAPI.Helpers;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.Structs;
using ToastifyAPI.Plugins;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using PixelFormat = System.Windows.Media.PixelFormat;
using Point = System.Windows.Point;
using Spotify = Toastify.Core.Spotify;
using Timer = System.Timers.Timer;

namespace Toastify.View
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class ToastView : Window
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastView));

        private const string DEFAULT_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyToastifyLogo.png";
        private const string AD_PLAYING_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyAdPlaying.png";
        private const string ALBUM_ACCESS_DENIED_ICON = "pack://application:,,,/Toastify;component/Resources/ToastifyAccessDenied.png";
        private const string UPDATE_LOGO_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyToastifyUpdateLogo.png";

        private const string STATE_PAUSED = "Paused";
        private const string STATE_NOTHING_PLAYING = "Nothing's playing";

        internal static ToastView Current { get; private set; }

        #region Private fields

        private IntPtr _windowHandle;

        private readonly ToastViewModel toastViewModel;

        private Timer minimizeTimer;

        private SystemTray trayIcon;

        private Song currentSong;
        private BitmapSource cover;
        private string toastIconURI = "";

        private bool isUpdateToast;
        private string toastClickWhenUpdateUrl;
        private bool isPreviewForSettings;

        private bool dragging;
        private bool paused;

        #endregion Private fields

        private IntPtr WindowHandle
        {
            get
            {
                if (this._windowHandle == IntPtr.Zero)
                    this._windowHandle = this.GetHandle();
                return this._windowHandle;
            }
        }

        public Point Position { get { return this.Rect.Location; } }

        public System.Windows.Rect Rect { get { return new System.Windows.Rect(this.Left, this.Top, this.Width, this.Height); } }

        public bool IsInitComplete { get; private set; }

        public Settings Settings
        {
            get
            {
                return this.toastViewModel?.Settings;
            }
            private set
            {
                if (this.toastViewModel != null)
                    this.toastViewModel.Settings = value;
            }
        }

        public bool ShownOrFading { get; private set; }

        internal List<IPluginBase> Plugins { get; set; }

        [PropertyDependency]
        public IToastifyActionRegistry ActionRegistry { get; set; }

        #region Events

        public event EventHandler Started;

        public event EventHandler ToastClosing;

        #endregion Events

        public ToastView()
        {
            this.InitializeComponent();
            App.Container.BuildUp(this);

            this.toastViewModel = new ToastViewModel();
            this.toastViewModel.PropertyChanged += this.ToastViewModel_PropertyChanged;
            this.DataContext = this.toastViewModel;

            // Set a static reference back to ourselves, useful for callbacks.
            Current = this;
        }

        #region Initialization

        private void Init()
        {
            if (logger.IsDebugEnabled)
                logger.Debug($"Current Settings:{Environment.NewLine}{Settings.PrintSettings(2)}");

            this.InitToast();
            this.InitTrayIcon();
            this.StartSpotify();
        }

        private void FinalizeInit()
        {
            // Subscribe to Spotify's events (i.e. SpotifyLocalAPI's).
            Spotify.Instance.Exited -= this.Application_Shutdown;
            Spotify.Instance.Exited += this.Application_Shutdown;
            Spotify.Instance.SongChanged -= this.Spotify_SongChanged;
            Spotify.Instance.SongChanged += this.Spotify_SongChanged;
            Spotify.Instance.PlayStateChanged -= this.Spotify_PlayStateChanged;
            Spotify.Instance.PlayStateChanged += this.Spotify_PlayStateChanged;
            Spotify.Instance.TrackTimeChanged -= this.Spotify_TrackTimeChanged;
            Spotify.Instance.TrackTimeChanged += this.Spotify_TrackTimeChanged;

            // Subscribe to SettingsView's events
            SettingsView.SettingsLaunched -= this.SettingsView_Launched;
            SettingsView.SettingsLaunched += this.SettingsView_Launched;
            SettingsView.SettingsClosed -= this.SettingsView_Closed;
            SettingsView.SettingsClosed += this.SettingsView_Closed;
            SettingsView.SettingsSaved -= this.SettingsView_SettingsSaved;
            SettingsView.SettingsSaved += this.SettingsView_SettingsSaved;

            this.Deactivated -= this.Toast_Deactivated;
            this.Deactivated += this.Toast_Deactivated;

            this.LoadPlugins();
            this.Started?.Invoke(this, EventArgs.Empty);

            // Subscribe to AutoUpdater's events
            AutoUpdater.Instance.AutoUpdateFailed -= this.AutoUpdater_AutoUpdateFailed;
            AutoUpdater.Instance.AutoUpdateFailed += this.AutoUpdater_AutoUpdateFailed;

            // Subscribe to VersionChecker's events
            VersionChecker.Instance.CheckVersionComplete -= this.VersionChecker_CheckVersionComplete;
            VersionChecker.Instance.CheckVersionComplete += this.VersionChecker_CheckVersionComplete;

            // Subscribe to actions' events
            var playPauseAction = this.ActionRegistry.GetAction(ToastifyActionEnum.PlayPause);
            if (playPauseAction != null)
                playPauseAction.ActionPerformed += this.ActionPlayPause_ActionPerformed;

            this.IsInitComplete = true;
        }

        public void InitToast()
        {
            if (!this.isPreviewForSettings)
                this.SetToastVisibility(false);

            // [ POSITION & DIMENSIONS ]
            this.Left = this.Settings.PositionLeft;
            this.Top = this.Settings.PositionTop;
            this.Width = this.Settings.ToastWidth;
            this.Height = this.Settings.ToastHeight;

            // [ BORDERS ]
            this.ToastBorder.BorderThickness = new Thickness(this.Settings.ToastBorderThickness);
            this.ToastBorder.BorderBrush = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastBorderColor));
            this.ToastBorder.CornerRadius = new CornerRadius(
                this.Settings.ToastBorderCornerRadiusTopLeft,
                this.Settings.ToastBorderCornerRadiusTopRight,
                this.Settings.ToastBorderCornerRadiusBottomRight,
                this.Settings.ToastBorderCornerRadiusBottomLeft);

            // [ BACKGROUND COLOR ]
            Color top = ColorHelper.HexToColor(this.Settings.ToastColorTop);
            Color bottom = ColorHelper.HexToColor(this.Settings.ToastColorBottom);

            GradientStopCollection stops = new GradientStopCollection(2)
            {
                new GradientStop(top, this.Settings.ToastColorTopOffset),
                new GradientStop(bottom, this.Settings.ToastColorBottomOffset)
            };
            this.ToastBorder.Background = new LinearGradientBrush(stops, new Point(0, 0), new Point(0, 1));

            // [ TEXT COLOR ]
            this.Title1.Foreground = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastTitle1Color));
            this.Title2.Foreground = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastTitle2Color));
            this.Title1.Effect = new DropShadowEffect
            {
                ShadowDepth = this.Settings.ToastTitle1DropShadow ? (double)this.Settings.ToastTitle1ShadowDepth : 0.0,
                BlurRadius = this.Settings.ToastTitle1DropShadow ? (double)this.Settings.ToastTitle1ShadowBlur : 0.0
            };
            this.Title2.Effect = new DropShadowEffect
            {
                ShadowDepth = this.Settings.ToastTitle2DropShadow ? (double)this.Settings.ToastTitle2ShadowDepth : 0.0,
                BlurRadius = this.Settings.ToastTitle2DropShadow ? (double)this.Settings.ToastTitle2ShadowBlur : 0.0
            };

            // [ TEXT FONT SIZE ]
            this.Title1.FontSize = this.Settings.ToastTitle1FontSize;
            this.Title2.FontSize = this.Settings.ToastTitle2FontSize;

            // [ SONG PROGRESS BAR ]
            //this.SongProgressBar.Visibility = this.Settings.ShowSongProgressBar ? Visibility.Visible : Visibility.Hidden;
            this.SongProgressBar.Visibility = Visibility.Hidden;
            this.SongProgressBarContainer.Background = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarBackgroundColor));
            this.SongProgressBarLine.Background = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarForegroundColor));
            this.SongProgressBarLineEllipse.Fill = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarForegroundColor));
        }

        private void InitTrayIcon()
        {
            this.trayIcon = new SystemTray(@"Toastify – Waiting for Spotify...", true)
            {
                AnimationStepMilliseconds = 75,
                Visible = true,
                ContextMenu = new ContextMenu()
            };

            object[] animationIcons = {
                Properties.Resources.toastify_loading_spotify_0,
                Properties.Resources.toastify_loading_spotify_0,
                Properties.Resources.toastify_loading_spotify_0,
                Properties.Resources.toastify_loading_spotify_1,
                Properties.Resources.toastify_loading_spotify_2,
                Properties.Resources.toastify_loading_spotify_3,
                Properties.Resources.toastify_loading_spotify_4,
                Properties.Resources.toastify_loading_spotify_5,
                Properties.Resources.toastify_loading_spotify_6,
                Properties.Resources.toastify_loading_spotify_7,
                Properties.Resources.toastify_loading_spotify_8,
                Properties.Resources.toastify_loading_spotify_9,
                Properties.Resources.toastify_loading_spotify_10,
                Properties.Resources.toastify_loading_spotify_11,
                Properties.Resources.toastify_loading_spotify_12,
                Properties.Resources.toastify_loading_spotify_13,
                Properties.Resources.toastify_loading_spotify_14,
                Properties.Resources.toastify_loading_spotify_15,
                Properties.Resources.toastify_loading_spotify_16,
                Properties.Resources.toastify_loading_spotify_17,
                Properties.Resources.toastify_loading_spotify_18,
                Properties.Resources.toastify_loading_spotify_19,
                Properties.Resources.toastify_loading_spotify_20,
                Properties.Resources.toastify_loading_spotify_21,
                Properties.Resources.toastify_loading_spotify_22,
                Properties.Resources.toastify_loading_spotify_23,
                Properties.Resources.toastify_loading_spotify_0,
                Properties.Resources.toastify_loading_spotify_0,
                Properties.Resources.toastify_loading_spotify_0
            };
            this.trayIcon.SetIconRange(animationIcons);
            this.trayIcon.StartAnimation();

            // Init tray icon menu
            MenuItem menuSettings = new MenuItem { Text = @"Settings", Enabled = false };
            menuSettings.Click += (s, ev) => { SettingsView.Launch(this); };

            MenuItem menuAbout = new MenuItem { Text = @"About Toastify" };
            menuAbout.Click += (s, ev) => { new AboutView().ShowDialog(); };

            MenuItem menuExit = new MenuItem { Text = @"Exit" };
            menuExit.Click += this.Application_Shutdown;

            this.trayIcon.ContextMenu.MenuItems.Add(menuSettings);
            this.trayIcon.ContextMenu.MenuItems.Add(menuAbout);
            this.trayIcon.ContextMenu.MenuItems.Add("-");
            this.trayIcon.ContextMenu.MenuItems.Add(menuExit);
        }

        private void FinalizeTrayIconInitialization()
        {
            this.trayIcon.StopAnimation();
            this.trayIcon.Animate = false;
            this.trayIcon.Text = @"Toastify";
            this.trayIcon.Icon = Properties.Resources.ToastifyIcon;
            this.trayIcon.ContextMenu.MenuItems[0].Enabled = true;
            this.trayIcon.DoubleClick += this.TrayIcon_DoubleClick;
        }

        private void StartSpotify()
        {
            Spotify.Instance.Connected -= this.Spotify_Connected;
            Spotify.Instance.Connected += this.Spotify_Connected;
            Spotify.Instance.StartSpotify();
        }

        private void LoadPlugins()
        {
            this.Plugins = new List<IPluginBase>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string applicationPath = new FileInfo(assembly.Location).DirectoryName;

            foreach (var p in this.Settings.Plugins)
            {
                try
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    string pluginFilePath = Path.Combine(applicationPath, p.FileName);
                    if (Activator.CreateInstanceFrom(pluginFilePath, p.TypeName).Unwrap() is IPluginBase plugin)
                    {
                        plugin.Init(p.Settings);
                        this.Plugins.Add(plugin);

                        this.Started += plugin.Started;
                        this.ToastClosing += plugin.Closing;
                        Spotify.Instance.SongChanged += (sender, e) => plugin.TrackChanged(sender, e);
                    }
                    else
                        Debug.WriteLine("'plugin' is not of type IPluginBase (?)");
                }
                catch (Exception e)
                {
                    // TODO: Handle plugins' errors.
                    logger.Error("Error while loading plugins.", e);
                    Analytics.TrackException(e);
                }
                Console.WriteLine(@"Loaded " + p.TypeName);
            }
        }

        #endregion Initialization

        #region Toast update

        /// <summary>
        /// Update current song's cover art url and toast's text.
        /// Also, save track info to file, if settings say so.
        /// </summary>
        /// <param name="song"> The song to set as current. </param>
        private void ChangeCurrentSong([CanBeNull] Song song)
        {
            if (logger.IsDebugEnabled)
                logger.Debug($"{nameof(this.ChangeCurrentSong)} has been called");

            this.currentSong = song;

            if (this.currentSong?.IsOtherTrackType() == true)
                this.FetchOtherTrackInfo();
            else
                this.UpdateToastText(this.currentSong);

            this.UpdateSongProgressBar(0.0);
            this.UpdateAlbumArt();

            // Save track info to file.
            if (this.Settings.SaveTrackToFile)
            {
                if (!string.IsNullOrEmpty(this.Settings.SaveTrackToFilePath))
                {
                    try
                    {
                        string trackText = this.currentSong.GetClipboardText(this.Settings.ClipboardTemplate);
                        File.WriteAllText(this.Settings.SaveTrackToFilePath, trackText);
                    }
                    catch (Exception e)
                    {
                        logger.Error("Error while saving track to file.", e);
                    }
                }
            }
        }

        #region UpdateAlbumArt

        private void UpdateAlbumArt(bool forceUpdate = false)
        {
            // TODO: Cache cover arts

            // Update cover art URL.
            string previousURI = this.toastIconURI;
            if (this.currentSong == null || !this.currentSong.IsValid())
                this.toastIconURI = DEFAULT_ICON;
            else
            {
                if (this.currentSong.IsAd())
                    this.currentSong.CoverArtUrl = AD_PLAYING_ICON;
                else if (string.IsNullOrWhiteSpace(this.currentSong?.CoverArtUrl) ||
                         !Uri.TryCreate(this.currentSong.CoverArtUrl, UriKind.RelativeOrAbsolute, out Uri _))
                    this.currentSong.CoverArtUrl = DEFAULT_ICON;

                this.toastIconURI = this.currentSong.CoverArtUrl;
            }

            // Update the cover art only if the url has changed.
            if (!string.IsNullOrEmpty(this.toastIconURI) && (forceUpdate || this.toastIconURI != previousURI))
                Task.Factory.StartNew(() => this.UpdateAlbumArt(this.toastIconURI));
        }

        private async Task UpdateAlbumArt(string albumArtUri)
        {
            this.toastIconURI = albumArtUri;
            Uri uri = new Uri(albumArtUri, UriKind.RelativeOrAbsolute);

            // If it's HTTP(S), download the album art using an HttpClient
            if (Regex.IsMatch(uri.Scheme, @"https?", RegexOptions.IgnoreCase))
            {
                HttpClientHandler httpClientHandler = Net.CreateHttpClientHandler(App.ProxyConfig);
                HttpClient http = new HttpClient(httpClientHandler);
                Stream stream = null;
                try
                {
                    stream = await GetAlbumArtAsStream(http, uri, async () => await this.UpdateAlbumArt(DEFAULT_ICON).ConfigureAwait(false))
                       .ConfigureAwait(false);
                    if (logger.IsDebugEnabled)
                        logger.Debug($"Album art downloaded: {albumArtUri}");

                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            byte[] bytebuffer = new byte[512];
                            int bytesRead = reader.Read(bytebuffer, 0, 512);

                            while (bytesRead > 0)
                            {
                                memoryStream.Write(bytebuffer, 0, bytesRead);
                                bytesRead = reader.Read(bytebuffer, 0, 512);
                            }

                            if (memoryStream.Length <= 0)
                            {
                                logger.Info($"Downloaded album art has size 0: {albumArtUri}");
                                this.Dispatcher.Invoke(DispatcherPriority.Render, (Action)(async () => await this.UpdateAlbumArt(DEFAULT_ICON).ConfigureAwait(false)));
                            }

                            this.Dispatcher.Invoke(DispatcherPriority.Render, (Action<MemoryStream>)this.UpdateAlbumArtFromMemoryStream, memoryStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Unhandled exception while updating the album art.", ex);
                }
                finally
                {
                    stream?.Close();
                    http.Dispose();
                    httpClientHandler.Dispose();
                }
            }
            else
            {
                // No need to download anything, this is most probably an internal resource
                await this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action<Uri>)this.UpdateAlbumArtFromUri, uri);
            }
        }

        private static async Task<Stream> GetAlbumArtAsStream([NotNull] HttpClient http, [NotNull] Uri uri, Action ifTooLong = null)
        {
            Task<Stream> downloader = http.GetStreamAsync(uri);

            CancellationTokenSource cts = null;
            Task awaiter = null;

            if (ifTooLong != null)
            {
                cts = new CancellationTokenSource();
                awaiter = Task.Factory.StartNew(() =>
                {
                    // ReSharper disable AccessToDisposedClosure
                    return Task.Delay(2000, cts.Token).ContinueWith(t => ifTooLong, cts.Token);
                    // ReSharper restore AccessToDisposedClosure
                }, cts.Token).Unwrap();
                await Task.WhenAny(awaiter, downloader).ConfigureAwait(false);
            }

            if (downloader.IsCompleted)
                cts?.Cancel();
            Stream stream = await downloader.ConfigureAwait(false);

            if (awaiter?.IsCompleted == false)
                awaiter.Dispose();
            cts?.Dispose();

            return stream;
        }

        /// <summary>
        /// Updates the album art using the specified URI.
        /// <para />
        /// NOTE: This method needs to be called from the UI thread (see <see cref="Dispatcher.Invoke(Action)"/>).
        /// </summary>
        /// <param name="uri"></param>
        private void UpdateAlbumArtFromUri([NotNull] Uri uri)
        {
            BitmapImage bitmapImage = new BitmapImage();
            this.cover = bitmapImage;
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
            try
            {
                bitmapImage.UriSource = uri;
            }
            catch (UriFormatException e)
            {
                logger.Error($"UriFormatException with URI=[{uri}]", e);
                bitmapImage.UriSource = new Uri(ALBUM_ACCESS_DENIED_ICON, UriKind.RelativeOrAbsolute);
            }
            finally
            {
                bitmapImage.EndInit();
                this.AlbumArt.Source = this.cover;
            }
        }

        /// <summary>
        /// Updates the album art using the specified memory stream.
        /// <para />
        /// NOTE #1: This method needs to be called from the UI thread (see <see cref="Dispatcher.Invoke(Action)"/>).
        /// <para />
        /// NOTE #2: The specified <see cref="MemoryStream"/> will be closed.
        /// </summary>
        /// <param name="memoryStream"></param>
        private void UpdateAlbumArtFromMemoryStream([NotNull] MemoryStream memoryStream)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                this.cover = bitmapImage;
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                try
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    bitmapImage.StreamSource = memoryStream;
                }
                catch (Exception e)
                {
                    logger.Error("Error while setting the stream source of the album art.", e);
                    bitmapImage.UriSource = new Uri(ALBUM_ACCESS_DENIED_ICON, UriKind.RelativeOrAbsolute);
                }
                finally
                {
                    bitmapImage.EndInit();
                    this.AlbumArt.Source = this.cover;

                    memoryStream.Close();
                }
            }
            catch (Exception e)
            {
                // Try to create a BitmapFrame from the MemoryStream.
                logger.Warn($"Unhandled exception while updating the album art from memory stream ({memoryStream.Length} bytes)", e);
                memoryStream.Seek(0, SeekOrigin.Begin);

                try
                {
                    this.cover = BitmapFrame.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    this.AlbumArt.Source = this.cover;
                }
                catch (Exception ee)
                {
                    logger.Error("Unhandled exception while updating the album art from a BitmapFrame", ee);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        // Last try: create a Bitmap from the MemoryStream and convert that to a BitmapSource
                        using (var bitmap = new Bitmap(memoryStream))
                        {
                            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                            PixelFormat? pixelFormat = bitmap.PixelFormat.ConvertToWpfPixelFormat();
                            if (!pixelFormat.HasValue)
                            {
                                logger.Error($"Unsupported bitmap pixel format: {bitmap.PixelFormat}");
                                this.Dispatcher.Invoke(DispatcherPriority.Render, (Action)(async () => await this.UpdateAlbumArt(DEFAULT_ICON).ConfigureAwait(false)));
                            }
                            else
                            {
                                BitmapSource bitmapSource = BitmapSource.Create(bitmapData.Width, bitmapData.Height,
                                    bitmap.HorizontalResolution, bitmap.VerticalResolution,
                                    pixelFormat.Value, null, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

                                bitmap.UnlockBits(bitmapData);

                                this.cover = bitmapSource;
                                this.AlbumArt.Source = bitmapSource;
                            }
                        }
                    }
                    catch (Exception eee)
                    {
                        // Don't know what else to try... :(
                        logger.Error("Unhandled exception.", eee);
                        this.Dispatcher.Invoke(DispatcherPriority.Render, (Action)(async () => await this.UpdateAlbumArt(DEFAULT_ICON).ConfigureAwait(false)));
                    }
                }
            }
        }

        #endregion UpdateAlbumArt

        /// <summary>
        /// Update the toast's text using the songs' information.
        /// </summary>
        /// <param name="song"> The song. </param>
        /// <param name="altTitle1">
        ///   An alternative text to use as the first title. If not null, this causes the
        ///   song's information to be displayed on one line as the second title.
        /// </param>
        /// <param name="fadeIn"> Whether or not to start the toast fade-in animation. </param>
        private void UpdateToastText([CanBeNull] Song song, string altTitle1 = null, bool fadeIn = true)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (song == null || !song.IsValid())
                    this.UpdateToastText(STATE_NOTHING_PLAYING, fadeIn: false);
                else if (altTitle1 != null)
                    this.UpdateToastText(altTitle1, song.ToString(), fadeIn);
                else if (this.paused)
                    this.UpdateToastText(STATE_PAUSED, song.ToString(), fadeIn);
                else
                {
                    this.toastViewModel.TrackName = song.Track ?? string.Empty;
                    this.toastViewModel.ArtistName = song.Artist ?? string.Empty;

                    this.Title1.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
                    this.Title2.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();

                    if (fadeIn)
                        this.ShowOrHideToast(keepUp: true);

                    if (logger.IsDebugEnabled)
                        logger.Debug($"Toast text changed. New track: {song}");
                }
            }));
        }

        /// <summary>
        /// Update the toast's text using custom strings.
        /// </summary>
        /// <param name="title1"> First title. </param>
        /// <param name="title2">  Second title. </param>
        /// <param name="fadeIn"> Whether or not to start the toast fade-in animation. </param>
        /// <param name="forceShow"> Whether or not to force the toast to show up. </param>
        /// <param name="showPermanent"></param>
        private void UpdateToastText([NotNull] string title1, [NotNull] string title2 = "", bool fadeIn = true, bool forceShow = false, bool showPermanent = false)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                this.toastViewModel.Title1 = title1;
                this.toastViewModel.Title2 = title2;
                if (fadeIn)
                    this.ShowOrHideToast(force: forceShow, keepUp: true, permanent: showPermanent);
            }));
        }

        private void UpdateSongProgressBar(double trackTime)
        {
            double timePercentage = trackTime / this.currentSong?.Length ?? trackTime;
            this.toastViewModel.SongProgressBarWidth = this.SongProgressBarContainer.ActualWidth * timePercentage;
        }

        private void FetchOtherTrackInfo()
        {
            // TODO: Podcast? Try to fetch track info using Spotify Web API? (Podcast specific APIs are not yet available)
        }

        #endregion Toast update

        /// <summary>
        /// Display a temporary content (title and image) on the toast.
        /// </summary>
        /// <param name="title1"> The string on the first line. </param>
        /// <param name="title2"> The text on the second line. </param>
        /// <param name="artUri"> The URI of the image to display. </param>
        /// <param name="displayTime"> The display time in milliseconds. </param>
        public void DisplayFlashContent([NotNull] string title1, [NotNull] string title2, [NotNull] string artUri, double displayTime)
        {
            this.DisplayFlashContent(title1, title2, artUri, displayTime, null);
        }

        /// <summary>
        /// Display a temporary content (title and image) on the toast and then execute some action.
        /// </summary>
        /// <param name="title1"> The string on the first line. </param>
        /// <param name="title2"> The text on the second line. </param>
        /// <param name="artUri"> The URI of the image to display. </param>
        /// <param name="displayTime"> The display time in milliseconds. </param>
        /// <param name="callback"> A callback. </param>
        public void DisplayFlashContent([NotNull] string title1, [NotNull] string title2, [NotNull] string artUri, double displayTime, Action callback)
        {
            this.UpdateToastText(title1, title2, fadeIn: true, forceShow: true, showPermanent: true);
            Task.Factory.StartNew(async () =>
            {
                await this.UpdateAlbumArt(artUri).ConfigureAwait(false);
                Timer timer = new Timer(displayTime) { AutoReset = false };
                timer.Elapsed += (sender, args) =>
                {
                    if (this.minimizeTimer?.Enabled != true)
                    {
                        this.FadeOut(now: true);
                        Thread.Sleep(750);
                    }
                    this.ChangeCurrentSong(Spotify.Instance.CurrentSong);
                    callback?.Invoke();
                };
                timer.Start();
            });
        }

        private void DisplayNewUpdateToast()
        {
            this.isUpdateToast = true;
            this.DisplayFlashContent("New update available!", "(click here to open the download page)", UPDATE_LOGO_ICON, 5000, () => this.isUpdateToast = false);
        }

        private void SetToastVisibility(bool shallBeVisible)
        {
            this.ShownOrFading = shallBeVisible;
            this.Topmost = shallBeVisible;
            this.Visibility = shallBeVisible ? Visibility.Visible : Visibility.Collapsed;
            User32.ShowWindow(this.WindowHandle, shallBeVisible ? ShowWindowCmd.SW_RESTORE : ShowWindowCmd.SW_SHOWMINNOACTIVE);

            if (shallBeVisible)
            {
                this.Left = this.Settings.PositionLeft;
                this.Top = this.Settings.PositionTop;
                this.ResetPositionIfOffScreen();
            }
        }

        private void ShowToastPreview(bool show)
        {
            if (show)
                this.minimizeTimer?.Stop();

            this.isPreviewForSettings = !show;
            this.ShowOrHideToast(force: true, previewForSettings: true);
            this.isPreviewForSettings = show;

            if (!show)
                this.InitToast();
        }

        public void Toggle(bool force)
        {
            this.ShowOrHideToast(force: force);
        }

        /// <summary>
        /// Toggles the show state of the toast.
        /// </summary>
        /// <param name="force"> Whether to force the toast to fade-in or fade-out. </param>
        /// <param name="keepUp"> Whether to reset the fade-out timer or not. </param>
        /// <param name="previewForSettings"></param>
        /// <param name="permanent"></param>
        private void ShowOrHideToast(bool force = false, bool keepUp = false, bool previewForSettings = false, bool permanent = false)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
            {
                if (this.ShownOrFading && keepUp && this.minimizeTimer != null && !previewForSettings && !this.isPreviewForSettings)
                {
                    // Reset the timer's Interval so that the toast does not fade out while pressing the hotkeys.
                    this.BeginAnimation(OpacityProperty, null);
                    this.Opacity = 1.0;
                    this.minimizeTimer.Interval = Math.Max(this.Settings.DisplayTime, 16);

                    if (!permanent)
                        this.minimizeTimer.Start();
                }
                else if (this.ShownOrFading && !this.isPreviewForSettings)
                    this.FadeOut(now: force);
                else
                    this.FadeIn(force: force, permanent: permanent || previewForSettings);
            }));
        }

        private void FadeIn(bool force = false, bool permanent = false)
        {
            bool doNotFadeIn = this.ShownOrFading ||
                               this.dragging ||
                               this.isPreviewForSettings ||
                               (this.Settings.DisableToast || (this.Settings.OnlyShowToastOnHotkey && !force)) ||
                               (this.Settings.DisableToastWithFullscreenApps && Win32API.IsForegroundAppInFullscreen());
            if (doNotFadeIn)
                return;

            // Stop FadeOut
            this.minimizeTimer?.Stop();

            this.SetToastVisibility(true);

            DoubleAnimation anim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250));
            anim.Completed += (s, e) =>
            {
                if (!permanent)
                    this.FadeOut();
            };
            this.BeginAnimation(OpacityProperty, anim);
        }

        private void FadeOut(bool now = false)
        {
            if (!this.ShownOrFading || this.isPreviewForSettings)
                return;

            // 16 == one frame (0 is not a valid interval)
            var interval = now ? 16 : Math.Max(this.Settings.DisplayTime, 16);

            if (this.minimizeTimer == null)
            {
                this.minimizeTimer = new Timer { AutoReset = false };
                this.minimizeTimer.Elapsed += (s, e) =>
                {
                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            DoubleAnimation anim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(500));
                            anim.Completed += (ss, ee) =>
                            {
                                this.SetToastVisibility(false);
                                this.isUpdateToast = false;
                            };
                            this.BeginAnimation(OpacityProperty, anim);
                        }, DispatcherPriority.Render);
                    }
                    catch (TaskCanceledException ex)
                    {
                        logger.Warn("FadeOut animation canceled.", ex);
                        this.SetToastVisibility(false);
                        this.isUpdateToast = false;
                    }
                };
            }

            this.minimizeTimer.Interval = interval;

            this.minimizeTimer.Stop();
            this.minimizeTimer.Start();
        }

        private void ResetPositionIfOffScreen()
        {
            // Bring the Toast inside the working area if it is off-screen
            Vector offsetVector = ScreenHelper.BringRectInsideWorkingArea(this.Rect);
            this.Left += offsetVector.X;
            this.Top += offsetVector.Y;
        }

        internal void PrintInternalDebugInfo()
        {
            if (logger.IsDebugEnabled)
            {
                WindowPlacement wPlacement = new WindowPlacement();
                User32.GetWindowPlacement(this.WindowHandle, ref wPlacement);

                StringBuilder sb = new StringBuilder();
                List<string> strings = new List<string>();

                sb.Append($"Internal Info:{Environment.NewLine}");
                sb.Append($"\thWnd[{this.WindowHandle}]{Environment.NewLine}");

                // minimizeTimer
                strings.Clear();
                if (this.minimizeTimer == null)
                    strings.Add("null");
                else
                {
                    strings.Add(this.minimizeTimer.Enabled ? "enabled" : "disabled");
                    strings.Add($"{this.minimizeTimer.Interval}");
                }
                sb.Append($"\tminimizeTimer: {string.Join(",", strings)}{Environment.NewLine}");

                // track
                strings.Clear();
                if (this.currentSong == null)
                    strings.Add("null");
                else
                    strings.Add(this.currentSong.IsValid() ? "valid" : "invalid");
                sb.Append($"\ttrack: {string.Join(",", strings)}{Environment.NewLine}");

                // state
                strings.Clear();
                strings.Add(this.isUpdateToast ? "is-update" : string.Empty);
                strings.Add(this.isPreviewForSettings ? "is-preview" : string.Empty);
                strings.Add(this.dragging ? "dragging" : string.Empty);
                strings.Add(this.paused ? "paused" : "playing");
                strings.RemoveAll(string.IsNullOrWhiteSpace);
                sb.Append($"\tstate: {string.Join(",", strings)}{Environment.NewLine}");

                // visibility
                strings.Clear();
                strings.Add(this.ShownOrFading ? "shown-or-fading" : string.Empty);
                strings.Add(this.IsVisible ? "is-visible" : string.Empty);
                strings.Add($"{this.Visibility}");
                strings.Add(this.Topmost ? "topmost" : string.Empty);
                strings.Add($"{{{wPlacement}}}");
                strings.RemoveAll(string.IsNullOrWhiteSpace);
                sb.Append($"\tvisibility: {string.Join(",", strings)}{Environment.NewLine}");

                // dispatcher
                strings.Clear();
                if (this.Dispatcher == null)
                    strings.Add("null");
                else
                {
                    strings.Add(this.Dispatcher.HasShutdownStarted ? "shutdown-started" : string.Empty);
                    strings.RemoveAll(string.IsNullOrWhiteSpace);
                }
                sb.Append($"\tdispatcher: {string.Join(",", strings)}{Environment.NewLine}");

                // settings
                strings.Clear();
                strings.Add(this.Settings.DisableToast ? "toast-disabled" : "toast-enabled");
                strings.Add(this.Settings.OnlyShowToastOnHotkey ? "only-show-toast-on-hotkey" : string.Empty);
                strings.Add($"{this.Settings.DisplayTime}");
                strings.RemoveAll(string.IsNullOrWhiteSpace);
                sb.Append($"\tsettings: {string.Join(",", strings)}{Environment.NewLine}");

                string internalInfo = sb.ToString();
                logger.Debug($"{internalInfo}\tStack Trace:{Environment.NewLine}{Environment.StackTrace}");
            }
        }

        #region DisplayAction

#if DEBUG
        public bool LogShowToastAction { get; set; }
#endif

        public void DisplayAction(ToastifyActionEnum action)
        {
            if (!Spotify.Instance.IsRunning && action != ToastifyActionEnum.SettingsSaved)
            {
                this.toastIconURI = DEFAULT_ICON;
                this.UpdateToastText("Spotify not available!");
                return;
            }

            switch (action)
            {
                case ToastifyActionEnum.SettingsSaved:
                    this.DisplayFlashContent("Settings Saved", string.Empty, DEFAULT_ICON, 1500.0);
                    break;

                case ToastifyActionEnum.VolumeUp:
                case ToastifyActionEnum.VolumeDown:
                    this.UpdateToastText(this.currentSong, $"Volume {(action == ToastifyActionEnum.VolumeUp ? "++" : "--")}");
                    break;

                case ToastifyActionEnum.Mute:
                    this.UpdateToastText(this.currentSong, "Mute On/Off");
                    break;

                case ToastifyActionEnum.ShowToast:
#if DEBUG
                    if (this.LogShowToastAction)
                    {
                        WindowPlacement wPlacement = new WindowPlacement();
                        User32.GetWindowPlacement(this.WindowHandle, ref wPlacement);

                        string hWnd = $"hWnd[{this.WindowHandle}]";
                        string timer = $"timer[{(this.minimizeTimer == null ? "null" : $"{(this.minimizeTimer.Enabled ? '1' : '0')}, {this.minimizeTimer.Interval}")}]";
                        string song = $"track[{(this.currentSong == null ? "null" : $"{(this.currentSong.IsValid() ? '1' : '0')}")}]";
                        string state = $"state[{(this.isUpdateToast ? '1' : '0')}{(this.isPreviewForSettings ? '1' : '0')}{(this.dragging ? '1' : '0')}{(this.paused ? '1' : '0')}]";
                        string visibility = $"visibility[{(this.ShownOrFading ? '1' : '0')}{(this.IsVisible ? '1' : '0')}{this.Visibility.ToString().Substring(0, 1)}{(this.Topmost ? '1' : '0')}, {{{wPlacement}}}]";
                        string dispatcher = $"dispatcher[{(this.Dispatcher == null ? "null" : $"{(this.Dispatcher.HasShutdownStarted ? '1' : '0')}")}]";
                        string settings = $"settings[{(this.Settings.DisableToast ? '1' : '0')}{(this.Settings.OnlyShowToastOnHotkey ? '1' : '0')}, {this.Settings.DisplayTime}]";
                        logger.Info($"{hWnd}, {timer}, {song}, {state}, {visibility}, {dispatcher}, {settings}{Environment.NewLine}  Stack Trace:{Environment.NewLine}{Environment.StackTrace}");
                    }
#endif
                    this.ShowOrHideToast(force: true);
                    break;

                case ToastifyActionEnum.ThumbsUp:
                    this.toastIconURI = "pack://application:,,,/Toastify;component/Resources/thumbs_up.png";
                    this.UpdateToastText(this.currentSong, "Thumbs Up!");
                    break;

                case ToastifyActionEnum.ThumbsDown:
                    this.toastIconURI = "pack://application:,,,/Toastify;component/Resources/thumbs_down.png";
                    this.UpdateToastText(this.currentSong, "Thumbs Down!");
                    break;

                default:
                    // Ignore any other action
                    break;
            }
        }

        #endregion DisplayAction

        #region Event handlers [xaml]

        protected override void OnClosing(CancelEventArgs e)
        {
            // Close Spotify first.
            if (this.Settings.CloseSpotifyWithToastify && Spotify.Instance.IsRunning)
                Spotify.Instance.Kill();

            // Dispose the timer.
            this.minimizeTimer?.Stop();
            this.minimizeTimer?.Dispose();

            // Ensure trayicon is removed on exit.
            if (this.trayIcon != null)
            {
                this.trayIcon.Visible = false;
                this.trayIcon.Dispose();
                this.trayIcon = null;
            }

            this.ToastClosing?.Invoke(this, EventArgs.Empty);
            this.Plugins?.Clear();

            base.OnClosing(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();

            // Remove from ALT+TAB.
            Windows.AddToolWindowStyle(this.WindowHandle);
        }

        /// <summary>
        /// Mouse is over the window, halt any fade out animations and keep the toast active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            this.BeginAnimation(OpacityProperty, null);
            this.Opacity = 1.0;
            if (this.minimizeTimer != null)
            {
                this.minimizeTimer.Stop();
                this.minimizeTimer = null;
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            this.FadeOut();
        }

        private void Window_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                this.dragging = true;
                this.DragMove();
            }
        }

        private void Window_MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            if (this.dragging)
            {
                this.dragging = false;

                // Save the new window position
                this.Settings.PositionLeft = this.Left;
                this.Settings.PositionTop = this.Top;
                if (this.Settings == Settings.Current)
                    this.Settings.Save();
            }
            else
            {
                this.FadeOut(true);

                if (this.isUpdateToast)
                {
                    string url = string.IsNullOrWhiteSpace(this.toastClickWhenUpdateUrl) ? VersionChecker.GitHubReleasesUrl : this.toastClickWhenUpdateUrl;
                    ProcessStartInfo psi = new ProcessStartInfo(url) { UseShellExecute = true };
                    Process.Start(psi);
                }
                else
                    Spotify.Instance.SendAction(ToastifyActionEnum.ShowSpotify);
            }
        }

        #endregion Event handlers [xaml]

        #region Event handlers [Spotify]

        /// <summary>
        /// This event is received only once, at the start of the application.
        /// Here, we finalize the initialization of Toastify and set up the initial state of the toast based on the current state of Spotify.
        /// </summary>
        /// <param name="sender"> <see cref="Spotify"/>. </param>
        /// <param name="e"></param>
        private void Spotify_Connected(object sender, SpotifyStateEventArgs e)
        {
            // Finalize initialization
            this.FinalizeTrayIconInitialization();
            this.FinalizeInit();

            // Update current song
            this.paused = !e.Playing;
            this.ChangeCurrentSong(e.CurrentSong);
            this.UpdateSongProgressBar(e.TrackTime);
        }

        private void Spotify_SongChanged(object sender, SpotifyTrackChangedEventArgs e)
        {
            this.ChangeCurrentSong(e.NewSong);
        }

        private void Spotify_PlayStateChanged(object sender, SpotifyPlayStateChangedEventArgs e)
        {
            // Check if the toast is actually displaying something
            if (this.currentSong == null)
                this.ChangeCurrentSong(Spotify.Instance.CurrentSong);

            this.paused = !e.Playing;
            this.UpdateToastText(this.currentSong, fadeIn: false);
        }

        private void Spotify_TrackTimeChanged(object sender, SpotifyTrackTimeChangedEventArgs e)
        {
            this.UpdateSongProgressBar(e.TrackTime);
        }

        #endregion Event handlers [Spotify]

        #region Event handlers [actions]

        private void ActionPlayPause_ActionPerformed(object sender, EventArgs e)
        {
            // Try to fade-in if the play state change was triggered by a hotkey
            // (depends on current Settings, specifically on OnlyShowToastOnHotkey)
            this.ShowOrHideToast(keepUp: true);
        }

        #endregion

        #region Event handlers

        private void SettingsView_Launched(object sender, SettingsViewLaunchedEventArgs e)
        {
            this.Settings = e.Settings;
            this.ShowToastPreview(true);
        }

        private void SettingsView_Closed(object sender, EventArgs e)
        {
            this.Settings = Settings.Current;
            this.ShowToastPreview(false);
        }

        private void SettingsView_SettingsSaved(object sender, SettingsSavedEventArgs e)
        {
            this.Settings = e.Settings;
        }

        private void Application_Shutdown(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() => Application.Current.Shutdown()));
        }

        private void Toast_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

        private void TrayIcon_DoubleClick(object s, EventArgs ev)
        {
            SettingsView.Launch(this);
        }

        private void VersionChecker_CheckVersionComplete(object sender, CheckVersionCompleteEventArgs e)
        {
            if (Settings.Current.UpdateDeliveryMode != UpdateDeliveryMode.NotifyUpdate || !e.IsNew)
                return;

            this.toastClickWhenUpdateUrl = e.GitHubReleaseUrl;
            this.DisplayNewUpdateToast();
        }

        private void AutoUpdater_AutoUpdateFailed(object sender, CheckVersionCompleteEventArgs e)
        {
            // If the auto-updater fails, fallback to just show a notification toast
            this.toastClickWhenUpdateUrl = e.GitHubReleaseUrl;
            this.DisplayNewUpdateToast();
        }

        private void ToastViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (e.PropertyName)
                {
                    case nameof(this.Settings.ShowSongProgressBar):
                        //this.SongProgressBar.Visibility = this.Settings.ShowSongProgressBar ? Visibility.Visible : Visibility.Hidden;
                        this.SongProgressBar.Visibility = Visibility.Hidden;
                        break;

                    case nameof(this.Settings.ToastTitlesOrder):
                        this.toastViewModel.SetTrackName(this.toastViewModel.TrackName);
                        this.toastViewModel.SetArtistName(this.toastViewModel.ArtistName);
                        break;

                    case nameof(this.Settings.ToastColorTop):
                    case nameof(this.Settings.ToastColorBottom):
                    case nameof(this.Settings.ToastColorTopOffset):
                    case nameof(this.Settings.ToastColorBottomOffset):
                        Color top = ColorHelper.HexToColor(this.Settings.ToastColorTop);
                        Color bottom = ColorHelper.HexToColor(this.Settings.ToastColorBottom);

                        GradientStopCollection stops = new GradientStopCollection(2)
                        {
                            new GradientStop(top, this.Settings.ToastColorTopOffset),
                            new GradientStop(bottom, this.Settings.ToastColorBottomOffset)
                        };
                        this.ToastBorder.Background = new LinearGradientBrush(stops, new Point(0, 0), new Point(0, 1));
                        break;

                    case nameof(this.Settings.ToastBorderColor):
                        this.ToastBorder.BorderBrush = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastBorderColor));
                        break;

                    case nameof(this.Settings.ToastBorderThickness):
                        this.ToastBorder.BorderThickness = new Thickness(this.Settings.ToastBorderThickness);
                        break;

                    case nameof(this.Settings.ToastBorderCornerRadiusTopLeft):
                    case nameof(this.Settings.ToastBorderCornerRadiusTopRight):
                    case nameof(this.Settings.ToastBorderCornerRadiusBottomLeft):
                    case nameof(this.Settings.ToastBorderCornerRadiusBottomRight):
                        this.ToastBorder.CornerRadius = new CornerRadius(
                            this.Settings.ToastBorderCornerRadiusTopLeft,
                            this.Settings.ToastBorderCornerRadiusTopRight,
                            this.Settings.ToastBorderCornerRadiusBottomRight,
                            this.Settings.ToastBorderCornerRadiusBottomLeft);
                        break;

                    case nameof(this.Settings.ToastWidth):
                        this.Width = this.Settings.ToastWidth >= this.MinWidth ? (double)this.Settings.ToastWidth : this.MinWidth;
                        this.ResetPositionIfOffScreen();
                        break;

                    case nameof(this.Settings.ToastHeight):
                        this.Height = this.Settings.ToastHeight >= this.MinHeight ? (double)this.Settings.ToastHeight : this.MinHeight;
                        this.ResetPositionIfOffScreen();
                        break;

                    case nameof(this.Settings.PositionLeft):
                        this.Left = this.Settings.PositionLeft;
                        this.ResetPositionIfOffScreen();
                        break;

                    case nameof(this.Settings.PositionTop):
                        this.Top = this.Settings.PositionTop;
                        this.ResetPositionIfOffScreen();
                        break;

                    #region Title1

                    case nameof(this.Settings.ToastTitle1Color):
                        this.Title1.Foreground = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastTitle1Color));
                        break;

                    case nameof(this.Settings.ToastTitle1FontSize):
                        this.Title1.FontSize = this.Settings.ToastTitle1FontSize;
                        break;

                    case nameof(this.Settings.ToastTitle1DropShadow):
                        this.Title1.Effect = new DropShadowEffect
                        {
                            ShadowDepth = this.Settings.ToastTitle1DropShadow ? (double)this.Settings.ToastTitle1ShadowDepth : 0.0,
                            BlurRadius = this.Settings.ToastTitle1DropShadow ? (double)this.Settings.ToastTitle1ShadowBlur : 0.0
                        };
                        break;

                    case nameof(this.Settings.ToastTitle1ShadowDepth):
                        if (this.Title1.Effect is DropShadowEffect t1_effect_sd)
                            t1_effect_sd.ShadowDepth = this.Settings.ToastTitle1DropShadow ? (double)this.Settings.ToastTitle1ShadowDepth : 0.0;
                        break;

                    case nameof(this.Settings.ToastTitle1ShadowBlur):
                        if (this.Title1.Effect is DropShadowEffect t1_effect_sb)
                            t1_effect_sb.BlurRadius = this.Settings.ToastTitle1DropShadow ? (double)this.Settings.ToastTitle1ShadowBlur : 0.0;
                        break;

                    #endregion Title1

                    #region Title2

                    case nameof(this.Settings.ToastTitle2Color):
                        this.Title2.Foreground = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastTitle2Color));
                        break;

                    case nameof(this.Settings.ToastTitle2FontSize):
                        this.Title2.FontSize = this.Settings.ToastTitle2FontSize;
                        break;

                    case nameof(this.Settings.ToastTitle2DropShadow):
                        this.Title2.Effect = new DropShadowEffect
                        {
                            ShadowDepth = this.Settings.ToastTitle2DropShadow ? (double)this.Settings.ToastTitle2ShadowDepth : 0.0,
                            BlurRadius = this.Settings.ToastTitle2DropShadow ? (double)this.Settings.ToastTitle2ShadowBlur : 0.0
                        };
                        break;

                    case nameof(this.Settings.ToastTitle2ShadowDepth):
                        if (this.Title2.Effect is DropShadowEffect t2_effect_sd)
                            t2_effect_sd.ShadowDepth = this.Settings.ToastTitle2DropShadow ? (double)this.Settings.ToastTitle2ShadowDepth : 0.0;
                        break;

                    case nameof(this.Settings.ToastTitle2ShadowBlur):
                        if (this.Title2.Effect is DropShadowEffect t2_effect_sb)
                            t2_effect_sb.BlurRadius = this.Settings.ToastTitle2DropShadow ? (double)this.Settings.ToastTitle2ShadowBlur : 0.0;
                        break;

                    #endregion Title2

                    case nameof(this.Settings.SongProgressBarBackgroundColor):
                        this.SongProgressBarContainer.Background = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarBackgroundColor));
                        break;

                    case nameof(this.Settings.SongProgressBarForegroundColor):
                        this.SongProgressBarLine.Background = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarForegroundColor));
                        this.SongProgressBarLineEllipse.Fill = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarForegroundColor));
                        break;

                    default:
                        // Ignore any other property
                        break;
                }
            });
        }

        #endregion Event handlers
    }
}