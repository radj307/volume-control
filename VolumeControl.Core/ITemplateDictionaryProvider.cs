using System.Collections;
using System.Windows;
using VolumeControl.Core.Attributes;

namespace VolumeControl.Core
{
    /// <summary>
    /// An alternative to <see cref="ITemplateProvider"/> that can be combined with a <see cref="ResourceDictionary"/>
    ///  (with a codebehind) to define many data templates in XAML and provide them for action settings based on a key
    ///  <see cref="string"/> and/or value type.<br/><br/>
    ///  XAML-defined data templates must be type <see cref="ActionSettingDataTemplate"/>, not <see cref="DataTemplate"/>.
    ///  <br/>Set <see cref="ActionSettingDataTemplate.ValueType"/> to the type of value that the data template supports.
    /// </summary>
    /// <remarks>
    /// Any public, non-static, non-abstract class that implements this interface can be specified for <see cref="HotkeyActionSettingAttribute.DataTemplateProviderType"/>.
    /// </remarks>
    public interface ITemplateDictionaryProvider : IEnumerable
    {
        /// <summary>
        /// Provides the <see cref="ActionSettingDataTemplate"/> instance with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key name of the target <see cref="DataTemplate"/> instance.</param>
        /// <returns><see cref="DataTemplate"/> instance with the specified <paramref name="key"/> if it exists; otherwise <see langword="null"/>.</returns>
        ActionSettingDataTemplate? ProvideDataTemplate(string key);
        /// <summary>
        /// Provides an <see cref="ActionSettingDataTemplate"/> for the specified <paramref name="valueType"/>.
        /// </summary>
        /// <remarks>
        /// This method will not return templates where <see cref="ActionSettingDataTemplate.IsExplicit"/> is <see langword="true"/>.
        /// </remarks>
        /// <param name="valueType">The value type to get a <see cref="DataTemplate"/> for.</param>
        /// <returns>An <see cref="ActionSettingDataTemplate"/> instance for the specified <paramref name="valueType"/> if one was found; otherwise <see langword="null"/>.</returns>
        ActionSettingDataTemplate? ProvideDataTemplate(Type valueType);
        /// <summary>
        /// Gets the contents of the dictionary as an enumerable collection of <see cref="DictionaryEntry"/> instances.
        /// </summary>
        /// <returns>An enumerable collection of all of the <see cref="DictionaryEntry"/> pairs in this instance.</returns>
        IEnumerable<DictionaryEntry> AsEnumerable();
    }
}
