using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core;
using VolumeControl.CoreAudio;

namespace VolumeControl.ViewModels
{
    public class TargetSessionVM : INotifyPropertyChanged
    {
        #region Constructor
        public TargetSessionVM(AudioSessionManager audioSessionManager, AudioSessionMultiSelector audioSessionMultiSelector)
        {
            _targetText = Settings.TargetSession.ProcessIdentifier;

            AudioSessionManager = audioSessionManager;
            AudioSessionMultiSelector = audioSessionMultiSelector;

            AudioSessionManager.RemovingSessionFromList += this.AudioSessionManager_RemovingSessionFromList;
            AudioSessionManager.RemovedSessionFromList += this.AudioSessionManager_RemovedSessionFromList;
            AudioSessionManager.AddedSessionToList += this.AudioSessionManager_AddedSessionToList;
            AudioSessionMultiSelector.CurrentSessionChanged += this.AudioSessionMultiSelector_CurrentSessionChanged;
            Settings.PropertyChanged += this.Settings_PropertyChanged;
        }
        #endregion Constructor

        #region Fields
        private bool _removingCurrentSession = false;
        private bool _isSettingTarget = false;
        #endregion Fields

        #region Properties
        private static Config Settings => Config.Default;
        private AudioSessionManager AudioSessionManager { get; }
        private AudioSessionMultiSelector AudioSessionMultiSelector { get; }
        public string TargetText
        {
            get => _targetText;
            set
            {
                var trimmedValue = value.Trim();
                if (trimmedValue.Equals(_targetText, StringComparison.Ordinal)) return;

                _targetText = trimmedValue;

                _isSettingTarget = true;
                SetTarget(_targetText);
                _isSettingTarget = false;

                NotifyPropertyChanged();

                SaveTarget();
            }
        }
        private string _targetText;
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods
        private void UpdateTargetText(bool notify = true)
        {
            _targetText = AudioSessionMultiSelector.CurrentSession?.ProcessIdentifier ?? string.Empty;
            if (notify) NotifyPropertyChanged(nameof(TargetText));
        }
        private void SetTarget(string target)
        {
            _isSettingTarget = true;
            if (AudioSessionManager.FindSessionWithSimilarProcessIdentifier(target) is AudioSession session)
            {
                AudioSessionMultiSelector.CurrentSession = session;
            }
            _isSettingTarget = false;
        }
        private void SaveTarget()
        {
            Settings.TargetSession = AudioSessionMultiSelector.CurrentSession?.GetTargetInfo() ?? new()
            {
                ProcessName = TargetText
            };
        }
        #endregion Methods

        #region EventHandlers

        #region AudioSessionManager
        private void AudioSessionManager_RemovingSessionFromList(object? sender, AudioSession e)
        { // prevent selected session update from changing target
            if (!e.Equals(AudioSessionMultiSelector.CurrentSession)) return;
            _removingCurrentSession = true;
        }
        private void AudioSessionManager_RemovedSessionFromList(object? sender, AudioSession e)
        { // allow selected session updates to change target
            if (!_removingCurrentSession) return;
            _removingCurrentSession = false;
        }
        private void AudioSessionManager_AddedSessionToList(object? sender, AudioSession e)
        { // resolve incoming session
            if (AudioSessionMultiSelector.CurrentSession == null && TargetText.Length > 0)
            { // current session is null but the target text is not empty
                // split process identifier first
                int separatorPos = TargetText.IndexOf(':');
                bool isProcessIdentifier = separatorPos != -1;
                string targetText = (isProcessIdentifier && separatorPos != TargetText.Length - 1)
                    ? TargetText[(separatorPos + 1)..]
                    : TargetText;

                if (e.HasMatchingName(targetText, StringComparison.OrdinalIgnoreCase)
                    || (int.TryParse(targetText, out int value) && value == e.PID))
                {
                    _isSettingTarget = true;
                    AudioSessionMultiSelector.CurrentSession = e;
                    SaveTarget();
                    if (isProcessIdentifier) // update the process identifier if that's what we have
                        UpdateTargetText();
                    _isSettingTarget = false;
                }
                // else this isn't our session
            }
        }
        #endregion AudioSessionManager

        #region AudioSessionMultiSelector
        private void AudioSessionMultiSelector_CurrentSessionChanged(object? sender, AudioSession? e)
        {
            if (_isSettingTarget) return;

            if (e != null)
            {
                UpdateTargetText();
            }
        }
        #endregion AudioSessionMultiSelector

        #region Settings
        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isSettingTarget || e.PropertyName == null) return;

            if (e.PropertyName.Equals(nameof(Config.TargetSession), StringComparison.Ordinal))
            {
                SetTarget(Settings.TargetSession.ProcessIdentifier);
            }
        }
        #endregion Settings

        #endregion EventHandlers
    }
}
