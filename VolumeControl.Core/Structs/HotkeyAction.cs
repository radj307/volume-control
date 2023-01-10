using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Media;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Generics;
using VolumeControl.Core.Input.Actions;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Core.Structs
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
            List<object?> parameters = new() { sender, e };

            if (UsesExtraParameters)
            {
                if (e.ActionSettings is null)
                {
                    Log.FLog.Log.Error($"Action '{Name}' requires additional parameters, but they weren't provided!");
                    return;
                }

                var extraParams = MethodInfo.GetParameters()[2..];

                for (int i = 0; i < extraParams.Length; ++i)
                {
                    var actionSetting = e.ActionSettings[i];
                    var actionSettingType = actionSetting.ValueType ?? actionSetting.Value?.GetType();
                    var paramType = extraParams[i].ParameterType;
                    if (paramType.Equals(actionSettingType))
                    { // parameter is the correct type, use it
                        parameters.Add(actionSetting.Value);
                    }
                    else
                    {
                        Log.FLog.Log.Error($"Action setting '{actionSetting.Label}' is unexpected type '{actionSettingType?.FullName ?? "null"}'; expected type '{paramType.FullName}'.");
                        parameters.Add(paramType.Equals(typeof(string)) ? string.Empty : Activator.CreateInstance(paramType));
                    }
                }
            }

            MethodInfo.Invoke(Instance, parameters.ToArray());
        }

        /// <inheritdoc/>
        public HotkeyActionSetting[] GetDefaultActionSettings()
        {
            List<HotkeyActionSetting> l = new();

            if (UsesExtraParameters)
            {
                foreach (var p in MethodInfo.GetParameters()[2..])
                {
                    var attr = p.GetCustomAttribute<HotkeyActionSettingAttribute>();
                    l.Add(new(
                        label: attr?.SettingLabel ?? p.Name ?? string.Empty,
                        valueType: attr?.SettingType ?? p.ParameterType
                        ));
                }
            }

            return l.ToArray();
        }
    }
}