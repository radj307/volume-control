using System.Collections;
using System.Windows;

namespace VolumeControl.Core
{
    /// <summary>
    /// Abstract <see cref="ITemplateDictionaryProvider"/> base implementation that inherits from <see cref="ResourceDictionary"/>.
    /// </summary>
    public abstract class ResourceDictionaryTemplateProvider : ResourceDictionary, ITemplateDictionaryProvider
    {
        #region ITemplateDictionaryProvider
        /// <inheritdoc/>
        public virtual ActionSettingDataTemplate? ProvideDataTemplate(string key) => base[key] as ActionSettingDataTemplate;
        /// <inheritdoc/>
        public virtual ActionSettingDataTemplate? ProvideDataTemplate(Type valueType)
        {
            foreach (var (_, value) in this.Cast<DictionaryEntry>())
            {
                if (value is ActionSettingDataTemplate actionSettingDataTemplate && actionSettingDataTemplate.SupportsValueType(valueType))
                {
                    return actionSettingDataTemplate;
                }
            }
            return null;
        }
        /// <inheritdoc/>
        public IEnumerable<DictionaryEntry> AsEnumerable() => this.Cast<DictionaryEntry>();
        #endregion ITemplateDictionaryProvider
    }
}
