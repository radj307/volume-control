using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.Hotkeys.Structs;
using VolumeControl.SDK;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers.Addon
{
    public class VCHotkeyAddon : VCAddonType
    {
        public VCHotkeyAddon() : base(new AttributeWrapper(typeof(HotkeyActionAttribute)), new AttributeWrapper(typeof(HotkeyActionGroupAttribute)))
        { }

        protected override void HandleAddonTypes(VCAPI api, IEnumerable<Type> types)
        {
            List<HotkeyAction> actions;
            foreach (Type t in types)
            {
                actions = new();
                if (FindAttributesOnType(t) is (MemberInfo?, Attribute)[] attributes && attributes.Length > 0)
                {
                    var actionAttr = attributes.FirstOrDefault(pr => pr.Item2 is HotkeyActionGroupAttribute).Item2 as HotkeyActionGroupAttribute;

                    string? defaultGroupName = actionAttr?.GroupName;
                    Brush? defaultGroupBrush = actionAttr?.GroupColor is not null ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(actionAttr.GroupColor)) : null;

                    foreach ((MemberInfo? mInfo, Attribute attr) in attributes)
                    {
                        if (attr.Equals(actionAttr)) continue;

                        if (attr is HotkeyActionAttribute hAttr && mInfo is MethodInfo methodInfo)
                        {
                            var data = hAttr.GetActionData();
                            if (data.ActionGroup is null && defaultGroupName is not null)
                                data.ActionGroup = defaultGroupName;
                            if (data.ActionGroupBrush is null && defaultGroupBrush is not null)
                                data.ActionGroupBrush = defaultGroupBrush;
                            actions.Add(new HotkeyAction(t, Activator.CreateInstance(t), methodInfo, data));
                        }
                        else
                        {
                            Log.Error($"Addon method is invalid: {mInfo?.Name}");
                        }
                    }
                }

                if (actions.Count > 0)
                {
                    Log.Debug($"Found {actions.Count} addon methods in type \"{t.FullName}\"");
                    api.HotkeyManager.Actions.Bindings.AddRangeIfUnique(actions);
                }
                else
                    Log.Debug($"No addon methods found in type \"{t.FullName}\".");
            }
        }
    }
}
