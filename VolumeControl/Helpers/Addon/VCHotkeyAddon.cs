using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Structs;
using VolumeControl.SDK;
using VolumeControl.SDK.Delegates;
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
            foreach (Type t in types) //< enumerate through all of the received types
            {
                actions = new(); // start collecting action methods
                if (FindMembersWithAttributesInType(t) is (MemberInfo?, Attribute)[] attributes && attributes.Length > 0)
                {
                    var actionGroupAttr = attributes.FirstOrDefault(pr => pr.Item2 is HotkeyActionGroupAttribute).Item2 as HotkeyActionGroupAttribute;

                    string? defaultGroupName = actionGroupAttr?.GroupName;
                    Brush? defaultGroupBrush = actionGroupAttr?.GroupColor is not null ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(actionGroupAttr.GroupColor)) : null;

                    foreach ((MemberInfo? mInfo, Attribute attr) in attributes) //< enumerate through all of the attributes on the method
                    {
                        if (attr.Equals(actionGroupAttr)) continue; //< skip the group attribute that we already consumed

                        if (attr is HotkeyActionAttribute hAttr && mInfo is MethodInfo methodInfo)
                        {
                            var data = hAttr.GetActionData();
                            if (data.ActionGroup is null && defaultGroupName is not null)
                                data.ActionGroup = defaultGroupName;
                            if (data.ActionGroupBrush is null && defaultGroupBrush is not null)
                                data.ActionGroupBrush = defaultGroupBrush;

                            // validate parameters & return type:
                            if (!methodInfo.ReturnType.Equals(typeof(void)))
                            {
                                Log.Debug($"Addon method return value is discarded");
                            }

                            var parameters = methodInfo.GetParameters();
                            if (parameters.Length < 2)
                            {
                                Log.Debug(
                                    $"Addon method is invalid: '{methodInfo.Name}' (Invalid Function Declaration; Missing Parameters)",
                                    $"Hotkey action methods must accept a first parameter of type `{typeof(object).FullName}`, and a second parameter of type `{typeof(System.ComponentModel.HandledEventArgs).FullName}`!",
                                    $"For an example of valid syntax, see \"{typeof(HotkeyActionDelegate).FullName}\".");
                                continue;
                            }
                            else if (!parameters[0].ParameterType.Equals(typeof(object)) || !parameters[1].ParameterType.Equals(typeof(System.ComponentModel.HandledEventArgs)))
                            {
                                Log.Debug(
                                    $"Addon method is invalid: '{methodInfo.Name}' (Invalid Function Declaration; First Parameter)",
                                    $"Hotkey action methods must accept a first parameter of type `{typeof(object).FullName}`, and a second parameter of type `{typeof(System.ComponentModel.HandledEventArgs).FullName}`!",
                                    $"For an example of valid syntax, see \"{typeof(HotkeyActionDelegate).FullName}\".");
                                continue;
                            }

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
