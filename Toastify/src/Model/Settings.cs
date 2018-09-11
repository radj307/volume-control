using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using JetBrains.Annotations;
using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using Toastify.Common;
using Toastify.Core;
using Toastify.DI;
using Toastify.Events;
using Toastify.Helpers;
using Application = System.Windows.Forms.Application;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace Toastify.Model
{
    [Serializable]
    [XmlRoot("SettingsXml")]
    [JsonObject(MemberSerialization.OptOut)]
    public class Settings : ObservableObject
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Settings));

        private const string REG_KEY_STARTUP = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string SETTINGS_FILENAME = "Toastify.cfg";

        #region Static Fields and Properties

        private static readonly Regex regex4ChannelsColor = new Regex("^#[0-9A-F]{8}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string _settingsFilePath;

        public static JsonSerializerSettings JsonSerializerSettings { get; }

        public static JsonSerializer JsonSerializer { get; }

        /// <summary>
        ///     Returns the location of the settings file
        /// </summary>
        public static string SettingsFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_settingsFilePath))
                {
                    string settingsPath = App.ApplicationData;

                    if (!Directory.Exists(settingsPath))
                    {
                        try
                        {
                            Directory.CreateDirectory(settingsPath);
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"Error creating user settings directory (\"{settingsPath}\")", ex);

                            // No messagebox as this should not happen (and there will be a MessageBox later on when
                            // settings fail to load)
                        }
                    }

                    _settingsFilePath = Path.Combine(settingsPath, SETTINGS_FILENAME);
                }

                return _settingsFilePath;
            }
        }

        #endregion

        #region Public Properties

        // TODO: When the Settings class will not be a Singleton anymore, this dependency can be moved to the public constructor.
        [JsonIgnore]
        [PropertyDependency]
        public IToastifyActionRegistry ToastifyActionRegistry { get; set; }

        #endregion

        public static event EventHandler<CurrentSettingsChangedEventArgs> CurrentSettingsChanged;

        static Settings()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new JsonConverterContractResolver(),
                StringEscapeHandling = StringEscapeHandling.Default,
                FloatParseHandling = FloatParseHandling.Decimal,
                FloatFormatHandling = FloatFormatHandling.String,
                DateParseHandling = DateParseHandling.DateTime,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                Formatting = Formatting.Indented,
                MaxDepth = null,
                Culture = CultureInfo.InvariantCulture,
                ConstructorHandling = ConstructorHandling.Default,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                MetadataPropertyHandling = MetadataPropertyHandling.Default,
                TypeNameHandling = TypeNameHandling.None,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = JsonSerializer_Error
            };

            JsonSerializer = JsonSerializer.Create(JsonSerializerSettings);
            JsonSerializer.Error += JsonSerializer_Error;
        }

        private Settings()
        {
        }

        ~Settings()
        {
            this.Unload();
        }

        /// <summary>
        ///     Save the current Settings instance to the file system.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     if this instance is not <see cref="Current" />. Call
        ///     <see cref="SetAsCurrentAndSave" />, instead.
        /// </exception>
        public void Save()
        {
            if (this != Current)
                throw new InvalidOperationException("Cannot save non-Current instance of Settings");

            this.SanitizeSettingsInstance();
            using (var sw = new StreamWriter(SettingsFilePath, false))
            {
                JsonSerializer.Serialize(sw, this);
            }

            // Save settings that were not serialized
            if (this._launchOnStartup.HasValue)
                this.LaunchOnStartup = this._launchOnStartup.Value;
        }

        /// <summary>
        ///     Loads the Settings instance from the file system onto the Current Settings.
        /// </summary>
        /// <exception cref="InvalidOperationException">if this instance is not <see cref="Current" />.</exception>
        /// <exception cref="FileNotFoundException">if the serialized settings file was not found.</exception>
        public void Load()
        {
            if (this != Current)
                throw new InvalidOperationException("Cannot load settings onto non-Current instance");

            // Load default values
            bool launchOnStartup = this.LaunchOnStartup;
            this.SetDefault(false);
            this.LaunchOnStartup = launchOnStartup;

            // Populate with saved values
            using (var sr = new StreamReader(SettingsFilePath))
            {
                JsonSerializer.Populate(sr, this);
            }

            this.CheckForNewSettings();
            this.SanitizeSettingsFile();
            this.Apply();
        }

        /// <summary>
        ///     Loads the Settings instance from the file system onto the Current Settings or use default settings.
        /// </summary>
        /// <exception cref="InvalidOperationException">if this instance is not <see cref="Current" />.</exception>
        public void LoadSafe()
        {
            try
            {
                this.Load();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                bool launchOnStartup = this.LaunchOnStartup;
                Current.SetDefault();
                this.LaunchOnStartup = launchOnStartup;

                Current.Save();
            }
        }

        /// <summary>
        ///     Saves this instance of Settings as Current to the file system.
        /// </summary>
        public void SetAsCurrentAndSave()
        {
            Current = this;
            this.Save();
        }

        /// <summary>
        ///     Helpful place to fix common issues with settings files
        /// </summary>
        private void SanitizeSettingsFile()
        {
            this.SanitizeSettingsInstance();
            this.Save();
        }

        /// <summary>
        ///     Helpful place to fix common issues with settings files
        /// </summary>
        private void SanitizeSettingsInstance()
        {
            // Remove duplicate hotkeys
            this._hotKeys = this._hotKeys?.DistinctAndSortByToastifyAction().ToList();

            // Bring the Toast inside the working area if it is off-screen
            var toastRect = new Rect(this.PositionLeft, this.PositionTop, this.ToastWidth, this.ToastHeight);
            Vector offsetVector = ScreenHelper.BringRectInsideWorkingArea(toastRect);
            this.PositionLeft += offsetVector.X;
            this.PositionTop += offsetVector.Y;

            // Validate WindowsVolumeMixerIncrement: must be positive!
            this.WindowsVolumeMixerIncrement = Math.Abs(this.WindowsVolumeMixerIncrement);

            // Validate StartupWaitTimeout: it cannot be negative!
            this.StartupWaitTimeout = Math.Abs(this.StartupWaitTimeout);
        }

        /// <summary>
        ///     Called when loading a settings file to iterate through new dynamic properties (such as Hotkeys)
        ///     which may have changed and would otherwise be hidden from the user
        /// </summary>
        private void CheckForNewSettings()
        {
            foreach (Hotkey defaultHotkey in this.DefaultHotkeys)
            {
                bool found = this.HotKeys.Any(hotkey => hotkey.Action?.Equals(defaultHotkey.Action) ?? defaultHotkey.Action == null);

                if (!found)
                    this.HotKeys.Add((Hotkey)defaultHotkey.Clone());
            }
        }

        /// <summary>
        ///     Any active settings (such as hotkeys) should be triggered here
        /// </summary>
        private void Apply()
        {
            if (!this.GlobalHotKeys || this.HotKeys == null)
                return;
            foreach (Hotkey hotkey in this.HotKeys)
            {
                hotkey.Activate();
            }
        }

        /// <summary>
        ///     Any active settings (such as hotkeys) should be unloaded here
        /// </summary>
        private void Unload()
        {
            if (this.HotKeys == null)
                return;
            foreach (Hotkey hotkey in this.HotKeys)
            {
                hotkey.Deactivate();
            }
        }

        public void ReActivateHotkeys()
        {
            if (this != Current)
                return;

            if (this.HotKeys != null)
            {
                foreach (Hotkey hotkey in this.HotKeys)
                {
                    hotkey.Deactivate();
                }

                if (this.GlobalHotKeys)
                {
                    foreach (Hotkey hotkey in this.HotKeys)
                    {
                        hotkey.Activate();
                    }
                }
            }
        }

        public Settings Clone()
        {
            var clone = this.MemberwiseClone() as Settings;

            if (clone != null)
            {
                App.Container.BuildUp(clone);

                // Hotkeys
                clone.HotKeys = new List<Hotkey>();
                foreach (Hotkey key in this.HotKeys)
                {
                    clone.HotKeys.Add((Hotkey)key.Clone());
                }

                // SettingValue<>'s
                IEnumerable<PropertyInfo> properties = typeof(Settings).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                       .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(ISettingValue)));
                foreach (PropertyInfo property in properties)
                {
                    property.SetValue(clone, null);

                    var value = (ISettingValue)property.GetValue(this);
                    property.SetValue(clone, value?.Clone());
                }

                // ProxyConfig
                clone._proxyConfig = (ProxyConfigAdapter)this._proxyConfig.Clone();

                clone.SanitizeSettingsInstance();
            }

            return clone;
        }

        /// <summary>
        ///     Get a <see cref="SettingValue{T}" />. Its value is first checked against its constraints and changed to its default
        ///     value if necessary.
        /// </summary>
        /// <typeparam name="T"> Type of setting. </typeparam>
        /// <param name="field"> A reference to the property's field. </param>
        /// <param name="callerPropertyName"> Ignore. Filled automatically at runtime. </param>
        /// <returns> Returns <paramref name="field" />. </returns>
        private SettingValue<T> GetSettingValue<T>(ref SettingValue<T> field, [CallerMemberName] string callerPropertyName = null)
            where T : IComparable, IConvertible
        {
            if (field != null && !field.CheckConstraintsSafe())
                this.SetSettingValue(ref field, field.Default, callerPropertyName);
            return field;
        }

        /// <summary>
        ///     Set the value of a <see cref="SettingValue{T}" /> if it has changed and notifies the change using the
        ///     <see cref="INotifyPropertyChanged" /> interface.
        /// </summary>
        /// <typeparam name="T"> Type of setting. </typeparam>
        /// <param name="field"> A reference to the property's field. </param>
        /// <param name="newValue"> The new value. </param>
        /// <param name="propertyName">
        ///     An optional property name to use in place of the automatically provided
        ///     <paramref name="callerPropertyName" />.
        /// </param>
        /// <param name="callerPropertyName"> Ignore. Filled automatically at runtime. </param>
        private void SetSettingValue<T>(ref SettingValue<T> field, SettingValue<T> newValue, [CanBeNull] string propertyName = null, [CallerMemberName] string callerPropertyName = null)
            where T : IComparable, IConvertible
        {
            if (field == null)
            {
                field = newValue;
                this.NotifyPropertyChanged(propertyName ?? callerPropertyName);
            }
            else if (newValue == null)
                field = null;
            else if (field.SetValueIfChanged(newValue))
                this.NotifyPropertyChanged(propertyName ?? callerPropertyName);
        }

        #region Static Members

        private static T DefaultValueOf<T>(SettingValue<T> _, [NotNull] string propertyName) where T : IComparable, IConvertible
        {
            PropertyInfo property = typeof(Settings).GetProperty(propertyName);
            if (property == null)
                return default(T);

            DefaultValueAttribute attribute = property.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault();
            if (attribute == null)
                return default(T);

            return attribute.Value is T @default ? @default : default(T);
        }

        private static T DefaultValueOf<T>(T _, [NotNull] string propertyName)
        {
            PropertyInfo property = typeof(Settings).GetProperty(propertyName);
            if (property == null)
                return default(T);

            DefaultValueAttribute attribute = property.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault();
            if (attribute == null)
                return default(T);

            return attribute.Value is T @default ? @default : default(T);
        }

        internal static string PrintSettings(int indentLevel = 0)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < indentLevel; ++i)
                sb.Append("    ");
            string indent = sb.ToString();

            sb.Clear();

            PropertyInfo[] properties = typeof(Settings).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                object current = property.GetValue(Current);
                if (property.PropertyType.GetInterfaces().Contains(typeof(ISettingValue)))
                    sb.Append($"{indent}{property.Name}: {current}{Environment.NewLine}");
                else
                {
                    if (property.PropertyType.GetInterfaces().Contains(typeof(ICollection)))
                        continue;

                    if (property.PropertyType == typeof(ProxyConfigAdapter))
                    {
                        var proxy = (ProxyConfigAdapter)current;
                        sb.Append($"{indent}{property.Name}: {proxy.ToString(true)}{Environment.NewLine}");
                    }
                }
            }

            return sb.ToString();
        }

        private static void JsonSerializer_Error(object sender, ErrorEventArgs errorEventArgs)
        {
            if (errorEventArgs.ErrorContext.Error.InnerException is ArgumentException)
            {
                // ReSharper disable once LocalNameCapturedOnly
                // ReSharper disable once RedundantAssignment
                if (errorEventArgs.CurrentObject is Hotkey hotkey)
                {
                    if (errorEventArgs.ErrorContext.Path.EndsWith(nameof(hotkey.Action)))
                        errorEventArgs.ErrorContext.Handled = true;
                }
            }

            if (!errorEventArgs.ErrorContext.Handled)
                logger.Error("JsonSerializer error.", errorEventArgs.ErrorContext.Error);
        }

        #endregion

        #region Settings instances

        private static Settings _current;
        private static Settings _default;

        /// <summary>
        ///     The currently applied settings. Whenever they get modified, the Settings file should be modified too.
        /// </summary>
        public static Settings Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new Settings();
                    App.Container.BuildUp(_current);
                }

                return _current;
            }
            private set
            {
                var previous = _current;

                _current?.Unload();
                _current = value;
                App.Container.BuildUp(_current);
                _current.Apply();

                CurrentSettingsChanged?.Invoke(typeof(Settings), new CurrentSettingsChangedEventArgs(previous, _current));
            }
        }

        /// <summary>
        ///     A temporary copy of the Current settings that can be modified without affecting the applied settings.
        /// </summary>
        public static Settings Temporary
        {
            get { return _current?.Clone(); }
        }

        public static Settings Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new Settings();
                    App.Container.BuildUp(_default);
                    _default?.SetDefault(false);
                    _default?.SanitizeSettingsInstance();
                }

                return _default;
            }
        }

        #endregion Settings instances

        #region Settings

        #region Private fields

        private bool? _launchOnStartup;
        private SettingValue<bool> _minimizeSpotifyOnStartup;
        private SettingValue<bool> _closeSpotifyWithToastify;
        private SettingValue<ToastifyVolumeControlMode> _volumeControlMode;
        private SettingValue<float> _windowsVolumeMixerIncrement;
        private SettingValue<VersionCheckFrequency> _versionCheckFrequency;
        private SettingValue<UpdateDeliveryMode> _updateDeliveryMode;
        private SettingValue<string> _clipboardTemplate;
        private SettingValue<bool> _saveTrackToFile;
        private SettingValue<string> _saveTrackToFilePath;
        private SettingValue<bool> _optInToAnalytics;

        private SettingValue<bool> _globalHotKeys;
        private List<Hotkey> _hotKeys;
        private IReadOnlyList<Hotkey> defaultHotKeys;

        private SettingValue<bool> _disableToast;
        private SettingValue<bool> _onlyShowToastOnHotkey;
        private SettingValue<bool> _disableToastWithFullscreenVideogames;
        private SettingValue<bool> _showSongProgressBar;
        private SettingValue<int> _displayTime;
        private SettingValue<ToastTitlesOrder> _toastTitlesOrder;
        private SettingValue<string> _toastColorTop;
        private SettingValue<string> _toastColorBottom;
        private SettingValue<double> _toastColorTopOffset;
        private SettingValue<double> _toastColorBottomOffset;
        private SettingValue<string> _toastBorderColor;
        private SettingValue<double> _toastBorderThickness;
        private SettingValue<double> _toastBorderCornerRadiusTopLeft;
        private SettingValue<double> _toastBorderCornerRadiusTopRight;
        private SettingValue<double> _toastBorderCornerRadiusBottomLeft;
        private SettingValue<double> _toastBorderCornerRadiusBottomRight;
        private SettingValue<double> _toastWidth;
        private SettingValue<double> _toastHeight;
        private SettingValue<double> _positionLeft;
        private SettingValue<double> _positionTop;
        private SettingValue<string> _toastTitle1Color;
        private SettingValue<string> _toastTitle2Color;
        private SettingValue<double> _toastTitle1FontSize;
        private SettingValue<double> _toastTitle2FontSize;
        private SettingValue<bool> _toastTitle1DropShadow;
        private SettingValue<double> _toastTitle1ShadowDepth;
        private SettingValue<double> _toastTitle1ShadowBlur;
        private SettingValue<bool> _toastTitle2DropShadow;
        private SettingValue<double> _toastTitle2ShadowDepth;
        private SettingValue<double> _toastTitle2ShadowBlur;
        private SettingValue<string> _songProgressBarBackgroundColor;
        private SettingValue<string> _songProgressBarForegroundColor;

        private SettingValue<bool> _useProxy;
        private ProxyConfigAdapter _proxyConfig;
        private SettingValue<bool> _enableBroadcaster;

        private SettingValue<DateTime> _lastVersionCheck;

        #endregion

        #region [General]

        // this is a dynamic setting depending on the existing registry key
        // (which allows for it to be set reliably from the installer) so make sure to not include it in the XML file
        [XmlIgnore]
        [JsonIgnore]
        public bool LaunchOnStartup
        {
            get
            {
                bool regValue;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REG_KEY_STARTUP, false))
                {
                    regValue = key?.GetValue("Toastify", null) != null;
                }

                if (this == Current)
                    return regValue;

                if (!this._launchOnStartup.HasValue)
                    this._launchOnStartup = regValue;
                return this._launchOnStartup.Value;
            }
            set
            {
                if (this == Current)
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REG_KEY_STARTUP, true))
                    {
                        if (value)
                            key?.SetValue("Toastify", $"\"{Application.ExecutablePath}\"");
                        else
                            key?.DeleteValue("Toastify", false);
                    }
                }
                else
                    this._launchOnStartup = value;

                this.NotifyPropertyChanged();
            }
        }

        [DefaultValue(false)]
        public SettingValue<bool> MinimizeSpotifyOnStartup
        {
            get { return this.GetSettingValue(ref this._minimizeSpotifyOnStartup); }
            set { this.SetSettingValue(ref this._minimizeSpotifyOnStartup, value); }
        }

        [DefaultValue(true)]
        public SettingValue<bool> CloseSpotifyWithToastify
        {
            get { return this.GetSettingValue(ref this._closeSpotifyWithToastify); }
            set { this.SetSettingValue(ref this._closeSpotifyWithToastify, value); }
        }

        [DefaultValue(ToastifyVolumeControlMode.SystemSpotifyOnly)]
        public SettingValue<ToastifyVolumeControlMode> VolumeControlMode
        {
            get { return this.GetSettingValue(ref this._volumeControlMode); }
            set { this.SetSettingValue(ref this._volumeControlMode, value); }
        }

        [DefaultValue(2.0f)]
        public SettingValue<float> WindowsVolumeMixerIncrement
        {
            get { return this.GetSettingValue(ref this._windowsVolumeMixerIncrement); }
            set { this.SetSettingValue(ref this._windowsVolumeMixerIncrement, value); }
        }

        [DefaultValue(Core.VersionCheckFrequency.EveryDay)]
        public SettingValue<VersionCheckFrequency> VersionCheckFrequency
        {
            get { return this.GetSettingValue(ref this._versionCheckFrequency); }
            set { this.SetSettingValue(ref this._versionCheckFrequency, value); }
        }

        [DefaultValue(Core.UpdateDeliveryMode.NotifyUpdate)]
        public SettingValue<UpdateDeliveryMode> UpdateDeliveryMode
        {
            get { return this.GetSettingValue(ref this._updateDeliveryMode); }
            set { this.SetSettingValue(ref this._updateDeliveryMode, value); }
        }

        [DefaultValue("I'm currently listening to {0}")]
        public SettingValue<string> ClipboardTemplate
        {
            get { return this.GetSettingValue(ref this._clipboardTemplate); }
            set { this.SetSettingValue(ref this._clipboardTemplate, value); }
        }

        [DefaultValue(false)]
        public SettingValue<bool> SaveTrackToFile
        {
            get { return this.GetSettingValue(ref this._saveTrackToFile); }
            set { this.SetSettingValue(ref this._saveTrackToFile, value); }
        }

        public SettingValue<string> SaveTrackToFilePath
        {
            get { return this.GetSettingValue(ref this._saveTrackToFilePath); }
            set { this.SetSettingValue(ref this._saveTrackToFilePath, value); }
        }

        [DefaultValue(true)]
        public SettingValue<bool> OptInToAnalytics
        {
            get { return this.GetSettingValue(ref this._optInToAnalytics); }
            set { this.SetSettingValue(ref this._optInToAnalytics, value); }
        }

        #endregion

        #region [Hotkeys]

        [DefaultValue(true)]
        public SettingValue<bool> GlobalHotKeys
        {
            get { return this.GetSettingValue(ref this._globalHotKeys); }
            set { this.SetSettingValue(ref this._globalHotKeys, value); }
        }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<Hotkey> HotKeys
        {
            get { return this._hotKeys; }
            set { this.RaiseAndSetIfChanged(ref this._hotKeys, value); }
        }

        [JsonIgnore]
        public IReadOnlyList<Hotkey> DefaultHotkeys
        {
            get
            {
                IReadOnlyList<Hotkey> hotkeys = this.defaultHotKeys ?? (this.defaultHotKeys = new List<Hotkey>
                {
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Alt, Key = Key.Space, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.ShowToast), Enabled = true },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Alt, Key = Key.Down, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.Stop), Enabled = true },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Alt, Key = Key.Up, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.PlayPause), Enabled = true },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Alt, Key = Key.Right, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.NextTrack), Enabled = true },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Alt, Key = Key.Left, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.PreviousTrack), Enabled = true },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Alt, Key = Key.Add, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.VolumeUp), Enabled = false },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Alt, Key = Key.Subtract, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.VolumeDown), Enabled = false },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Alt, Key = Key.M, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.Mute), Enabled = false },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift, Key = Key.Right, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.FastForward), Enabled = false },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift, Key = Key.Left, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.Rewind), Enabled = false },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control, Key = Key.S, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.ShowSpotify), Enabled = false },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Shift, Key = Key.C, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.CopyTrackInfo), Enabled = false },
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control | ModifierKeys.Shift, Key = Key.V, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.PasteTrackInfo), Enabled = false },

#if DEBUG
                    new KeyboardHotkey { Modifiers = ModifierKeys.Control, Key = Key.D, Action = this.ToastifyActionRegistry.GetAction(ToastifyActionEnum.ShowDebugView), Enabled = true },
#endif
                });
                App.Container.BuildUpAll(hotkeys);
                return hotkeys;
            }
        }

        #endregion

        #region [Toast]

        [DefaultValue(false)]
        public SettingValue<bool> DisableToast
        {
            get { return this.GetSettingValue(ref this._disableToast); }
            set { this.SetSettingValue(ref this._disableToast, value); }
        }

        /// <summary>
        ///     Only show the toast when the `<see cref="ToastifyActionEnum.ShowToast" />` hotkey is pressed.
        /// </summary>
        [DefaultValue(true)]
        public SettingValue<bool> OnlyShowToastOnHotkey
        {
            get { return this.GetSettingValue(ref this._onlyShowToastOnHotkey); }
            set { this.SetSettingValue(ref this._onlyShowToastOnHotkey, value); }
        }

        [DefaultValue(true)]
        public SettingValue<bool> DisableToastWithFullscreenVideogames
        {
            get { return this.GetSettingValue(ref this._disableToastWithFullscreenVideogames); }
            set { this.SetSettingValue(ref this._disableToastWithFullscreenVideogames, value); }
        }

        [DefaultValue(true)]
        public SettingValue<bool> ShowSongProgressBar
        {
            get { return this.GetSettingValue(ref this._showSongProgressBar); }
            set { this.SetSettingValue(ref this._showSongProgressBar, value); }
        }

        [Obsolete("FadeOutTime is obsolete and will be removed in the future. Use DisplayTime instead.")]
        public SettingValue<int> FadeOutTime
        {
            get { return this.DisplayTime; }
            set { this.DisplayTime = value; }
        }

        [DefaultValue(4000)]
        public SettingValue<int> DisplayTime
        {
            get { return this.GetSettingValue(ref this._displayTime); }
            set { this.SetSettingValue(ref this._displayTime, value); }
        }

        [DefaultValue(Core.ToastTitlesOrder.TrackByArtist)]
        public SettingValue<ToastTitlesOrder> ToastTitlesOrder
        {
            get { return this.GetSettingValue(ref this._toastTitlesOrder); }
            set { this.SetSettingValue(ref this._toastTitlesOrder, value); }
        }

        [DefaultValue(300.0)]
        public SettingValue<double> ToastWidth
        {
            get { return this.GetSettingValue(ref this._toastWidth); }
            set { this.SetSettingValue(ref this._toastWidth, value); }
        }

        [DefaultValue(80.0)]
        public SettingValue<double> ToastHeight
        {
            get { return this.GetSettingValue(ref this._toastHeight); }
            set { this.SetSettingValue(ref this._toastHeight, value); }
        }

        public SettingValue<double> PositionLeft
        {
            get { return this.GetSettingValue(ref this._positionLeft); }
            set { this.SetSettingValue(ref this._positionLeft, value); }
        }

        public SettingValue<double> PositionTop
        {
            get { return this.GetSettingValue(ref this._positionTop); }
            set { this.SetSettingValue(ref this._positionTop, value); }
        }

        [DefaultValue(1.0)]
        public SettingValue<double> ToastBorderThickness
        {
            get { return this.GetSettingValue(ref this._toastBorderThickness); }
            set { this.SetSettingValue(ref this._toastBorderThickness, value); }
        }

        [DefaultValue(0.0)]
        public SettingValue<double> ToastBorderCornerRadiusTopLeft
        {
            get { return this.GetSettingValue(ref this._toastBorderCornerRadiusTopLeft); }
            set { this.SetSettingValue(ref this._toastBorderCornerRadiusTopLeft, value); }
        }

        [DefaultValue(0.0)]
        public SettingValue<double> ToastBorderCornerRadiusTopRight
        {
            get { return this.GetSettingValue(ref this._toastBorderCornerRadiusTopRight); }
            set { this.SetSettingValue(ref this._toastBorderCornerRadiusTopRight, value); }
        }

        [DefaultValue(0.0)]
        public SettingValue<double> ToastBorderCornerRadiusBottomLeft
        {
            get { return this.GetSettingValue(ref this._toastBorderCornerRadiusBottomLeft); }
            set { this.SetSettingValue(ref this._toastBorderCornerRadiusBottomLeft, value); }
        }

        [DefaultValue(0.0)]
        public SettingValue<double> ToastBorderCornerRadiusBottomRight
        {
            get { return this.GetSettingValue(ref this._toastBorderCornerRadiusBottomRight); }
            set { this.SetSettingValue(ref this._toastBorderCornerRadiusBottomRight, value); }
        }

        [DefaultValue("#FF000000")]
        public SettingValue<string> ToastColorTop
        {
            get { return this.GetSettingValue(ref this._toastColorTop); }
            set { this.SetSettingValue(ref this._toastColorTop, value); }
        }

        [DefaultValue("#FF000000")]
        public SettingValue<string> ToastColorBottom
        {
            get { return this.GetSettingValue(ref this._toastColorBottom); }
            set { this.SetSettingValue(ref this._toastColorBottom, value); }
        }

        [DefaultValue(0.0)]
        public SettingValue<double> ToastColorTopOffset
        {
            get { return this.GetSettingValue(ref this._toastColorTopOffset); }
            set { this.SetSettingValue(ref this._toastColorTopOffset, value); }
        }

        [DefaultValue(1.0)]
        public SettingValue<double> ToastColorBottomOffset
        {
            get { return this.GetSettingValue(ref this._toastColorBottomOffset); }
            set { this.SetSettingValue(ref this._toastColorBottomOffset, value); }
        }

        [DefaultValue("#FF000000")]
        public SettingValue<string> ToastBorderColor
        {
            get { return this._toastBorderColor; }
            set { this.SetSettingValue(ref this._toastBorderColor, value); }
        }

        [DefaultValue("#FFFFFFFF")]
        public SettingValue<string> ToastTitle1Color
        {
            get { return this.GetSettingValue(ref this._toastTitle1Color); }
            set { this.SetSettingValue(ref this._toastTitle1Color, value); }
        }

        [DefaultValue("#FFF0F0F0")]
        public SettingValue<string> ToastTitle2Color
        {
            get { return this.GetSettingValue(ref this._toastTitle2Color); }
            set { this.SetSettingValue(ref this._toastTitle2Color, value); }
        }

        [DefaultValue(16.0)]
        public SettingValue<double> ToastTitle1FontSize
        {
            get { return this.GetSettingValue(ref this._toastTitle1FontSize); }
            set { this.SetSettingValue(ref this._toastTitle1FontSize, value); }
        }

        [DefaultValue(12.0)]
        public SettingValue<double> ToastTitle2FontSize
        {
            get { return this.GetSettingValue(ref this._toastTitle2FontSize); }
            set { this.SetSettingValue(ref this._toastTitle2FontSize, value); }
        }

        [DefaultValue(true)]
        public SettingValue<bool> ToastTitle1DropShadow
        {
            get { return this.GetSettingValue(ref this._toastTitle1DropShadow); }
            set { this.SetSettingValue(ref this._toastTitle1DropShadow, value); }
        }

        [DefaultValue(3.0)]
        public SettingValue<double> ToastTitle1ShadowDepth
        {
            get { return this.GetSettingValue(ref this._toastTitle1ShadowDepth); }
            set { this.SetSettingValue(ref this._toastTitle1ShadowDepth, value); }
        }

        [DefaultValue(2.0)]
        public SettingValue<double> ToastTitle1ShadowBlur
        {
            get { return this.GetSettingValue(ref this._toastTitle1ShadowBlur); }
            set { this.SetSettingValue(ref this._toastTitle1ShadowBlur, value); }
        }

        [DefaultValue(false)]
        public SettingValue<bool> ToastTitle2DropShadow
        {
            get { return this.GetSettingValue(ref this._toastTitle2DropShadow); }
            set { this.SetSettingValue(ref this._toastTitle2DropShadow, value); }
        }

        [DefaultValue(2.0)]
        public SettingValue<double> ToastTitle2ShadowDepth
        {
            get { return this.GetSettingValue(ref this._toastTitle2ShadowDepth); }
            set { this.SetSettingValue(ref this._toastTitle2ShadowDepth, value); }
        }

        [DefaultValue(2.0)]
        public SettingValue<double> ToastTitle2ShadowBlur
        {
            get { return this.GetSettingValue(ref this._toastTitle2ShadowBlur); }
            set { this.SetSettingValue(ref this._toastTitle2ShadowBlur, value); }
        }

        [DefaultValue("#FF333333")]
        public SettingValue<string> SongProgressBarBackgroundColor
        {
            get { return this.GetSettingValue(ref this._songProgressBarBackgroundColor); }
            set { this.SetSettingValue(ref this._songProgressBarBackgroundColor, value); }
        }

        [DefaultValue("#FFA0A0A0")]
        public SettingValue<string> SongProgressBarForegroundColor
        {
            get { return this.GetSettingValue(ref this._songProgressBarForegroundColor); }
            set { this.SetSettingValue(ref this._songProgressBarForegroundColor, value); }
        }

        #endregion

        #region [Advanced]

        [DefaultValue(false)]
        public SettingValue<bool> UseProxy
        {
            get { return this.GetSettingValue(ref this._useProxy); }
            set { this.SetSettingValue(ref this._useProxy, value); }
        }

        [JsonConverter(typeof(SecureProxyConfigJsonConverter))]
        public ProxyConfigAdapter ProxyConfig
        {
            get
            {
                if (this._proxyConfig == null)
                    this._proxyConfig = new ProxyConfigAdapter();

                // Retrieve the encrypted password
                string plaintext = Security.GetSecureProxyPassword()?.ToPlainString();
                this._proxyConfig.Password = plaintext;
                return this._proxyConfig;
            }
            set
            {
                if (this._proxyConfig == null)
                    this._proxyConfig = value;
                else
                    this._proxyConfig.Set(value?.ProxyConfig);
                this.NotifyPropertyChanged();
            }
        }

        [DefaultValue(false)]
        public SettingValue<bool> EnableBroadcaster
        {
            get { return this.GetSettingValue(ref this._enableBroadcaster); }
            set { this.SetSettingValue(ref this._enableBroadcaster, value); }
        }

        #endregion

        #region (hidden)

        public bool FirstRun { get; set; }

        public string PreviousVersion { get; set; }

        public SettingValue<DateTime> LastVersionCheck
        {
            get { return this.GetSettingValue(ref this._lastVersionCheck); }
            set { this.SetSettingValue(ref this._lastVersionCheck, value); }
        }

        [DefaultValue(60000)]
        public int StartupWaitTimeout { get; set; }

        public WindowPosition SettingsWindowLastLocation { get; set; }

        public List<PluginDetails> Plugins { get; set; }

        #endregion

        #endregion

        #region SetDefault*

        public void SetDefault()
        {
            this.SetDefault(true);
        }

        public void SetDefault(bool activateHotkeys)
        {
            this.SetDefaultGeneral();
            this.SetDefaultHotkeys(activateHotkeys);
            this.SetDefaultToastGeneral();
            this.SetDefaultToastColors();
            this.SetDefaultAdvanced();

            // (hidden)
            this.StartupWaitTimeout = DefaultValueOf(this.StartupWaitTimeout, nameof(this.StartupWaitTimeout));
            this.Plugins = new List<PluginDetails>();

            // There are a few settings that we don't really want to override when
            // clearing settings (in fact these are more properties that we store
            // alongside settings for convenience), so don't reset them if they have values
            if (_current != null)
            {
                this.FirstRun = _current.FirstRun;
                this.PreviousVersion = _current.PreviousVersion;
                this.LastVersionCheck = new SettingValue<DateTime>(DateTime.Now);
            }
        }

        public void SetDefaultGeneral()
        {
            this.LaunchOnStartup = DefaultValueOf(this.LaunchOnStartup, nameof(this.LaunchOnStartup));
            this.MinimizeSpotifyOnStartup = DefaultValueOf(this.MinimizeSpotifyOnStartup, nameof(this.MinimizeSpotifyOnStartup));
            this.CloseSpotifyWithToastify = DefaultValueOf(this.CloseSpotifyWithToastify, nameof(this.CloseSpotifyWithToastify));
            this.VolumeControlMode = DefaultValueOf(this.VolumeControlMode, nameof(this.VolumeControlMode));
            this.WindowsVolumeMixerIncrement = new SettingValue<float>(DefaultValueOf(this.WindowsVolumeMixerIncrement, nameof(this.WindowsVolumeMixerIncrement)), new Range<float>(0.1f, 100.0f));

            this.VersionCheckFrequency = DefaultValueOf(this.VersionCheckFrequency, nameof(this.VersionCheckFrequency));
            this.UpdateDeliveryMode = DefaultValueOf(this.UpdateDeliveryMode, nameof(this.UpdateDeliveryMode));

            this.ClipboardTemplate = DefaultValueOf(this.ClipboardTemplate, nameof(this.ClipboardTemplate));
            this.SaveTrackToFile = DefaultValueOf(this.SaveTrackToFile, nameof(this.SaveTrackToFile));
            this.SaveTrackToFilePath = Path.Combine(App.LocalApplicationData, "current_song.txt");

            this.OptInToAnalytics = DefaultValueOf(this.SaveTrackToFile, nameof(this.SaveTrackToFile));
        }

        public void SetDefaultHotkeys()
        {
            this.SetDefaultHotkeys(true);
        }

        public void SetDefaultHotkeys(bool activateHotkeys)
        {
            this.GlobalHotKeys = DefaultValueOf(this.GlobalHotKeys, nameof(this.GlobalHotKeys));

            if (this.HotKeys != null)
            {
                foreach (Hotkey hotkey in this.HotKeys)
                {
                    hotkey.Deactivate();
                }
            }

            this.HotKeys = (List<Hotkey>)this.DefaultHotkeys.Clone();

            if (activateHotkeys && this == _current && this.HotKeys != null)
            {
                foreach (Hotkey hotkey in this.HotKeys)
                {
                    hotkey.Activate();
                }
            }
        }

        public void SetDefaultToastGeneral()
        {
            this.DisableToast = DefaultValueOf(this.DisableToast, nameof(this.DisableToast));

            this.OnlyShowToastOnHotkey = DefaultValueOf(this.OnlyShowToastOnHotkey, nameof(this.OnlyShowToastOnHotkey));
            this.DisableToastWithFullscreenVideogames = DefaultValueOf(this.DisableToastWithFullscreenVideogames, nameof(this.DisableToastWithFullscreenVideogames));
            this.ShowSongProgressBar = DefaultValueOf(this.ShowSongProgressBar, nameof(this.ShowSongProgressBar));
            this.DisplayTime = new SettingValue<int>(DefaultValueOf(this.DisplayTime, nameof(this.DisplayTime)), new Range<int>(100, int.MaxValue));
            this.ToastTitlesOrder = DefaultValueOf(this.ToastTitlesOrder, nameof(this.ToastTitlesOrder));

            this.ToastWidth = new SettingValue<double>(DefaultValueOf(this.ToastWidth, nameof(this.ToastWidth)), new Range<double>(0.0, double.MaxValue));
            this.ToastHeight = new SettingValue<double>(DefaultValueOf(this.ToastHeight, nameof(this.ToastHeight)), new Range<double>(0.0, double.MaxValue));

            this.PositionLeft = ScreenHelper.GetScreenRect().Right - this.ToastWidth;
            this.PositionTop = ScreenHelper.GetScreenRect().Bottom - this.ToastHeight - 5.0;

            this.ToastBorderThickness = new SettingValue<double>(DefaultValueOf(this.ToastBorderThickness, nameof(this.ToastBorderThickness)), new Range<double>(0.0, double.MaxValue));
            this.ToastBorderCornerRadiusTopLeft = new SettingValue<double>(DefaultValueOf(this.ToastBorderCornerRadiusTopLeft, nameof(this.ToastBorderCornerRadiusTopLeft)), new Range<double>(0.0, double.MaxValue));
            this.ToastBorderCornerRadiusTopRight = new SettingValue<double>(DefaultValueOf(this.ToastBorderCornerRadiusTopRight, nameof(this.ToastBorderCornerRadiusTopRight)), new Range<double>(0.0, double.MaxValue));
            this.ToastBorderCornerRadiusBottomRight = new SettingValue<double>(DefaultValueOf(this.ToastBorderCornerRadiusBottomRight, nameof(this.ToastBorderCornerRadiusBottomRight)), new Range<double>(0.0, double.MaxValue));
            this.ToastBorderCornerRadiusBottomLeft = new SettingValue<double>(DefaultValueOf(this.ToastBorderCornerRadiusBottomLeft, nameof(this.ToastBorderCornerRadiusBottomLeft)), new Range<double>(0.0, double.MaxValue));
        }

        public void SetDefaultToastColors()
        {
            this.ToastColorTop = new SettingValue<string>(DefaultValueOf(this.ToastColorTop, nameof(this.ToastColorTop)), s => regex4ChannelsColor.IsMatch(s));
            this.ToastColorBottom = new SettingValue<string>(DefaultValueOf(this.ToastColorBottom, nameof(this.ToastColorBottom)), s => regex4ChannelsColor.IsMatch(s));
            this.ToastColorTopOffset = new SettingValue<double>(DefaultValueOf(this.ToastColorTopOffset, nameof(this.ToastColorTopOffset)), new Range<double>(0.0, 1.0));
            this.ToastColorBottomOffset = new SettingValue<double>(DefaultValueOf(this.ToastColorBottomOffset, nameof(this.ToastColorBottomOffset)), new Range<double>(0.0, 1.0));
            this.ToastBorderColor = new SettingValue<string>(DefaultValueOf(this.ToastBorderColor, nameof(this.ToastBorderColor)), s => regex4ChannelsColor.IsMatch(s));

            this.ToastTitle1Color = new SettingValue<string>(DefaultValueOf(this.ToastTitle1Color, nameof(this.ToastTitle1Color)), s => regex4ChannelsColor.IsMatch(s));
            this.ToastTitle2Color = new SettingValue<string>(DefaultValueOf(this.ToastTitle2Color, nameof(this.ToastTitle2Color)), s => regex4ChannelsColor.IsMatch(s));
            this.ToastTitle1FontSize = new SettingValue<double>(DefaultValueOf(this.ToastTitle1FontSize, nameof(this.ToastTitle1FontSize)), new Range<double>(6.0, double.MaxValue));
            this.ToastTitle2FontSize = new SettingValue<double>(DefaultValueOf(this.ToastTitle2FontSize, nameof(this.ToastTitle2FontSize)), new Range<double>(6.0, double.MaxValue));
            this.ToastTitle1DropShadow = DefaultValueOf(this.ToastTitle1DropShadow, nameof(this.ToastTitle1DropShadow));
            this.ToastTitle1ShadowDepth = new SettingValue<double>(DefaultValueOf(this.ToastTitle1ShadowDepth, nameof(this.ToastTitle1ShadowDepth)), new Range<double>(0.0, double.MaxValue));
            this.ToastTitle1ShadowBlur = new SettingValue<double>(DefaultValueOf(this.ToastTitle1ShadowBlur, nameof(this.ToastTitle1ShadowBlur)), new Range<double>(0.0, double.MaxValue));
            this.ToastTitle2DropShadow = DefaultValueOf(this.ToastTitle2DropShadow, nameof(this.ToastTitle2DropShadow));
            this.ToastTitle2ShadowDepth = new SettingValue<double>(DefaultValueOf(this.ToastTitle2ShadowDepth, nameof(this.ToastTitle2ShadowDepth)), new Range<double>(0.0, double.MaxValue));
            this.ToastTitle2ShadowBlur = new SettingValue<double>(DefaultValueOf(this.ToastTitle2ShadowBlur, nameof(this.ToastTitle2ShadowBlur)), new Range<double>(0.0, double.MaxValue));

            this.SongProgressBarBackgroundColor = new SettingValue<string>(DefaultValueOf(this.SongProgressBarBackgroundColor, nameof(this.SongProgressBarBackgroundColor)), s => regex4ChannelsColor.IsMatch(s));
            this.SongProgressBarForegroundColor = new SettingValue<string>(DefaultValueOf(this.SongProgressBarForegroundColor, nameof(this.SongProgressBarForegroundColor)), s => regex4ChannelsColor.IsMatch(s));
        }

        public void SetDefaultAdvanced()
        {
            this.UseProxy = DefaultValueOf(this.UseProxy, nameof(this.UseProxy));
            this.ProxyConfig = new ProxyConfigAdapter();
            this.EnableBroadcaster = DefaultValueOf(this.EnableBroadcaster, nameof(this.EnableBroadcaster));
        }

        #endregion
    }

    [Serializable]
    public class PluginDetails
    {
        #region Public Properties

        public string FileName { get; set; }
        public string TypeName { get; set; }
        public string Settings { get; set; }

        #endregion
    }
}