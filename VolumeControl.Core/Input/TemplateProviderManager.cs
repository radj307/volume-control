using System.Windows;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Helpers;
using VolumeControl.Core.Input.Exceptions;
using VolumeControl.Log;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Manages <see cref="ITemplateProvider"/> &amp; <see cref="ITemplateDictionaryProvider"/> instances to provide 
    ///  action settings with a suitable value editor <see cref="DataTemplate"/>.
    /// </summary>
    public sealed class TemplateProviderManager
    {
        #region Properties
        /// <summary>
        /// Gets the list of <see cref="ITemplateProvider"/> instances.
        /// </summary>
        public IReadOnlyList<ITemplateProvider> TemplateProviders => _templateProviders;
        private readonly List<ITemplateProvider> _templateProviders = new();
        /// <summary>
        /// Gets the list of <see cref="ITemplateDictionaryProvider"/> instances.
        /// </summary>
        public IReadOnlyList<ITemplateDictionaryProvider> TemplateDictionaryProviders => _templateDictionaryProviders;
        private readonly List<ITemplateDictionaryProvider> _templateDictionaryProviders = new();
        /// <summary>
        /// Gets the list of types that failed to initialize for any reason.<br/>
        /// If initialization of a provider type fails, it is added to this list and not attempted again.
        /// </summary>
        public IReadOnlySet<Type> FailedTypes => _failedTypes;
        private readonly HashSet<Type> _failedTypes = new();
        #endregion Properties

        #region Methods

        #region IndexOfProviderType
        /// <summary>
        /// Gets the index of the <see cref="ITemplateProvider"/> type specified by <paramref name="providerType"/> in TemplateProviders.
        /// </summary>
        /// <param name="providerType">A type that implements <see cref="ITemplateProvider"/>.</param>
        /// <returns>The index of the provider with the specified <paramref name="providerType"/> if found; otherwise -1.</returns>
        /// <exception cref="InvalidProviderTypeException"><paramref name="providerType"/> does not implement <see cref="ITemplateProvider"/>.</exception>
        public int IndexOfProviderType(Type providerType)
        {
            if (!providerType.IsAssignableTo(typeof(ITemplateProvider)))
                throw new InvalidProviderTypeException(providerType, $"Type \"{providerType}\" is not a valid provider type because it does not implement {typeof(ITemplateProvider)}!", new ArgumentException(null, nameof(providerType)));

            for (int i = 0, i_max = TemplateProviders.Count; i < i_max; ++i)
            {
                if (TemplateProviders[i].GetType().Equals(providerType))
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// Gets the index of the <see cref="ITemplateDictionaryProvider"/> type specified by <paramref name="dictionaryProviderType"/> in DictionaryProviders.
        /// </summary>
        /// <param name="dictionaryProviderType">A type that implements <see cref="ITemplateDictionaryProvider"/>.</param>
        /// <returns>The index of the dictionary provider with the specified <paramref name="dictionaryProviderType"/> if found; otherwise -1.</returns>
        /// <exception cref="InvalidProviderTypeException"><paramref name="dictionaryProviderType"/> does not implement <see cref="ITemplateDictionaryProvider"/>.</exception>
        public int IndexOfDictionaryProviderType(Type dictionaryProviderType)
        {
            if (!dictionaryProviderType.IsAssignableTo(typeof(ITemplateDictionaryProvider)))
                throw new InvalidProviderTypeException(dictionaryProviderType, $"Type \"{dictionaryProviderType}\" is not a valid provider type because it does not implement {typeof(ITemplateDictionaryProvider)}!", new ArgumentException(null, nameof(dictionaryProviderType)));

            for (int i = 0, i_max = TemplateDictionaryProviders.Count; i < i_max; ++i)
            {
                if (TemplateDictionaryProviders[i].GetType().Equals(dictionaryProviderType))
                    return i;
            }
            return -1;
        }
        #endregion IndexOfProviderType

        #region TryCreateProvider
        /// <summary>
        /// Attempts to create a new instance of the specified <paramref name="providerType"/>.
        /// </summary>
        /// <param name="providerType">A type that implements <see cref="ITemplateProvider"/>.</param>
        /// <param name="provider">The newly-created <see cref="ITemplateProvider"/> instance when successful; otherwise <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when successful and <paramref name="provider"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        private bool TryCreateProvider(Type providerType, out ITemplateProvider provider)
        {
            try
            {
                provider = (ITemplateProvider)Activator.CreateInstance(providerType)!;
            }
            catch (Exception ex)
            {
                FLog.Error($"[{nameof(TemplateProviderManager)}] Failed to initialize {nameof(ITemplateProvider)} type \"{providerType}\" due to an exception:", ex);

                _failedTypes.Add(providerType);
                provider = null!;
                return false;
            }

            if (provider == null)
            {
                FLog.Error($"[{nameof(TemplateProviderManager)}] Failed to initialize {nameof(ITemplateProvider)} type \"{providerType}\" due to an unknown error!");

                _failedTypes.Add(providerType);
                return false;
            }
            else if (FLog.FilterEventType(Log.Enum.EventType.TRACE))
            {
                FLog.Trace($"[{nameof(TemplateProviderManager)}] Successfully initialized {nameof(ITemplateProvider)} type \"{providerType}\".");
            }

            return true;
        }
        /// <summary>
        /// Attempts to create a new instance of the specified <paramref name="dictionaryProviderType"/>.
        /// </summary>
        /// <param name="dictionaryProviderType">A type that implements <see cref="ITemplateDictionaryProvider"/>.</param>
        /// <param name="dictionaryProvider">The newly-created <see cref="ITemplateDictionaryProvider"/> instance when successful; otherwise <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when successful and <paramref name="dictionaryProvider"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        private bool TryCreateDictionaryProvider(Type dictionaryProviderType, out ITemplateDictionaryProvider dictionaryProvider)
        {
            try
            {
                dictionaryProvider = (ITemplateDictionaryProvider)Activator.CreateInstance(dictionaryProviderType)!;
            }
            catch (Exception ex)
            {
                FLog.Error($"[{nameof(TemplateProviderManager)}] Failed to initialize {nameof(ITemplateDictionaryProvider)} type \"{dictionaryProviderType}\" due to an exception:", ex);

                _failedTypes.Add(dictionaryProviderType);
                dictionaryProvider = null!;
                return false;
            }

            if (dictionaryProvider == null)
            {
                FLog.Error($"[{nameof(TemplateProviderManager)}] Failed to initialize {nameof(ITemplateDictionaryProvider)} type \"{dictionaryProviderType}\" due to an unknown error!");
                _failedTypes.Add(dictionaryProviderType);
                return false;
            }
            else if (FLog.FilterEventType(Log.Enum.EventType.TRACE))
            {
                FLog.Trace($"[{nameof(TemplateProviderManager)}] Successfully initialized {nameof(ITemplateDictionaryProvider)} type \"{dictionaryProviderType}\".");
            }

            return true;
        }
        #endregion TryCreateProvider

        #region TryGetProvider
        /// <summary>
        /// Attempts to get the <paramref name="provider"/> instance with the specified <paramref name="providerType"/>.
        /// </summary>
        /// <remarks>
        /// If no provider was found, a new instance is created. This method can fail if the provider initializer fails.
        /// </remarks>
        /// <param name="providerType">A type that implements <see cref="ITemplateProvider"/>.</param>
        /// <param name="provider">The requested <see cref="ITemplateProvider"/> instance when successful; otherwise <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when successful and <paramref name="provider"/> is not <see langword="null"/>; <see langword="false"/> when the provider failed.</returns>
        /// <exception cref="InvalidProviderTypeException"><paramref name="providerType"/> does not implement <see cref="ITemplateProvider"/>.</exception>
        public bool TryGetProvider(Type providerType, out ITemplateProvider provider)
        {
            if (!providerType.IsAssignableTo(typeof(ITemplateProvider)))
                throw new InvalidProviderTypeException(providerType, $"Type \"{providerType}\" is not a valid provider type because it does not implement {typeof(ITemplateProvider)}!", new ArgumentException(null, nameof(providerType)));
            else if (FailedTypes.Contains(providerType))
            {
                provider = null!;
                return false;
            }

            var existingIndex = IndexOfProviderType(providerType);

            if (existingIndex != -1)
            {
                provider = TemplateProviders[existingIndex];
                return provider != null;
            }
            else if (TryCreateProvider(providerType, out provider))
            {
                _templateProviders.Add(provider);
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Attempts to get the <paramref name="dictionaryProvider"/> instance with the specified <paramref name="dictionaryProviderType"/>.
        /// </summary>
        /// <remarks>
        /// If no provider was found, a new instance is created. This method can fail if the provider initializer fails.
        /// </remarks>
        /// <param name="dictionaryProviderType">A type that implements <see cref="ITemplateDictionaryProvider"/>.</param>
        /// <param name="dictionaryProvider">The requested <see cref="ITemplateDictionaryProvider"/> instance when successful; otherwise <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when successful and <paramref name="dictionaryProvider"/> is not <see langword="null"/>; <see langword="false"/> when the provider failed.</returns>
        /// <exception cref="InvalidProviderTypeException"><paramref name="dictionaryProviderType"/> does not implement <see cref="ITemplateDictionaryProvider"/>.</exception>
        public bool TryGetDictionaryProvider(Type dictionaryProviderType, out ITemplateDictionaryProvider dictionaryProvider)
        {
            if (!dictionaryProviderType.IsAssignableTo(typeof(ITemplateDictionaryProvider)))
                throw new InvalidProviderTypeException(dictionaryProviderType, $"Type \"{dictionaryProviderType}\" is not a valid provider type because it does not implement {typeof(ITemplateDictionaryProvider)}!", new ArgumentException(null, nameof(dictionaryProviderType)));
            else if (FailedTypes.Contains(dictionaryProviderType))
            {
                dictionaryProvider = null!;
                return false;
            }

            var existingIndex = IndexOfDictionaryProviderType(dictionaryProviderType);

            if (existingIndex != -1)
            {
                dictionaryProvider = TemplateDictionaryProviders[existingIndex];
                return dictionaryProvider != null;
            }
            else if (TryCreateDictionaryProvider(dictionaryProviderType, out dictionaryProvider))
            {
                if (dictionaryProvider is ResourceDictionary resourceDictionary)
                {

                }

                _templateDictionaryProviders.Add(dictionaryProvider);
                return true;
            }
            else return false;
        }
        #endregion TryGetProvider

        #region FindDataTemplate
        /// <summary>
        /// Finds the <see cref="DataTemplate"/> with the specified <paramref name="key"/> in the TemplateDictionaryProviders list.
        /// </summary>
        /// <param name="key">The key name of the target <see cref="DataTemplate"/> instance.</param>
        /// <returns>The <see cref="ActionSettingDataTemplate"/> instance with the specified <paramref name="key"/> when found; otherwise <see langword="null"/>.</returns>
        public DataTemplate? FindDataTemplate(string key)
        {
            // search dictionary providers for a data template
            for (int i = TemplateDictionaryProviders.Count - 1; i >= 0; --i)
            {
                var dictionaryProvider = TemplateDictionaryProviders[i];

                if (dictionaryProvider.ProvideDataTemplate(key) is ActionSettingDataTemplate actionSettingDataTemplate)
                {
                    // write trace log message
                    if (FLog.FilterEventType(Log.Enum.EventType.TRACE))
                        FLog.Trace($"[{nameof(TemplateProviderManager)}] Found DataTemplate with key \"{key}\" in {nameof(ITemplateDictionaryProvider)} type \"{dictionaryProvider.GetType()}\".");

                    return actionSettingDataTemplate;
                }
            }

            // write trace log message
            if (FLog.FilterEventType(Log.Enum.EventType.TRACE))
                FLog.Trace($"[{nameof(TemplateProviderManager)}] Couldn't find any DataTemplates with key \"{key}\".");

            return null;
        }
        /// <summary>
        /// Finds the most recent <see cref="DataTemplate"/> instance that supports the specified <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The type of value displayed in the data template.</param>
        /// <returns>The most recently-added <see cref="ActionSettingDataTemplate"/> instance that supports <paramref name="valueType"/> when found; otherwise <see langword="null"/>.</returns>
        public DataTemplate? FindDataTemplate(Type valueType)
        {
            // search providers for a data template
            for (int i = TemplateProviders.Count - 1; i >= 0; --i)
            {
                var provider = TemplateProviders[i];

                if (!provider.CanProvideDataTemplate(valueType)) continue;

                if (provider.ProvideDataTemplate(valueType) is ActionSettingDataTemplate actionSettingDataTemplate)
                {
                    // write trace log message
                    if (FLog.FilterEventType(Log.Enum.EventType.TRACE))
                        FLog.Trace($"[{nameof(TemplateProviderManager)}] Found DataTemplate for value type \"{valueType}\" in {nameof(ITemplateProvider)} type \"{provider.GetType()}\".");

                    return actionSettingDataTemplate;
                }
            }

            // search dictionary providers for a data template
            for (int i = TemplateDictionaryProviders.Count - 1; i >= 0; --i)
            {
                var dictionaryProvider = TemplateDictionaryProviders[i];

                if (dictionaryProvider.ProvideDataTemplate(valueType) is ActionSettingDataTemplate actionSettingDataTemplate)
                {
                    // write trace log message
                    if (FLog.FilterEventType(Log.Enum.EventType.TRACE))
                        FLog.Trace($"[{nameof(TemplateProviderManager)}] Found DataTemplate for value type \"{valueType}\" in {nameof(ITemplateDictionaryProvider)} type \"{dictionaryProvider.GetType()}\".");

                    return actionSettingDataTemplate;
                }
            }

            // write trace log message
            if (FLog.FilterEventType(Log.Enum.EventType.TRACE))
                FLog.Trace($"[{nameof(TemplateProviderManager)}] Couldn't find any DataTemplates for value type \"{valueType}\".");

            return null;
        }
        #endregion FindDataTemplate

        #region FindDataTemplateFor
        /// <summary>
        /// Defines the cases when the <see cref="FindDataTemplateFor(Type?, string?, Type, FallbackMode)"/> method will fall back to fall back to providers that weren't explicitly specified.
        /// </summary>
        [Flags]
        public enum FallbackMode
        {
            /// <summary>
            /// Never fallback to templates that weren't specified.<br/>
            /// Only searches all providers when no provider was specified.
            /// </summary>
            /// <remarks>
            /// It is recommended that <see cref="OnNullTemplate"/> is used instead of this, 
            ///  as some providers are designed to return null to fallback to other providers.
            /// </remarks>
            Never = 0,
            /// <summary>
            /// Fallback to other providers when the specified provider returned a null template.
            /// </summary>
            OnNullTemplate = 1,
            /// <summary>
            /// Fallback to other providers when the specified template key wasn't found.
            /// </summary>
            OnMissingKey = 2,
            /// <summary>
            /// Fallback to other providers when the specified provider failed to initialize.
            /// </summary>
            OnFailedProvider = 4,
            /// <summary>
            /// Always fallback to other providers.
            /// </summary>
            Always = OnNullTemplate | OnMissingKey | OnFailedProvider,
        }
        /// <summary>
        /// Finds a suitable <see cref="DataTemplate"/> instance with the specified parameters.
        /// </summary>
        /// <param name="providerType">Optional type that implements </param>
        /// <param name="templateKey">Optional key for a specific <see cref="ActionSettingDataTemplate"/> instance in a <see cref="ITemplateDictionaryProvider"/>.<br/>When <paramref name="providerType"/> is <see langword="null"/></param>
        /// <param name="valueType">The type of value that will be displayed in the <see cref="DataTemplate"/>.</param>
        /// <param name="fallbackMode">The <see cref="FallbackMode"/> to use when searching for a template.</param>
        /// <returns><see cref="DataTemplate"/> instance for the specified parameters when successful; otherwise <see langword="null"/>.</returns>
        /// <exception cref="InvalidProviderTypeException"><paramref name="providerType"/> does not implement <see cref="ITemplateProvider"/> or <see cref="ITemplateDictionaryProvider"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueType"/> was <see langword="null"/> and the <paramref name="providerType"/> and <paramref name="templateKey"/> didn't resolve to a valid template.</exception>
        public DataTemplate? FindDataTemplateFor(Type? providerType, string? templateKey, Type valueType, FallbackMode fallbackMode)
        {
            bool traceMessagesAreEnabled = FLog.FilterEventType(Log.Enum.EventType.TRACE);

            // search specified provider for a data template
            if (providerType != null && !FailedTypes.Contains(providerType))
            {
                if (providerType.IsAssignableTo(typeof(ITemplateProvider)))
                { // provider
                    if (templateKey != null && FLog.FilterEventType(Log.Enum.EventType.WARN))
                    { // [WARN] a key name was set, but the specified provider type doesn't support keys
                        FLog.Warning(
                            $"[{nameof(TemplateProviderManager)}] A {nameof(HotkeyActionSettingAttribute.DataTemplateProviderKey)} (\"{templateKey}\") was specified, but {nameof(HotkeyActionSettingAttribute.DataTemplateProviderType)} is {nameof(ITemplateProvider)} type \"{providerType}\"!",
                            $"Template providers that implement {nameof(ITemplateProvider)} do not support specifying a {nameof(HotkeyActionSettingAttribute.DataTemplateProviderKey)}.");
                    }

                    // get the provider instance of the specified type
                    if (TryGetProvider(providerType, out var provider))
                    {
                        if (provider.CanProvideDataTemplate(valueType))
                        {
                            var providedTemplate = provider.ProvideDataTemplate(valueType);
                            if (providedTemplate is DataTemplate template)
                            {
                                if (traceMessagesAreEnabled)
                                    FLog.Trace($"[{nameof(TemplateProviderManager)}] Retrieved DataTemplate for value type \"{valueType}\" from provider type \"{provider.GetType()}\"");
                                return template;
                            }
                            else if (!fallbackMode.HasFlag(FallbackMode.OnNullTemplate))
                            {
                                if (traceMessagesAreEnabled)
                                    FLog.Trace($"[{nameof(TemplateProviderManager)}] Provider \"{providerType}\" returned null and fallback is disabled.");
                                return null;
                            }
                            //< else fallthrough
                        }
                        else
                        { // [DEBUG] the specified provider does not support this value type
                            if (FLog.FilterEventType(Log.Enum.EventType.DEBUG))
                                FLog.Debug($"[{nameof(TemplateProviderManager)}] {nameof(ITemplateProvider)} \"{providerType}\" does not support value type \"{valueType}\" ({StringHelper.GetFullMethodName(providerType.GetMethod(nameof(provider.CanProvideDataTemplate))!)} returned false)! Falling back to other providers.");
                        }
                        //< fallthrough
                    }
                    else if (!fallbackMode.HasFlag(FallbackMode.OnFailedProvider))
                    { // [ERROR] the specified provider type failed
                        if (FLog.FilterEventType(Log.Enum.EventType.ERROR))
                            FLog.Error($"[{nameof(TemplateProviderManager)}] {nameof(ITemplateProvider)} \"{providerType}\" failed! (see the failure message above for details)");

                        return null;
                    }
                    //< fallthrough
                }
                else if (providerType.IsAssignableTo(typeof(ITemplateDictionaryProvider)))
                { // dictionary provider
                    if (TryGetDictionaryProvider(providerType, out var dictionaryProvider))
                    {
                        // check if a key was specified
                        if (templateKey != null)
                        {
                            var template = dictionaryProvider.ProvideDataTemplate(templateKey);

                            if (template != null || !fallbackMode.HasFlag(FallbackMode.OnMissingKey))
                                return template;
                            //< fallthrough when template is null && allowFallbackOnMissingKey is true
                        }

                        // fallback to value type within this dictionary provider
                        return dictionaryProvider.ProvideDataTemplate(valueType);
                    }
                    else if (!fallbackMode.HasFlag(FallbackMode.OnFailedProvider))
                    { // [ERROR] the specified dictionary provider type failed
                        if (FLog.FilterEventType(Log.Enum.EventType.ERROR))
                            FLog.Error($"[{nameof(TemplateProviderManager)}] {nameof(ITemplateDictionaryProvider)} \"{providerType}\" failed! (see the failure message above for details)");

                        return null;
                    }
                    //< fallthrough
                }
                else throw InvalidProviderTypeException.DoesNotImplementAnyRequiredInterface(providerType, new ArgumentException(null, nameof(providerType)));
            }
            // search all dictionaries for a data template with the specified key
            else if (templateKey != null)
            {
                var template = FindDataTemplate(templateKey);

                if (template != null || !fallbackMode.HasFlag(FallbackMode.OnMissingKey))
                    return template;
                //< fallthrough when template is null && allowFallbackOnMissingKey is true
            }

            ArgumentNullException.ThrowIfNull(valueType);

            // fallback to searching for a suitable data template in all dictionaries
            return FindDataTemplate(valueType);
        }
        /// <summary>
        /// Finds a suitable <see cref="DataTemplate"/> instance with the specified parameters.
        /// </summary>
        /// <param name="providerType">Optional type that implements </param>
        /// <param name="templateKey">Optional key for a specific <see cref="ActionSettingDataTemplate"/> instance in a <see cref="ITemplateDictionaryProvider"/>.<br/>When <paramref name="providerType"/> is <see langword="null"/></param>
        /// <param name="valueType">The type of value that will be displayed in the <see cref="DataTemplate"/>.</param>
        /// <param name="allowFallbackOnMissingKey">When <see langword="true"/> &amp; <paramref name="templateKey"/> wasn't found and isn't <see langword="null"/>, fallback to searching all dictionaries for a <see cref="DataTemplate"/> that supports the specified <paramref name="valueType"/> instead of returning <see langword="null"/>.</param>
        /// <returns><see cref="DataTemplate"/> instance for the specified parameters when successful; otherwise <see langword="null"/>.</returns>
        /// <exception cref="InvalidProviderTypeException"><paramref name="providerType"/> does not implement <see cref="ITemplateProvider"/> or <see cref="ITemplateDictionaryProvider"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueType"/> was <see langword="null"/> and the <paramref name="providerType"/> and <paramref name="templateKey"/> didn't resolve to a valid template.</exception>
        public DataTemplate? FindDataTemplateFor(Type? providerType, string? templateKey, Type valueType, bool allowFallbackOnMissingKey = false)
        {
            bool showTraceMessages = FLog.FilterEventType(Log.Enum.EventType.TRACE);

            // search specified provider for a data template
            if (providerType != null && !FailedTypes.Contains(providerType))
            {
                if (providerType.IsAssignableTo(typeof(ITemplateProvider)))
                { // provider
                    if (templateKey != null && FLog.FilterEventType(Log.Enum.EventType.WARN))
                    { // [WARN] a key name was set, but the specified provider type doesn't support keys
                        FLog.Warning(
                            $"[{nameof(TemplateProviderManager)}] A {nameof(HotkeyActionSettingAttribute.DataTemplateProviderKey)} (\"{templateKey}\") was specified, but {nameof(HotkeyActionSettingAttribute.DataTemplateProviderType)} is {nameof(ITemplateProvider)} type \"{providerType}\"!",
                            $"Template providers that implement {nameof(ITemplateProvider)} do not support specifying a {nameof(HotkeyActionSettingAttribute.DataTemplateProviderKey)}.");
                    }

                    // get the provider instance of the specified type
                    if (TryGetProvider(providerType, out var provider))
                    {
                        if (provider.CanProvideDataTemplate(valueType))
                        {
                            if (provider.ProvideDataTemplate(valueType) is DataTemplate template)
                            {
                                if (showTraceMessages)
                                    FLog.Trace($"[{nameof(TemplateProviderManager)}] Retrieved DataTemplate for value type \"{valueType}\" from provider type \"{provider.GetType()}\"");
                                return template;
                            }
                            //< fallthrough without issue if the provider returns null; that is allowed
                        }
                        else
                        { // [DEBUG] the specified provider does not support this value type
                            if (FLog.FilterEventType(Log.Enum.EventType.DEBUG))
                                FLog.Debug($"[{nameof(TemplateProviderManager)}] {nameof(ITemplateProvider)} \"{providerType}\" does not support value type \"{valueType}\" ({StringHelper.GetFullMethodName(providerType.GetMethod(nameof(provider.CanProvideDataTemplate))!)} returned false)! Falling back to other providers.");
                        }
                    }
                    else
                    { // [ERROR] the specified provider type failed
                        if (FLog.FilterEventType(Log.Enum.EventType.ERROR))
                            FLog.Error($"[{nameof(TemplateProviderManager)}] {nameof(ITemplateProvider)} \"{providerType}\" failed! (see the failure message above for details)");

                        return null;
                    }
                    //< fallthrough
                }
                else if (providerType.IsAssignableTo(typeof(ITemplateDictionaryProvider)))
                { // dictionary provider
                    if (TryGetDictionaryProvider(providerType, out var dictionaryProvider))
                    {
                        // check if a key was specified
                        if (templateKey != null)
                        {
                            var template = dictionaryProvider.ProvideDataTemplate(templateKey);

                            if (template != null || !allowFallbackOnMissingKey)
                                return template;
                            //< fallthrough when template is null && allowFallbackOnMissingKey is true
                        }

                        // fallback to value type within this dictionary provider
                        return dictionaryProvider.ProvideDataTemplate(valueType);
                    }
                    else
                    { // [ERROR] the specified dictionary provider type failed
                        if (FLog.FilterEventType(Log.Enum.EventType.ERROR))
                            FLog.Error($"[{nameof(TemplateProviderManager)}] {nameof(ITemplateDictionaryProvider)} \"{providerType}\" failed! (see the failure message above for details)");

                        return null;
                    }
                    //< fallthrough
                }
                else throw InvalidProviderTypeException.DoesNotImplementAnyRequiredInterface(providerType, new ArgumentException(null, nameof(providerType)));
            }
            // search all dictionaries for a data template with the specified key
            else if (templateKey != null)
            {
                var template = FindDataTemplate(templateKey);

                if (template != null || !allowFallbackOnMissingKey)
                    return template;
                //< fallthrough when template is null && allowFallbackOnMissingKey is true
            }

            ArgumentNullException.ThrowIfNull(valueType);

            // fallback to searching for a suitable data template in all dictionaries
            return FindDataTemplate(valueType);
        }
        #endregion FindDataTemplateFor

        #region RegisterProvider
        /// <summary>
        /// Registers the specified <paramref name="providerType"/>, if it isn't already registered.
        /// </summary>
        /// <remarks>
        /// Calling this will create a new instance of the <paramref name="providerType"/> and add it to the provider manager.
        /// </remarks>
        /// <param name="providerType">A data template provider type that implements either <see cref="ITemplateProvider"/> or <see cref="ITemplateDictionaryProvider"/>.</param>
        /// <returns><see langword="true"/> when successful; otherwise <see langword="false"/> when <paramref name="providerType"/> has already failed to register.</returns>
        /// <exception cref="InvalidDataTemplateProviderTypeException"><paramref name="providerType"/> is invalid. It may not implement either <see cref="ITemplateProvider"/> or <see cref="ITemplateDictionaryProvider"/>, is abstract, or does not provide a default constructor. See the exception message for details.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="providerType"/> was <see langword="null"/>.</exception>
        internal bool RegisterProvider(Type providerType)
        {
            ArgumentNullException.ThrowIfNull(providerType);

            if (FailedTypes.Contains(providerType))
                return false;

            if (providerType.IsAssignableTo(typeof(ITemplateProvider)))
            {
                //< providerType cannot be static here (static classes cannot implement interfaces)
                bool result = false;
                try
                {
                    result = TryCreateProvider(providerType, out _);
                }
                catch (Exception ex)
                {
                    _failedTypes.Add(providerType);
                    throw new InvalidProviderTypeException(providerType, $"Initialization of {nameof(ITemplateProvider)} type \"{providerType}\" failed!", ex);
                }
                return result;
            }
            else if (providerType.IsAssignableTo(typeof(ITemplateDictionaryProvider)))
            {
                //< providerType cannot be static here (static classes cannot implement interfaces)
                bool result = false;
                try
                {
                    result = TryCreateDictionaryProvider(providerType, out _);
                }
                catch (Exception ex)
                {
                    _failedTypes.Add(providerType);
                    throw new InvalidProviderTypeException(providerType, $"Initialization of {nameof(ITemplateDictionaryProvider)} type \"{providerType}\" failed!", ex);
                }
                return result;
            }
            else throw new InvalidDataTemplateProviderTypeException(providerType, new ArgumentException(null, nameof(providerType)));
        }
        #endregion RegisterProvider

        #region GetProviderResourceDictionaries
        /// <summary>
        /// Gets all of the <see cref="ResourceDictionary"/> objects from the list of template dictionary providers.
        /// </summary>
        /// <remarks>
        /// Not all <see cref="ITemplateDictionaryProvider"/> instances are derived from <see cref="ResourceDictionary"/>,
        ///  so the number of returned objects may be different.
        /// </remarks>
        /// <returns>An array of all of the loaded <see cref="ResourceDictionary"/> objects.</returns>
        public ResourceDictionary[] GetProviderResourceDictionaries()
        {
            List<ResourceDictionary> l = new();

            foreach (var dictionaryProvider in TemplateDictionaryProviders)
            {
                if (dictionaryProvider is ResourceDictionary resourceDictionary)
                {
                    l.Add(resourceDictionary);
                }
            }

            return l.ToArray();
        }
        #endregion GetProviderResourceDictionaries

        #endregion Methods
    }
}
