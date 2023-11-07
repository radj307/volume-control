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
}
