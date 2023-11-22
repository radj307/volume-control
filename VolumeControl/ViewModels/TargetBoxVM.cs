using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VolumeControl.Core;
using VolumeControl.CoreAudio;

namespace VolumeControl.ViewModels
{
    public class TargetBoxVM : INotifyPropertyChanged
    {
        #region Constructor
        public TargetBoxVM(AudioSessionManager audioSessionManager, AudioSessionMultiSelector audioSessionMultiSelector)
        {
            AudioSessionManager = audioSessionManager;

            _duplicateSessionNames = GetDuplicateSessionNames();

            var previousTargetInfo = Settings.TargetSession;

            _targetText = GetSessionName(AudioSessionManager.FindSessionWithSimilarProcessIdentifier(previousTargetInfo.ProcessIdentifier)) ?? previousTargetInfo.ProcessName;

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
        private bool _settingTarget = false;
        private bool _savingTarget = false;
        private IEnumerable<string> _duplicateSessionNames;
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

                SetTarget(_targetText);

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

        #region (Private) SetTargetTextTo
        private void SetTargetTextTo(AudioSession? session)
        {
            _targetText = session != null
                ? GetSessionName(session)!
                : string.Empty;
            NotifyPropertyChanged(nameof(TargetText));
        }
        #endregion (Private) SetTargetTextTo

        #region (Private) SetTarget
        private void SetTarget(string target, bool setToNullIfNotFound = true)
        {
            _settingTarget = true;
            if (AudioSessionManager.FindSessionWithSimilarProcessIdentifier(target) is AudioSession session)
            {
                AudioSessionMultiSelector.CurrentSession = session;
            }
            else if (setToNullIfNotFound)
            {
                AudioSessionMultiSelector.CurrentSession = null;
            }
            _settingTarget = false;
        }
        #endregion (Private) SetTarget

        #region (Private) SaveTarget
        private void SaveTarget()
        {
            _savingTarget = true;
            Settings.TargetSession = AudioSessionMultiSelector.CurrentSession?.GetTargetInfo() ?? new()
            {
                ProcessName = TargetText
            };
            _savingTarget = false;
        }
        #endregion (Private) SaveTarget

        #region (Private) GetDuplicateSessionNames
        private IEnumerable<string> GetDuplicateSessionNames()
            => from session in AudioSessionManager.Sessions
               group session.ProcessName by session.ProcessName into g
               where g.Count() > 1
               select g.Key;
        #endregion (Private) GetDuplicateSessionNames

        #region (Private) GetSessionName
        private string? GetSessionName(AudioSession? session)
        {
            if (session == null)
                return null;
            else if (_duplicateSessionNames.Contains(session.ProcessName))
                return session.ProcessIdentifier;
            else
                return session.Name;
        }
        #endregion (Private) GetSessionName

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
            _duplicateSessionNames = GetDuplicateSessionNames();

            if (!_removingCurrentSession) return;
            _removingCurrentSession = false;
        }
        private void AudioSessionManager_AddedSessionToList(object? sender, AudioSession e)
        { // resolve incoming session
            _duplicateSessionNames = GetDuplicateSessionNames();

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
                    _settingTarget = true;
                    AudioSessionMultiSelector.CurrentSession = e;
                    SaveTarget();
                    if (isProcessIdentifier) // update the process identifier if that's what we have
                        SetTargetTextTo(e);
                    _settingTarget = false;
                }
                // else this isn't our session
            }
        }
        #endregion AudioSessionManager

        #region AudioSessionMultiSelector
        private void AudioSessionMultiSelector_CurrentSessionChanged(object? sender, AudioSession? e)
        {
            if (_settingTarget) return;

            if (e != null)
            {
                SetTargetTextTo(e);
            }
            SaveTarget();
        }
        #endregion AudioSessionMultiSelector

        #region Settings
        private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_savingTarget || _settingTarget || e.PropertyName == null) return;

            if (e.PropertyName.Equals(nameof(Config.TargetSession), StringComparison.Ordinal))
            {
                SetTarget(Settings.TargetSession.ProcessIdentifier);
            }
        }
        #endregion Settings

        #endregion EventHandlers
    }
}
