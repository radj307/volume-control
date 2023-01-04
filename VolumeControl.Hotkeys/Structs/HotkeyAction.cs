using System.ComponentModel;
using System.Reflection;
using System.Windows.Media;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Hotkeys.Structs
{

    /// <summary>
    /// Wrapper for a hotkey action method type.
    /// </summary>
    public class HotkeyAction : IHotkeyAction
    {
        /// <summary>
        /// Creates a new <see cref="HotkeyAction"/> instance.
        /// </summary>
        /// <param name="objectType"><see cref="Type"/></param>
        /// <param name="inst"><see cref="Instance"/></param>
        /// <param name="methodInfo"><see cref="MethodInfo"/></param>
        /// <param name="data"><see cref="Data"/></param>
        public HotkeyAction(Type objectType, object? inst, MethodInfo methodInfo, HotkeyActionData data)
        {
            Data = data;
            Type = objectType;
            Instance = inst;
            MethodInfo = methodInfo;
            UsesExtraParameters = MethodInfo.GetParameters().Length > 2;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyAction"/> instance.
        /// </summary>
        /// <param name="inst"><see cref="Instance"/></param>
        /// <param name="methodInfo"><see cref="MethodInfo"/></param>
        /// <param name="data"><see cref="Data"/></param>
        public HotkeyAction(object inst, MethodInfo methodInfo, HotkeyActionData data)
        {
            Data = data;
            Type = inst.GetType();
            Instance = inst;
            MethodInfo = methodInfo;
            UsesExtraParameters = MethodInfo.GetParameters().Length > 2;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyAction"/> instance.
        /// </summary>
        /// <param name="objectType"><see cref="Type"/></param>
        /// <param name="methodInfo"><see cref="MethodInfo"/></param>
        /// <param name="data"><see cref="Data"/></param>
        public HotkeyAction(Type objectType, MethodInfo methodInfo, HotkeyActionData data)
        {
            Data = data;
            Type = objectType;
            Instance = null;
            MethodInfo = methodInfo;
            UsesExtraParameters = MethodInfo.GetParameters().Length > 2;
        }

        /// <inheritdoc/>
        public Type Type { get; }
        /// <inheritdoc/>
        public MethodInfo MethodInfo { get; }
        /// <inheritdoc/>
        public object? Instance { get; }
        /// <inheritdoc/>
        public HotkeyActionData Data { get; set; }
        /// <inheritdoc cref="HotkeyActionData.ActionName"/>
        public string Identifier => $"{(this.Data.ActionGroup is null ? "" : $"{this.Data.ActionGroup}:")}{this.Data.ActionName}";
        /// <inheritdoc/>
        public string Name => Data.ActionName;
        /// <inheritdoc cref="HotkeyActionData.ActionDescription"/>
        public string? Description => Data.ActionDescription;
        /// <inheritdoc cref="HotkeyActionData.ActionGroup"/>
        public string? GroupName => Data.ActionGroup;
        /// <inheritdoc cref="HotkeyActionData.ActionGroupBrush"/>
        public Brush? GroupBrush => Data.ActionGroupBrush;
        /// <inheritdoc/>
        public bool UsesExtraParameters { get; }

        /// <inheritdoc/>
        public void HandleKeyEvent(object? sender, HotkeyActionPressedEventArgs e)
        {
            List<object?> parameters = new() { sender, (HandledEventArgs)e };

            if (UsesExtraParameters)
            {
                if (e.ActionSettings is null)
                {
                    Log.FLog.Log.Error($"Action '{Name}' requires additional parameters, but they weren't provided!");
                    return;
                }

                var extraParams = MethodInfo.GetParameters()[2..];

                if (extraParams.Length != e.ActionSettings.Count)
                    throw new InvalidOperationException($"Method parameters and available action settings do not match! (Try re-selecting this hotkey's associated action to fix this.)");

                int i = 0;
                foreach (var p in extraParams)
                {
                    parameters.Add(e.ActionSettings[i++].Value);
                }
            }

            MethodInfo.Invoke(Instance, parameters.ToArray());
        }

        /// <inheritdoc/>
        public List<HotkeyActionSetting> GetDefaultActionSettings()
        {
            List<HotkeyActionSetting> l = new();

            if (UsesExtraParameters)
            {
                MethodInfo.GetParameters()[2..].ForEach(p => l.Add(new HotkeyActionSetting()
                {
                    Name = p.Name is null ? string.Empty : HotkeyActionAttribute.InsertSpacesIn(p.Name),
                    Value = p.ParameterType.Equals(typeof(string)) ? string.Empty : Activator.CreateInstance(p.ParameterType)
                }));
            }

            return l;
        }
    }
}