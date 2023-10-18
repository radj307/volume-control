using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VolumeControl.Core.Attributes;
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
                FLog.Log.Warning($"[ActionMethodValidator] {method.GetFullName()} returns type {method.ReturnType.Name}; expected type {typeof(void).Name}. The returned value will be ignored.");
                state = EMethodValidationState.Warning;
            }

            var parameters = method.GetParameters();

            // NO PARAMETERS
            if (parameters.Length == 0)
            {
                FLog.Log.Error($"[ActionMethodValidator] {method.GetFullName()} is invalid because it doesn't have enough parameters.",
                    $"  Expected:  object? sender, {typeof(HotkeyActionPressedEventArgs).Name} e");
                state = EMethodValidationState.Error;
            }
            // PARAMETER 0 "sender"
            if (parameters.Length >= 1)
            {
                var param = parameters[0];
                var paramType = param.ParameterType;
                if (paramType != typeof(object) && paramType != typeof(Hotkey) && paramType != typeof(IHotkey))
                {
                    FLog.Log.Error($"[ActionMethodValidator] {method.GetFullName()} is invalid because parameter 0 is incorrect. (Invalid Type)",
                        $"  Expected:  {typeof(object).FullName} sender",
                        $"  Actual:    {paramType.FullName} {param.Name}");
                    state = EMethodValidationState.Error;
                }
            }
            else
            {
                FLog.Log.Error($"[ActionMethodValidator] {method.GetFullName()} is invalid because parameter 0 is missing.",
                    $"  Expected:  {typeof(object).FullName} sender");
            }
            // PARAMETER 1 "e"
            if (parameters.Length >= 2)
            {
                var param = parameters[1];
                var paramType = param.ParameterType;
                if (paramType != typeof(HotkeyActionPressedEventArgs) && paramType != typeof(System.ComponentModel.HandledEventArgs) && paramType != typeof(EventArgs))
                {
                    FLog.Log.Error($"[ActionMethodValidator] {method.GetFullName()} is invalid because parameter 1 is incorrect. (Invalid Type)",
                        $"  Expected:  {typeof(HotkeyActionPressedEventArgs).FullName} e",
                        $"  Actual:    {paramType.FullName} {param.Name}");
                    state = EMethodValidationState.Error;
                }
            }
            else
            {
                FLog.Log.Error($"[ActionMethodValidator] {method.GetFullName()} is invalid because parameter 1 is missing.",
                    $"  Expected:  {typeof(HotkeyActionPressedEventArgs).FullName} e");
            }
            // EXTRA PARAMETERS
            if (parameters.Length > 2)
            {
                FLog.Log.Error($"[ActionMethodValidator] {method.GetFullName()} is invalid because it has too many parameters.",
                    $"Expected:  {method.Name}(object? sender, {typeof(HotkeyActionPressedEventArgs).Name} e)",
                    $"Actual:    {method.GetFullName()}");
            }

            return state;
        }
        #endregion ValidateMethodIsEligibleAsAction

        #region GetFullName
        [Flags]
        public enum MethodNamePart
        {
            /// <summary>
            /// Shows only the declaring type name and the method name, with empty brackets.
            /// </summary>
            None = 0,
            /// <summary>
            /// Shows namespace qualifiers for all types.
            /// </summary>
            FullTypeNames = 1,
            /// <summary>
            /// Shows namespace qualifiers for the declaring type.
            /// </summary>
            FullDeclaringTypeName = 2,
            /// <summary>
            /// Shows generic type parameters.
            /// </summary>
            GenericParameters = 4,
            /// <summary>
            /// Shows parameter types.
            /// </summary>
            ParameterTypes = 8,
            /// <summary>
            /// Shows parameter names.
            /// </summary>
            ParameterNames = 16,
            /// <summary>
            /// Shows parameter types and names.
            /// </summary>
            Parameters = ParameterTypes | ParameterNames,
            /// <summary>
            /// Shows the return type, but only if it isn't <see cref="void"/>.
            /// </summary>
            NonVoidReturnType = 32,
            /// <summary>
            /// Shows the return type, even if it is <see cref="void"/>.
            /// </summary>
            VoidReturnType = 64,
            /// <summary>
            /// Shows the return type.
            /// </summary>
            ReturnType = NonVoidReturnType | VoidReturnType,
        }
        public static string GetFullName(this MethodInfo method, MethodNamePart includedNameComponents)
        {
            string fullName = string.Empty;
            bool showFullTypeNames = includedNameComponents.HasFlag(MethodNamePart.FullTypeNames);

            // type name
            if (method.DeclaringType != null)
            {
                fullName += showFullTypeNames || includedNameComponents.HasFlag(MethodNamePart.FullDeclaringTypeName)
                    ? method.DeclaringType.FullName
                    : method.DeclaringType.Name;
                fullName += '.';
            }
            // method name
            fullName += method.Name;
            // generic parameters
            if (method.IsGenericMethod && includedNameComponents.HasFlag(MethodNamePart.GenericParameters))
            {
                // generic opening bracket
                fullName += '<';
                // generics
                var genericParams = method.GetGenericMethodDefinition().GetGenericArguments();
                for (int i = 0, max = genericParams.Length; i < max; ++i)
                {
                    // generic name
                    fullName += genericParams[i].Name;

                    // separator
                    if (i + 1 < max)
                        fullName += ", ";
                }
                // generic closing bracket
                fullName += '>';
            }
            // opening bracket
            fullName += '(';
            // parameters
            bool showParamTypes = includedNameComponents.HasFlag(MethodNamePart.ParameterTypes);
            bool showParamNames = includedNameComponents.HasFlag(MethodNamePart.ParameterNames);
            if (showParamTypes || showParamNames)
            {
                var parameters = method.GetParameters();
                for (int i = 0, max = parameters.Length; i < max; ++i)
                {
                    // parameter type
                    if (showParamTypes)
                    {
                        var paramType = parameters[i].ParameterType;
                        fullName += showFullTypeNames ? paramType.FullName : paramType.Name;
                    }
                    // parameter name
                    if (showParamNames)
                    {
                        if (showParamTypes)
                            fullName += ' ';
                        fullName += parameters[i].Name;
                    }
                    // separator
                    if (i + 1 < max)
                        fullName += ", ";
                }
            }
            // closing bracket
            fullName += ')';
            // return type
            bool showNonVoidReturnType = includedNameComponents.HasFlag(MethodNamePart.NonVoidReturnType);
            bool showVoidReturnType = includedNameComponents.HasFlag(MethodNamePart.VoidReturnType);
            bool returnTypeIsVoid = method.ReturnType.Equals(typeof(void));
            if ((showNonVoidReturnType && !returnTypeIsVoid) || (showVoidReturnType && returnTypeIsVoid))
            {
                fullName += " => ";
                fullName += showFullTypeNames ? method.ReturnType.FullName : method.ReturnType.Name;
            }

            return fullName;
        }
        public static string GetFullName(this MethodInfo method)
            => GetFullName(method, MethodNamePart.FullDeclaringTypeName | MethodNamePart.GenericParameters | MethodNamePart.Parameters | MethodNamePart.NonVoidReturnType);
        #endregion GetFullName

        /// <summary>
        /// Loads hotkey actions from the specified <paramref name="types"/>.
        /// </summary>
        /// <param name="types">Any number of <see cref="Type"/> instances that represent hotkey action group types.</param>
        /// <returns>List of <see cref="HotkeyActionDefinition"/> objects for all publicly-accessible action methods.</returns>
        public static HotkeyActionDefinition[] Load(params Type[] types)
        {
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
                var publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                // if this type doesn't have any public methods, skip it
                if (publicMethods.Length == 0)
                {
                    FLog.Log.Error($"[ActionLoader] {type.FullName} doesn't contain any publicly-accessible methods marked with {typeof(HotkeyActionAttribute).FullName}!");
                    continue;
                }

                object? groupInst = null;
                int loadedActionsFromTypeCount = 0, //< tracks the number of actions that were loaded from the current type
                    methodsWithActionAttrCount = 0; //< tracks the number of methods in the current type with HotkeyActionAttribute

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
                        FLog.Log.Error($"[ActionLoader] {method.GetFullName()} was skipped because it is invalid.");
                        continue;
                    }

                    // get the action setting definitions for this method
                    var actionSettingDefs = method.GetCustomAttributes<HotkeyActionSettingAttribute>().Select(attr => (ActionSettingDefinition)attr).ToArray();

                    // make sure there aren't any duplicate action setting names
                    var distinctSettingNames = actionSettingDefs.Select(d => d.Name).Distinct();
                    if (distinctSettingNames.Count() != actionSettingDefs.Length)
                    {
                        string duplicateNames = string.Join(", ", actionSettingDefs
                            .GroupBy(d => d.Name)
                            .Where(g => g.Count() > 1)
                            .Select(g => $"\"{g.Key}\""));
                        FLog.Log.Error($"[ActionLoader] {method.GetFullName()} was skipped because multiple settings have the same name: {duplicateNames}!");
                        continue;
                    }

                    var actionGroupColorString = actionAttribute.GroupColorOverride ?? actionGroupAttribute.GroupColor;

                    HotkeyActionDefinition? hotkeyActionDefinition;
                    if (!method.IsStatic)
                    { // non-static method
                        try
                        { // create an instance of this type if one doesn't exist yet:
                            groupInst ??= Activator.CreateInstance(type)!;
                        }
                        catch (Exception ex)
                        {
                            FLog.Log.Error($"[ActionLoader] {method.GetFullName()} was skipped because constructor of type {type.Name} threw an exception:", ex);
                            continue;
                        }

                        hotkeyActionDefinition = new(
                            groupInst,
                            method,
                            actionAttribute.UseExactName ? actionAttribute.Name : Regex.Replace(actionAttribute.Name, "\\B([A-Z])", " $1"),
                            actionAttribute.Description,
                            actionAttribute.GroupNameOverride ?? actionGroupAttribute.GroupName,
                            actionGroupColorString == null ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(actionGroupColorString)),
                            actionSettingDefs);
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
                            actionSettingDefs);
                    }

                    l.Add(hotkeyActionDefinition);
                    ++loadedActionsFromTypeCount;

                    if (FLog.Log.FilterEventType(Log.Enum.EventType.TRACE))
                        FLog.Log.Trace($"[ActionLoader] Loaded {method.GetFullName()}.");
                } //< enumerate public methods

                if (FLog.Log.FilterEventType(Log.Enum.EventType.DEBUG))
                    FLog.Log.Debug($"[ActionLoader] Loaded {loadedActionsFromTypeCount}{(loadedActionsFromTypeCount == methodsWithActionAttrCount ? "" : $"/{methodsWithActionAttrCount}")} actions from {type.FullName}");
            } //< enumerate public types

            if (FLog.Log.FilterEventType(Log.Enum.EventType.DEBUG))
                FLog.Log.Debug($"[ActionLoader] Loaded {l.Count} total actions from {typesWithGroupAttrCount} action groups.");

            return l.ToArray();
        }
    }
}
