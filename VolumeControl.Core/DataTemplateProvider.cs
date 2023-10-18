using System.Windows;
using VolumeControl.Core.Attributes;

namespace VolumeControl.Core
{
    /// <summary>
    /// Provides a <see cref="DataTemplate"/> for some hardcoded type.<br/>
    /// The <see cref="Type"/> of a derived class can be specified for the <see cref="HotkeyActionSettingAttribute.DataTemplateProviderType"/> property to specify which <see cref="DataTemplate"/> will be used to display an editor control for that action setting.
    /// </summary>
    /// <remarks>
    /// To create a <see cref="DataTemplateProvider"/>, first create a new class that implements it:
    /// <code>
    /// public class IntDataTemplateProvider : DataTemplateProvider
    /// {
    ///     public override DataTemplate ProvideDataTemplate()
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
    /// </remarks>
    public abstract class DataTemplateProvider
    {
        /// <summary>
        /// Gets the <see cref="DataTemplate"/> instance to use for displaying an editor control for the action setting's value.
        /// </summary>
        /// <remarks>
        /// The returned <see cref="DataTemplate"/> will always be used with a DataContext of the <see cref="Input.Actions.Settings.IActionSettingInstance"/> instance for that action setting.
        /// Therefore, valid data binding paths can be relative to an <see cref="Input.Actions.Settings.IActionSettingInstance"/> instance, or absolute by directly specifying a binding Source.
        /// <br/>
        /// To bind to the value of the action setting, set the data binding's path to "Value".
        /// </remarks>
        /// <returns>A new <see cref="DataTemplate"/> instance specifying the controls to use for displaying an action setting value editor.</returns>
        public abstract DataTemplate ProvideDataTemplate();
    }
}
