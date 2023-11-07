using System.Collections;
using System.Windows;
using VolumeControl.Core.Attributes;

namespace VolumeControl.Core
{
    /// <summary>
    /// Provides a <see cref="DataTemplate"/> instance constructed in codebehind using <see cref="FrameworkElementFactory"/>.
    /// </summary>
    /// <remarks>
    /// Any public, non-static, non-abstract class that implements this interface can be specified for <see cref="HotkeyActionSettingAttribute.DataTemplateProviderType"/>.
    /// </remarks>
    /// <example>
    /// To create an <see cref="ITemplateProvider"/>, first create a new class that implements it:
    /// <code>
    /// public class IntDataTemplateProvider : IDataTemplateProvider
    /// {
    ///     public DataTemplate ProvideDataTemplate()
    ///     {
    ///         // Create a TextBox (via a factory) and bind its Text property to the action setting's Value property:
    ///         var textBoxFactory = new FrameworkElementFactory(typeof(TextBox));
    ///         textBoxFactory.SetBinding(TextBox.TextProperty, new Binding("Value"));
    ///         
    ///         return new DataTemplate(typeof(int)) { VisualTree = textBoxFactory };
    ///     }
    /// }
    /// </code>
    /// 
    /// You would then specify the <c>IntDataTemplateProvider</c> type in your <see cref="HotkeyActionSettingAttribute"/>:
    /// <code>
    /// [HotkeyActionSetting("SomeSetting", typeof(int), typeof(IntDataTemplateProvider))]
    /// </code>
    /// </example>
    public interface ITemplateProvider
    {
        /// <summary>
        /// Checks if this <see cref="ITemplateProvider"/> instance can provide a <see cref="DataTemplate"/> for the specified <paramref name="valueType"/>.
        /// </summary>
        /// <param name="valueType">The value type of the action setting to display in the template.</param>
        /// <returns><see langword="true"/> when this instance supports the <paramref name="valueType"/>; otherwise <see langword="false"/>.</returns>
        bool CanProvideDataTemplate(Type valueType);

        /// <summary>
        /// Gets the <see cref="DataTemplate"/> instance to use for displaying an editor control for the action setting's value.
        /// </summary>
        /// <remarks>
        /// The returned <see cref="DataTemplate"/> will always be used with a DataContext of the <see cref="Input.Actions.Settings.IActionSettingInstance"/> instance for that action setting.
        /// Therefore, valid data binding paths can be relative to an <see cref="Input.Actions.Settings.IActionSettingInstance"/> instance, or absolute by directly specifying a binding Source.
        /// <br/>
        /// A data binding path of "<see langword="Value"/>" will return the value instance of type <see cref="HotkeyActionSettingAttribute.ValueType"/>.
        /// <br/>
        /// Return <see langword="null"/> to allow another data template to be selected.
        /// </remarks>
        /// <param name="valueType">The value type of the action setting to display in the template.</param>
        /// <returns>A new <see cref="DataTemplate"/> instance for the specified <paramref name="valueType"/> when successful; otherwise <see langword="null"/>.</returns>
        DataTemplate? ProvideDataTemplate(Type valueType);
    }
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
