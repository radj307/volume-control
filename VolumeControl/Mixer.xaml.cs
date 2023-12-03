using Localization;
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
using VolumeControl.Core.Extensions;
using VolumeControl.Core.Input;
using VolumeControl.Core.Input.Enums;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.SDK;
using VolumeControl.SDK.Internal;
using VolumeControl.ViewModels;
using VolumeControl.WPF;
using VolumeControl.WPF.Controls;
using VolumeControl.WPF.Extensions;

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
            if (!_loaded)
            {
                _loaded = true;

                // restore previous position
                if (Settings.RestoreMainWindowPosition && Settings.MainWindowPosition.HasValue)
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.SetPosAtCorner(Settings.MainWindowOriginCorner, Settings.MainWindowPosition.Value);
                    });
                }
            }
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
        private static Config Settings => Config.Default;
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
            var session = (AudioSessionVM)MixerGrid.CurrentCell.Item;
            VCAPI.Default.AudioSessionMultiSelector.CurrentSession = session.AudioSession;
        }
        /// <summary>Handles the create new hotkey button's click event.</summary>
        private void Handle_CreateNewHotkeyClick(object sender, RoutedEventArgs e)
            => this.HotkeyAPI.HotkeyManager.AddHotkey(new HotkeyWithError(EFriendlyKey.None, EModifierKey.None, false));
        /// <summary>Handles the remove hotkey button's click event.</summary>
        private void Handle_hotkeyGridRemoveClick(object sender, RoutedEventArgs e)
        {
            var hotkeyGridRemove = (Button)sender;
            var id = (ushort)hotkeyGridRemove.CommandParameter;

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
                    FLog.Info($"User specified option '{MessageBoxResult.Cancel:G}' in the remove hotkey confirmation dialog, indicating they want to disable the confirmation dialog; the '{nameof(Settings.DeleteHotkeyConfirmation)}' setting has been set to false.");
                    return;
                }
            }

            var hk = this.HotkeyAPI.HotkeyManager.GetHotkey(id);
            FLog.Debug($"User is removing hotkey {id} {hk?.Name}{(hk == null || hk.Name.Length == 0 ? "" : " ")}({hk?.GetStringRepresentation()})");

            this.HotkeyAPI.HotkeyManager.RemoveHotkey(id);
        }
        /// <inheritdoc cref="VolumeControlVM.ResetHotkeySettings"/>
        private void Handle_ResetHotkeysClick(object sender, RoutedEventArgs e)
        {
            FLog.Info("User clicked the reset hotkeys button. Showing confirmation prompt.");
            if (MessageBoxResult.Yes == MessageBox.Show(
                Loc.Tr("VolumeControl.Dialogs.ResetHotkeys.Message"),
                Loc.Tr("VolumeControl.Dialogs.ResetHotkeys.Caption"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No))
            {
                VCSettings.HotkeyAPI.ResetHotkeys();

                FLog.Info("All hotkeys were reset to default.");
            }
            else FLog.Info("User cancelled the reset hotkeys operation.");
        }
        private void Handle_BrowseForLogFilePathClick(object sender, RoutedEventArgs e)
        {
            string initialPath = Path.GetFullPath(Settings.LogPath);

            var sfd = new SaveFileDialog()
            {
                OverwritePrompt = false,
                DefaultExt = "log",
                InitialDirectory = initialPath,
                Title = "Choose a location to save the log file.",
                FileName = Path.GetFileName(initialPath),
            };
            sfd.ShowDialog(this);

            var path = Path.GetFullPath(sfd.FileName);
            if (!path.Equals(initialPath, StringComparison.OrdinalIgnoreCase))
            {
                if (ShellHelper.CanWriteToDirectory(Path.GetDirectoryName(path)!))
                {
                    logPath.Text = path; //< updates the actual log path via data binding
                }
                else
                {
                    MessageBox.Show($"The application does not have write access to the specified directory:\n\"{path}\"\nThe log path was not changed.", "Cancelled");
                }
            }
        }
        private void Handle_OpenLogClick(object sender, RoutedEventArgs e)
        {
            ShellHelper.OpenWithDefault(VCSettings.LogFilePath);
        }
        private void Handle_TargetNameBoxDoubleClick(object sender, MouseButtonEventArgs e)
        {
            targetbox.SelectionStart = 0;
            targetbox.SelectionLength = targetbox.Text.Length;
        }
        private void Handle_MinimizeClick(object sender, RoutedEventArgs e) => this.Hide();
        private void Handle_MaximizeClick(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Maximized;
        private void Handle_CloseClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown(Program.EXITCODE_SUCCESS);
        private void Handle_CheckForUpdatesClick(object sender, RoutedEventArgs e) => this.VCSettings.Updater.CheckForUpdateNow();
        private void ComboBox_RemoveSelection(object sender, SelectionChangedEventArgs e)
        {
            ((ComboBox)sender).SelectedItem = null;
        }
        private void Handle_ReloadLanguageConfigs(object sender, RoutedEventArgs e)
        {
            LocalizationHelper.ReloadTranslations();
        }

        private void Handle_HotkeyActionSettingsClick(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var hkid = (ushort)b.CommandParameter;
            var hk = HotkeyAPI.Hotkeys.FirstOrDefault(hk => hk.Hotkey.ID.Equals(hkid));

            if (hk == null) return;

            try
            {
                ActionSettingsWindow settingsWindow = new(owner: this, hotkey: hk);
                settingsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                FLog.Critical($"The {nameof(ActionSettingsWindow)} for hotkey {hk.Hotkey.ID} \"{hk.Hotkey.Name}\" failed due to an exception:", ex);

#if DEBUG
                throw; //< rethrow in debug configuration
#endif
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
                    sessionListNotificationWindow.ResetPosition();
                    VCAPI.Default.ShowSessionListNotification();
                }
                else if (appWindows[i] is DeviceListNotification deviceListNotificationWindow)
                {
                    deviceListNotificationWindow.ResetPosition();
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
                Application.Current.Shutdown(Program.EXITCODE_RESTARTING); //< this waits for the method to return; doing this first seems to prevent crashes
                Process.Start(VCSettings.ExecutablePath, "--wait-for-mutex")?.Dispose();
            }
        }
        private void window_Closing(object sender, CancelEventArgs e)
        {
            // save current position
            if (Settings.RestoreMainWindowPosition)
            {
                var corner = this.GetCurrentScreenCorner();
                Settings.MainWindowOriginCorner = corner;
                Settings.MainWindowPosition = this.GetPosAtCorner(corner);
            }
        }
        private void window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.PrimaryDevice.IsModifierKeyDown(EModifierKey.Alt))
                this.DragMove();
        }

        #region KeySelectorToggleButton
        private void KeySelectorToggleButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = (Button)sender;
            button.PreviewKeyDown += this.KeySelectorToggleButton_PreviewKeyDown;
            FLog.Info("[KeySelector] Activated, waiting for input...");
        }
        private void KeySelectorToggleButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var button = (Button)sender;
            button.PreviewKeyDown -= this.KeySelectorToggleButton_PreviewKeyDown;
            FLog.Info("[KeySelector] Deactivated.");
        }
        private void KeySelectorToggleButton_PreviewKeyDown(object sender, KeyEventArgs e)
        { // find the key that was pressed and select it
            var button = (Button)sender;
            var cmb = (ComboBox)button.Tag;

            var friendlyKey = (EFriendlyKey)e.Key;

            int i = 0, i_max = cmb.Items.Count;
            for (; i < i_max; ++i)
            {
                if (friendlyKey == ((KeyOptions.FriendlyKeyVM)cmb.Items[i]).Key)
                    break;
            }
            if (i == i_max)
            {
                FLog.Error($"[KeySelector] Key \"{friendlyKey:G}\" was pressed, but it isn't supported!");
                return; //< key wasn't found
            }
            else FLog.Info($"[KeySelector] Key \"{friendlyKey:G}\" was pressed & selected.");

            cmb.SelectedIndex = i;
            e.Handled = true;
        }
        #endregion KeySelectorToggleButton

        #endregion EventHandlers
    }
}
