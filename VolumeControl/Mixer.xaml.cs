using CodingSeb.Localization;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Input;
using VolumeControl.Core.Input.Enums;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.SDK;
using VolumeControl.SDK.Internal;
using VolumeControl.ViewModels;
using VolumeControl.WPF;
using VolumeControl.WPF.Collections;
using VolumeControl.WPF.Controls;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for Mixer.xaml
    /// </summary>
    public partial class Mixer : Window
    {
        #region Setup
        public Mixer()
        {
            this.AllowsTransparency = Settings.AllowTransparency; //< this has to happen prior to initialization

            this.InitializeComponent();

            this.ShowInTaskbar = Settings.ShowInTaskbar;
            this.Topmost = Settings.AlwaysOnTop;

            _currentMultiInstanceState = Settings.AllowMultipleDistinctInstances;

            if (Settings.StartMinimized) Hide();

            // bind to VCAPI show event
            VCEvents.ShowMixer += (s, e) => Show();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            // Initialize secondary windows so they can run their own code to appear:

            // SessionListNotification
            _ = WindowHandleGetter.GetWindowHandle((this.FindResource("SessionListNotification") as SessionListNotification)!);
            // DeviceListNotification
            _ = WindowHandleGetter.GetWindowHandle((this.FindResource("DeviceListNotification") as DeviceListNotification)!);

            // Enable auto-saving of the config
            Settings.ResumeAutoSave(); //< doing this here prevents unnecessary write operations during window initialization

            _initialized = true;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
        }
        #endregion Setup

        #region Fields
        private readonly bool _currentMultiInstanceState;
        private bool _initialized = false;
        private bool _loaded = false;
        #endregion Fields

        #region Properties
        //private ListNotification ListNotification => (this.FindResource("Notification") as ListNotification)!;
        private VolumeControlVM VCSettings => (this.FindResource("Settings") as VolumeControlVM)!;
        private AudioDeviceManagerVM AudioAPI => this.VCSettings.AudioAPI;
        private HotkeyManagerVM HotkeyAPI => this.VCSettings.HotkeyAPI;
        private static LogWriter Log => FLog.Log;
        private static Config Settings => (AppConfig.Configuration.Default as Config)!;
        #endregion Properties

        #region Window Method Overrides
        protected override void OnRenderSizeChanged(SizeChangedInfo e)
        {
            base.OnRenderSizeChanged(e);

            // if the window hasn't loaded, or if relative positioning is disabled, don't do anything else.
            if (!_loaded || !Settings.KeepRelativePosition) return;

            // update the position of the window for the new size of the window
            switch (this.GetCurrentScreenCorner())
            {
            case EScreenCorner.TopLeft:
                break;
            case EScreenCorner.TopRight:
                if (!e.WidthChanged) return;

                this.Left += e.PreviousSize.Width - e.NewSize.Width;
                break;
            case EScreenCorner.BottomLeft:
                if (!e.HeightChanged) return;

                this.Top += e.PreviousSize.Height - e.NewSize.Height;
                break;
            case EScreenCorner.BottomRight:
                this.Left += e.PreviousSize.Width - e.NewSize.Width;
                this.Top += e.PreviousSize.Height - e.NewSize.Height;
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(Settings.SessionListNotificationConfig.PositionOriginCorner), (byte)Settings.SessionListNotificationConfig.PositionOriginCorner, typeof(EScreenCorner));
            }
        }
        #endregion Window Method Overrides

        #region EventHandlers
        /// <summary>Handles the Select process button's click event.</summary>
        private void Handle_ProcessSelectClick(object sender, RoutedEventArgs e)
        {
            if (MixerGrid.CurrentCell.Item is AudioSessionVM session)
            {
                VCAPI.Default.AudioSessionMultiSelector.CurrentSession = session.AudioSession;
            }
        }
        /// <summary>Handles the create new hotkey button's click event.</summary>
        private void Handle_CreateNewHotkeyClick(object sender, RoutedEventArgs e)
            => this.HotkeyAPI.HotkeyManager.AddHotkey(new HotkeyWithError(EFriendlyKey.None, EModifierKey.None, false));
        /// <summary>Handles the remove hotkey button's click event.</summary>
        private void Handle_hotkeyGridRemoveClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button hotkeyGridRemove && hotkeyGridRemove.CommandParameter is int id)
            {
                if (!this.HotkeyAPI.Hotkeys.Any(hk => hk.Hotkey.ID.Equals(id)))
                    return;

                if (Settings.DeleteHotkeyConfirmation)
                {
                    switch (MessageBox.Show(
                        Loc.Tr("VolumeControl.Dialogs.RemoveHotkey.Message",
                        $"Are you sure you want to delete hotkey {id}?\n" +
                        $"\n" +
                        $"- 'Yes'     Delete the hotkey.\n" +
                        $"- 'No'      Do not delete the hotkey.\n" +
                        $"- 'Cancel'  Don't show this again. (Press again to delete)\n")
                        .Replace("${ID}", id.ToString()),
                        Loc.Tr("VolumeControl.Dialogs.RemoveHotkey.Caption", "Confirm Delete"),
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question,
                        MessageBoxResult.No)
                        )
                    {
                    case MessageBoxResult.Yes:
                        break; // continue
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        Settings.DeleteHotkeyConfirmation = false;
                        Log.Info($"User specified option '{MessageBoxResult.Cancel:G}' in the remove hotkey confirmation dialog, indicating they want to disable the confirmation dialog; the '{nameof(Settings.DeleteHotkeyConfirmation)}' setting has been set to false.");
                        return;
                    }
                }

                Log.Debug($"User is removing hotkey {id} ({this.HotkeyAPI.HotkeyManager.GetHotkey(id)?.GetStringRepresentation()})");

                this.HotkeyAPI.HotkeyManager.RemoveHotkey(id);
            }
        }
        /// <inheritdoc cref="VolumeControlVM.ResetHotkeySettings"/>
        private void Handle_ResetHotkeysClick(object sender, RoutedEventArgs e)
        {
            this.VCSettings.ResetHotkeySettings();
        }
        private void Handle_BrowseForLogFilePathClick(object sender, RoutedEventArgs e)
        {
            string myDir = Path.GetDirectoryName(this.VCSettings.ExecutablePath) ?? string.Empty;
            if (Path.GetDirectoryName(this.VCSettings.LogFilePath) is not string initial || initial.Length == 0)
            {
                initial = myDir;
            }

            var sfd = new SaveFileDialog
            {
                OverwritePrompt = false,
                DefaultExt = "log",
                InitialDirectory = initial,
                Title = "Choose a location to save the log file.",
                FileName = VCSettings.LogFilePath
            };
            _ = sfd.ShowDialog(this);
            string path = Path.GetRelativePath(myDir, sfd.FileName);
            if (!path.Equals(logPath.Text, StringComparison.Ordinal))
            {
                logPath.Text = path;
            }
        }
        private void Handle_LogFilterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName?.Equals("Value", StringComparison.Ordinal) ?? false)
            {
                Settings.LogFilter = (this.FindResource("EventTypeOptions") as BindableEventType)!.Value;
            }
        }
        private void Handle_TargetNameBoxDoubleClick(object sender, MouseButtonEventArgs e)
        {
            targetbox.SelectionStart = 0;
            targetbox.SelectionLength = targetbox.Text.Length;
        }
        private void Handle_MinimizeClick(object sender, RoutedEventArgs e) => this.Hide();
        private void Handle_MaximizeClick(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Maximized;
        private void Handle_CloseClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void Handle_CheckForUpdatesClick(object sender, RoutedEventArgs e) => this.VCSettings.Updater.CheckForUpdateNow();
        private void Handle_LogFilterBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => logFilterBox.SelectedItem = null;
        private void Handle_KeySelectorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Tab))
                return; //< don't capture the tab key or it breaks keyboard navigation
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.LeftCtrl))
                return; //< only capture when CTRL is held down to allow proper searching
            if (sender is ComboBox cmb)
            {
                var eventKey = (EFriendlyKey)e.Key;
                int i = cmb.Items.IndexOf(eventKey);

                if (i.Equals(-1))
                    return;

                object? item = cmb.Items[i];

                if (this.TryFindResource("KeyOptions") is KeyOptions keys && keys.Contains(item))
                {
                    cmb.SelectedItem = eventKey;
                    e.Handled = true;
                }
            }
        }
        private void Handle_ReloadLanguageConfigs(object sender, RoutedEventArgs e) => LocalizationHelper.ReloadLanguageConfigs();

        private void Handle_HotkeyActionSettingsClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.CommandParameter is int hkid && HotkeyAPI.Hotkeys.FirstOrDefault(hk => hk.Hotkey.ID.Equals(hkid)) is HotkeyVM hk)
            {
                ActionSettingsWindow settingsWindow = new(owner: this, hotkey: hk);
                settingsWindow.ShowDialog();
            }
        }
        private void Handle_ResetNotificationPositionClick(object sender, RoutedEventArgs e)
        {
            var appWindows = Application.Current.Windows;

            var cp = this.GetPosAtCenterPoint();

            for (int i = 0; i < appWindows.Count; ++i)
            {
                if (appWindows[i] is SessionListNotification sessionListNotificationWindow)
                {
                    sessionListNotificationWindow.SetPosAtCenterPoint(cp);
                    VCAPI.Default.ShowSessionListNotification();
                }
                else if (appWindows[i] is DeviceListNotification deviceListNotificationWindow)
                {
                    deviceListNotificationWindow.SetPosAtCenterPoint(cp);
                    VCAPI.Default.ShowDeviceListNotification();
                }
            }
        }
        private void Handle_ForceHideNotificationClick(object sender, RoutedEventArgs e)
        {
            var appWindows = Application.Current.Windows;

            for (int i = 0; i < appWindows.Count; ++i)
            {
                if (appWindows[i] is SessionListNotification sessionListNotificationWindow)
                {
                    sessionListNotificationWindow.Hide();
                }
            }
        }
        /// <summary>
        /// Update TextProperty source when the Enter key is pressed while a textbox is focused.
        /// </summary>
        private void Handle_TextBoxKeyUp_UpdateTextBindingSource(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;

                if (textBox.GetBindingExpression(TextBox.TextProperty) is BindingExpression binding)
                {
                    binding.UpdateSource();
                }
            }
        }
        private void HiddenSessionProcessNamesCompletionBox_CommittedText(object sender, TextBoxWithCompletionOptions.CommittedTextEventArgs e)
        {
            if (e.Text.Trim().Length == 0)
            { // don't commit empty text
                e.Handled = true;
                return;
            }

            if (AudioAPI.AudioSessionManager.FindSessionWithProcessName(e.Text, StringComparison.OrdinalIgnoreCase) is CoreAudio.AudioSession session)
            {
                Settings.HiddenSessionProcessNames.Add(session.ProcessName);
            }
            else
            {
                Settings.HiddenSessionProcessNames.Add(e.Text);
            }

            // clear the text box:
            HiddenSessionProcessNamesCompletionBox.Text = string.Empty;
        }
        private void HiddenSessionProcessNamesCompletionBox_SuggestionClicked(object sender, TextBoxWithCompletionOptions.SuggestionClickedEventArgs e)
        {
            Settings.HiddenSessionProcessNames.Add(e.SuggestionText);
        }
        private void HiddenSessionProcessNamesRemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var listBoxItem = (ListBoxItem)button.DataContext;

            var index = HiddenSessionProcessNamesListBox.ItemContainerGenerator.IndexFromContainer(listBoxItem);
            Settings.HiddenSessionProcessNames.RemoveAt(index);
        }
        private void HiddenSessionProcessNamesCompletionBox_BackPressed(object sender, RoutedEventArgs e)
        {
            if (Settings.HiddenSessionProcessNames.Count == 0) return;
            Settings.HiddenSessionProcessNames.RemoveAt(Settings.HiddenSessionProcessNames.Count - 1);
        }
        private void MultiInstanceCheckbox_CheckStateChanged(object sender, RoutedEventArgs e)
        {
            // don't show the messagebox if window hasn't initialized yet, or if the user changes the setting back without restarting
            if (!_initialized || Settings.AllowMultipleDistinctInstances == _currentMultiInstanceState) return;

            if (MessageBox.Show(Loc.Tr("VolumeControl.Dialogs.MultiInstanceChanged.Message",
                            "Changes to the MultiInstance setting will take effect after you restart Volume Control.\n\n" +
                            "Do you want to restart the application now?"),
                            Loc.Tr("VolumeControl.Dialogs.MultiInstanceChanged.Caption", "Application Restart Required"),
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Exclamation,
                            MessageBoxResult.OK)
                == MessageBoxResult.OK)
            {
                Application.Current.Shutdown(); //< this waits for the method to return; doing this first seems to prevent crashes
                Process.Start(VCSettings.ExecutablePath, "--wait-for-mutex");
            }
        }
        #endregion EventHandlers
    }
}
