﻿using System.Diagnostics;
using System.Windows.Media;
using VolumeControl.Core.Input.Actions.Settings;
using VolumeControl.Log;

namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// An instance of a hotkey action.
    /// </summary>
    [DebuggerDisplay("Identifier = {Identifier}")]
    public class HotkeyActionInstance
    {
        #region Constructor
        internal HotkeyActionInstance(HotkeyActionDefinition definition, IActionSettingInstance[] actionSettings)
        {
            HotkeyActionDefinition = definition;
            ActionSettings = actionSettings;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets the HotkeyActionDefinition that defines this hotkey action.
        /// </summary>
        public HotkeyActionDefinition HotkeyActionDefinition { get; }
        /// <inheritdoc/>
        public string Name => HotkeyActionDefinition.Name;
        /// <inheritdoc/>
        public string? Description => HotkeyActionDefinition.Description;
        /// <inheritdoc/>
        public string? GroupName => HotkeyActionDefinition.GroupName;
        /// <inheritdoc/>
        public Brush? GroupBrush => HotkeyActionDefinition.GroupBrush;
        /// <inheritdoc/>
        public string Identifier => HotkeyActionDefinition.Identifier;
        /// <summary>
        /// Gets the action setting instances for this hotkey action.
        /// </summary>
        public IActionSettingInstance[] ActionSettings { get; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Invokes the method specified by the HotkeyActionDefinition
        /// </summary>
        /// <param name="sender">The hotkey instance to use as the sender of the event.</param>
        /// <param name="e">The event arguments to use for the event.</param>
        public void Invoke(IHotkey sender, HotkeyPressedEventArgs e)
        {
            try
            {
                HotkeyActionDefinition.Invoke_Unsafe(sender, e);
                if (FLog.Log.FilterEventType(EventType.TRACE))
                    FLog.Log.Trace($"Successfully executed action \"{Name}\".");
            }
            catch (Exception ex)
            {
                if (FLog.Log.FilterEventType(EventType.ERROR))
                    FLog.Log.Error($"Action \"{Name}\" triggered an exception:", ex);
            }
        }
        /// <summary>
        /// Invokes the method specified by the HotkeyActionDefinition with a new <see cref="HotkeyPressedEventArgs"/> instance.
        /// </summary>
        /// <param name="sender">The hotkey instance to use as the sender of the event.</param>
        public void Invoke(IHotkey sender) => Invoke(sender, new HotkeyPressedEventArgs(ActionSettings));
        #endregion Methods
    }
}
