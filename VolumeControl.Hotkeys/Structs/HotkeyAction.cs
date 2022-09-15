using System.ComponentModel;
using System.Reflection;
using System.Windows.Media;
using VolumeControl.Core.Input.Actions;

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
        public void HandleKeyEvent(object? sender, HandledEventArgs e) => MethodInfo.Invoke(Instance, new object?[] { sender, e });
    }
}