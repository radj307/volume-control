using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Helpers;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Input.Actions.Settings;
using VolumeControl.Log;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Provides methods to load <see cref="HotkeyActionDefinition"/> instances from types or assemblies using reflection.
    /// </summary>
    public static class HotkeyActionAddonLoader
    {
        #region LoadProviders
        /// <summary>
        /// Loads DataTemplate providers from the specified <paramref name="types"/> and registers them with the <paramref name="provider"/>. 
        /// </summary>
        /// <param name="provider">The <see cref="TemplateProviderManager"/> instance to load the provider types into.</param>
        /// <param name="types">Any number of <see cref="Type"/> instances that represent classes with the <see cref="DataTemplateProviderAttribute"/>.</param>
        public static void LoadProviders(ref TemplateProviderManager provider, params Type[] types)
        {
            // load provider types
            for (int i = 0, i_max = types.Length; i < i_max; ++i)
            {
                var type = types[i];

                if (type.GetCustomAttribute<DataTemplateProviderAttribute>(true) == null)
                    continue;

                try
                {
                    provider.RegisterProvider(type);
                }
                catch (Exception ex)
                {
                    FLog.Error($"[ActionAddonLoader] Failed to load {nameof(DataTemplate)} provider type \"{type}\" due to an exception:", ex);
                }
            }

        }
        #endregion LoadProviders

        #region ValidateMethodIsEligibleAsAction
        private enum EMethodValidationState : byte
        {
            IsValid = 0,
            Warning = 1,
            Error = 2,
        }
        private static EMethodValidationState ValidateMethodIsEligibleAsAction(MethodInfo method)
        {
            var state = EMethodValidationState.IsValid;

            if (method.ReturnParameter.ParameterType != typeof(void))
            {
                FLog.Log.Warning($"[ActionMethodValidator] {method.GetFullMethodName()} returns type {method.ReturnType.Name}; expected type {typeof(void).Name}. The returned value will be ignored.");
                state = EMethodValidationState.Warning;
            }

            var parameters = method.GetParameters();

            // NO PARAMETERS
            if (parameters.Length == 0)
            {
                FLog.Log.Error($"[ActionMethodValidator] {method.GetFullMethodName()} is invalid because it doesn't have enough parameters.",
                    $"  Expected:  object? sender, {typeof(HotkeyPressedEventArgs).Name} e");
                state = EMethodValidationState.Error;
            }
            // PARAMETER 0 "sender"
            if (parameters.Length >= 1)
            {
                var param = parameters[0];
                var paramType = param.ParameterType;
                if (paramType != typeof(object) && paramType != typeof(Hotkey) && paramType != typeof(IHotkey))
                {
                    FLog.Log.Error($"[ActionMethodValidator] {method.GetFullMethodName()} is invalid because parameter 0 is incorrect. (Invalid Type)",
                        $"  Expected:  {typeof(object).FullName} sender",
                        $"  Actual:    {paramType.FullName} {param.Name}");
                    state = EMethodValidationState.Error;
                }
            }
            else
            {
                FLog.Log.Error($"[ActionMethodValidator] {method.GetFullMethodName()} is invalid because parameter 0 is missing.",
                    $"  Expected:  {typeof(object).FullName} sender");
            }
            // PARAMETER 1 "e"
            if (parameters.Length >= 2)
            {
                var param = parameters[1];
                var paramType = param.ParameterType;
                if (paramType != typeof(HotkeyPressedEventArgs) && paramType != typeof(System.ComponentModel.HandledEventArgs) && paramType != typeof(EventArgs))
                {
                    FLog.Log.Error($"[ActionMethodValidator] {method.GetFullMethodName()} is invalid because parameter 1 is incorrect. (Invalid Type)",
                        $"  Expected:  {typeof(HotkeyPressedEventArgs).FullName} e",
                        $"  Actual:    {paramType.FullName} {param.Name}");
                    state = EMethodValidationState.Error;
                }
            }
            else
            {
                FLog.Log.Error($"[ActionMethodValidator] {method.GetFullMethodName()} is invalid because parameter 1 is missing.",
                    $"  Expected:  {typeof(HotkeyPressedEventArgs).FullName} e");
            }
            // EXTRA PARAMETERS
            if (parameters.Length > 2)
            {
                FLog.Log.Error($"[ActionMethodValidator] {method.GetFullMethodName()} is invalid because it has too many parameters.",
                    $"Expected:  {method.Name}(object? sender, {typeof(HotkeyPressedEventArgs).Name} e)",
                    $"Actual:    {method.GetFullMethodName()}");
            }

            return state;
        }
        #endregion ValidateMethodIsEligibleAsAction

        #region LoadActions
        /// <summary>
        /// Loads hotkey actions from the specified <paramref name="types"/>.
        /// </summary>
        /// <param name="provider">The <see cref="TemplateProviderManager"/> instance to use for resolving data template providers.</param>
        /// <param name="types">Any number of <see cref="Type"/> instances that represent hotkey action group types.</param>
        /// <returns>List of <see cref="HotkeyActionDefinition"/> objects for all publicly-accessible action methods.</returns>
        public static HotkeyActionDefinition[] LoadActions(TemplateProviderManager provider, params Type[] types)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

            List<HotkeyActionDefinition> l = new();
            int typesWithGroupAttrCount = 0; //< tracks the number of types that have the HotkeyActionGroupAttribute

            // enumerate through all public types in this assembly:
            for (int i = 0, i_max = types.Length; i < i_max; ++i)
            {
                var type = types[i];
                var actionGroupAttribute = type.GetCustomAttribute<HotkeyActionGroupAttribute>();

                // if this type doesn't have an action group attribute, skip it
                if (actionGroupAttribute == null) continue;
                else ++typesWithGroupAttrCount;

                // get all public methods from this type
                var publicMethods = type.GetMethods(bindingFlags);

                // if this type doesn't have any public methods, skip it
                if (publicMethods.Length == 0)
                {
                    FLog.Error($"[ActionAddonLoader] {type.FullName} doesn't contain any publicly-accessible methods marked with {typeof(HotkeyActionAttribute).FullName}!");
                    continue;
                }

                object? groupInst = null;
                int loadedActionsFromTypeCount = 0, //< tracks the number of actions that were loaded from the current type
                    methodsWithActionAttrCount = 0; //< tracks the number of methods in the current type with HotkeyActionAttribute
                bool typeIsStatic = type.IsAbstract && type.IsSealed; //< C# makes static types abstract & sealed

                // enumerate through all public methods in this type:
                for (int j = 0, j_max = publicMethods.Length; j < j_max; ++j)
                {
                    var method = publicMethods[j];
                    var actionAttribute = method.GetCustomAttribute<HotkeyActionAttribute>();

                    // if this method doesn't have an action attribute, skip it
                    if (actionAttribute == null) continue;
                    else ++methodsWithActionAttrCount;

                    // make sure the method is valid as a hotkey action
                    if (ValidateMethodIsEligibleAsAction(method) == EMethodValidationState.Error)
                    {
                        // this doesn't need more information because ValidateMethodIsEligibleAsAction
                        //  logs all of the problems in detail anyway.
                        FLog.Error($"[ActionAddonLoader] {method.GetFullMethodName()} was skipped because it is invalid.");
                        continue;
                    }

                    // get the action settings for this method
                    List<ActionSettingDefinition> actionSettingDefs = new();

                    if (FLog.FilterEventType(EventType.DEBUG))
                        FLog.Debug($"[ActionAddonLoader] Loading action setting definitions for \"{method.GetFullMethodName()}\"");

                    // enumerate action setting attributes
                    foreach (var actionSettingAttribute in method.GetCustomAttributes<HotkeyActionSettingAttribute>())
                    {
                        var providerType = actionSettingAttribute.DataTemplateProviderType
                            ?? actionAttribute.DefaultDataTemplateProvider
                            ?? actionGroupAttribute.DefaultDataTemplateProvider;
                        var providerTemplateKey = actionSettingAttribute.DataTemplateProviderKey;

                        DataTemplate? dataTemplate = null;
                        try
                        {
                            dataTemplate = provider.FindDataTemplateFor(providerType, providerTemplateKey, actionSettingAttribute.ValueType);
                        }
                        catch (Exception ex)
                        {
                            FLog.Error($"[ActionAddonLoader] An exception occurred while searching for a DataTemplate for setting \"{actionSettingAttribute.Name}\" on method \"{method.Name}\":", ex);
                        }

                        if (dataTemplate == null)
                        {
                            FLog.Error(
                                $"[ActionAddonLoader] Failed to get a DataTemplate for setting \"{actionSettingAttribute.Name}\" on method \"{method.Name}\"!",
                                $"                    This can be resolved by specifying a data template provider that supports type \"{actionSettingAttribute.ValueType}\"");
                        }

                        actionSettingDefs.Add(new(actionSettingAttribute.Name, actionSettingAttribute.ValueType, actionSettingAttribute.DefaultValue, actionSettingAttribute.Description, actionSettingAttribute.IsToggleable, actionSettingAttribute.StartsEnabled, dataTemplate));
                    }

                    // make sure there aren't any duplicate action setting names
                    var distinctSettingNames = actionSettingDefs.Select(d => d.Name).Distinct();
                    if (distinctSettingNames.Count() != actionSettingDefs.Count)
                    {
                        string duplicateNames = string.Join(", ", actionSettingDefs
                            .GroupBy(d => d.Name)
                            .Where(g => g.Count() > 1)
                            .Select(g => $"\"{g.Key}\""));
                        FLog.Error(
                            $"[ActionAddonLoader] {method.GetFullMethodName()} was skipped because multiple settings have the same name: {duplicateNames}!",
                            $"                    This can be resolved by removing one or both of the offending settings.");
                        continue;
                    }

                    var actionGroupColorString = actionAttribute.GroupColorOverride ?? actionGroupAttribute.GroupColor;

                    // create the hotkey action definition for this method
                    HotkeyActionDefinition? hotkeyActionDefinition;
                    if (!typeIsStatic && !method.IsStatic)
                    { // non-static method
                        if (groupInst == null)
                        { // create a new instance because one doesn't exist
                            // ensure this type isn't abstract before we try to instantiate it
                            if (type.IsAbstract)
                            {
                                FLog.Error(
                                    $"[ActionAddonLoader] {method.GetFullMethodName()} cannot be loaded because class {type.Name} is abstract!",
                                    $"                    This can be resolved by not defining {type.Name} as \"abstract\", or defining {method.Name} as \"static\".");
                                continue;
                            }

                            // find a valid constructor
                            if (type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Array.Empty<Type>()) is not ConstructorInfo ctor)
                            { // no valid constructor was found; log this error
                                FLog.Error(
                                    $"[ActionAddonLoader] {method.GetFullMethodName()} was skipped because class {type.Name} does not define a parameterless constructor!",
                                    $"                    This can be resolved by removing all constructors with parameters. If your class needs to initialize something, do that inside a parameterless constructor.");
                                continue;
                            }

                            // create a new instance using the constructor
                            try
                            {
                                groupInst = ctor.Invoke(Array.Empty<object>())!;
                            }
                            catch (Exception ex)
                            {
                                FLog.Error($"[ActionAddonLoader] {method.GetFullMethodName()} was skipped because the constructor for type {type.Name} threw an exception:", ex);
                                continue;
                            }
                        }

                        hotkeyActionDefinition = new(
                            groupInst,
                            method,
                            actionAttribute.UseExactName ? actionAttribute.Name : Regex.Replace(actionAttribute.Name, "\\B([A-Z])", " $1"),
                            actionAttribute.Description,
                            actionAttribute.GroupNameOverride ?? actionGroupAttribute.GroupName,
                            actionGroupColorString == null ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(actionGroupColorString)),
                            actionSettingDefs.ToArray());
                    }
                    else
                    { // static method
                        hotkeyActionDefinition = new(
                            type,
                            method,
                            actionAttribute.UseExactName ? actionAttribute.Name : Regex.Replace(actionAttribute.Name, "\\B([A-Z])", " $1"),
                            actionAttribute.Description,
                            actionAttribute.GroupNameOverride ?? actionGroupAttribute.GroupName,
                            actionGroupColorString == null ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(actionGroupColorString)),
                            actionSettingDefs.ToArray());
                    }

                    l.Add(hotkeyActionDefinition);
                    ++loadedActionsFromTypeCount;

                    if (FLog.Log.FilterEventType(EventType.TRACE))
                    {
                        List<object?> lines = new();

                        var lineHeader = "[ActionAddonLoader] ";
                        lines.Add($"{lineHeader}Loaded {method.GetFullMethodName()}.");
                        lineHeader = new string(' ', lineHeader.Length);
                        for (int k = 0, k_max = actionSettingDefs.Count; k < k_max; ++k)
                        {
                            var settingDef = actionSettingDefs[k];

                            lines.Add($"{lineHeader}+ Setting \"{settingDef.Name}\"{StringHelper.IndentWithPattern(30, settingDef.Name.Length)}({settingDef.ValueType.FullName})");
                        }

                        FLog.Log.Trace(lines.ToArray());
                    }
                } //< enumerate public methods

                if (FLog.Log.FilterEventType(EventType.DEBUG))
                    FLog.Log.Debug($"[ActionAddonLoader] Loaded {loadedActionsFromTypeCount}{(loadedActionsFromTypeCount == methodsWithActionAttrCount ? "" : $"/{methodsWithActionAttrCount}")} actions from {type.FullName}");
            } //< enumerate public types

            if (FLog.Log.FilterEventType(EventType.DEBUG))
                FLog.Log.Debug($"[ActionAddonLoader] Loaded {l.Count} total actions from {typesWithGroupAttrCount} action groups.");

            return l.ToArray();
        }
        #endregion LoadActions
    }
}
