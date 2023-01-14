using System.Reflection;
using System.Windows.Media;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input.Actions;
using VolumeControl.TypeExtensions;

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
            var methodParameters = MethodInfo.GetParameters();
            RequiresExtraParameters = methodParameters.Length > 2;
            AcceptsExtraActionSettings = methodParameters[1].ParameterType.Equals(typeof(HotkeyActionPressedEventArgs));
            ExtraActionSettings = GetExtraActionSettings();
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
            var methodParameters = MethodInfo.GetParameters();
            RequiresExtraParameters = methodParameters.Length > 2;
            AcceptsExtraActionSettings = methodParameters[1].ParameterType.Equals(typeof(HotkeyActionPressedEventArgs));
            ExtraActionSettings = GetExtraActionSettings();
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
            var methodParameters = MethodInfo.GetParameters();
            RequiresExtraParameters = methodParameters.Length > 2;
            AcceptsExtraActionSettings = methodParameters[1].ParameterType.Equals(typeof(HotkeyActionPressedEventArgs));
            ExtraActionSettings = GetExtraActionSettings();
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
        public bool RequiresExtraParameters { get; }
        /// <inheritdoc/>
        public bool AcceptsExtraActionSettings { get; }
        /// <inheritdoc/>
        public IHotkeyActionSetting[]? ExtraActionSettings { get; }
        /// <inheritdoc/>
        public bool UsesActionSettings => _usesActionSettings ??= RequiresExtraParameters || (AcceptsExtraActionSettings && ExtraActionSettings?.Length > 0);
        private bool? _usesActionSettings = null;

        /// <inheritdoc/>
        public void HandleKeyEvent(object? sender, HotkeyActionPressedEventArgs e)
        {
            List<object?> parameters = new() { sender, e };

            if (RequiresExtraParameters)
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
        public IHotkeyActionSetting[] GetDefaultActionSettings()
        {
            List<IHotkeyActionSetting> l = new();

            if (RequiresExtraParameters) //< Add parameters before extra action settings
            {
                foreach (var p in MethodInfo.GetParameters()[2..])
                {
                    if (p.GetCustomAttribute<HotkeyActionSettingAttribute>() is HotkeyActionSettingAttribute attr)
                    {
                        l.Add(new HotkeyActionSetting(attr));
                    }
                }
            }
            if (ExtraActionSettings is not null)
                ExtraActionSettings.ForEach(item =>
                {
                    if (item is not null)
                        l.Add((Activator.CreateInstance(item.GetType(), item) as IHotkeyActionSetting)!);
                });

            return l.ToArray();
        }

        private IHotkeyActionSetting[] GetExtraActionSettings()
        {
            List<IHotkeyActionSetting> l = new();
            var attributes = MethodInfo.GetCustomAttributes<HotkeyActionSettingAttribute>();

            foreach (var attr in attributes)
            {
                l.Add(new HotkeyActionSetting(attr));
            }

            return l.ToArray();
        }
    }
}