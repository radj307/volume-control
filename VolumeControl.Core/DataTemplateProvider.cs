using System.Reflection;
using System.Windows;
using System.Windows.Markup;
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
        #region Abstract Methods
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
        #endregion Abstract Methods

        #region Static Methods
        /// <summary>
        /// Loads the specified XAML embedded resource file and returns the root element.
        /// </summary>
        /// <param name="resourceName">The name of the resource file, including namespace qualifiers.</param>
        /// <param name="assembly">The assembly that contains the embedded resource.</param>
        /// <returns>The root element of the specified embedded resource XAML file.</returns>
        /// <exception cref="EmbeddedResourceNotFoundException">The specified <paramref name="resourceName"/> was <see langword="null"/> or does not exist in the <paramref name="assembly"/>.</exception>
        public static object LoadEmbeddedXamlResource(string resourceName, Assembly assembly)
        {
            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);

                if (stream == null)
                    throw new EmbeddedResourceNotFoundException(resourceName, assembly);

                return XamlReader.Load(stream);
            }
            catch (Exception ex)
            { // rethrow as nested exception
                throw new EmbeddedResourceNotFoundException(resourceName, assembly, ex);
            }
        }
        /// <summary>
        /// Loads a <see cref="DataTemplate"/> from an embedded resource XAML file in the specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="resourceName">The full name (including namespace qualifiers) of an embedded XAML resource file that contains the <see cref="DataTemplate"/> definition.</param>
        /// <param name="assembly">The assembly that contains the embedded XAML resource file.</param>
        /// <returns>The <see cref="DataTemplate"/></returns>
        /// <exception cref="EmbeddedResourceNotFoundException">The specified <paramref name="resourceName"/> was <see langword="null"/> or does not exist in the <paramref name="assembly"/>.</exception>
        /// <exception cref="InvalidOperationException">The specified <paramref name="resourceName"/> does not define a <see cref="DataTemplate"/>.</exception>
        public static DataTemplate FromEmbeddedResource(string resourceName, Assembly assembly)
        {
            var rootElement = LoadEmbeddedXamlResource(resourceName, assembly);

            if (rootElement is DataTemplate dataTemplate)
            {
                return dataTemplate;
            }
            else throw new InvalidOperationException($"The resource file {resourceName} is invalid because the root element is of type {rootElement.GetType().FullName}; expected a root element of type {typeof(DataTemplate).FullName}!");
        }
        /// <summary>
        /// Loads a <see cref="DataTemplate"/> from an embedded resource XAML file in the calling assembly.
        /// </summary>
        /// <inheritdoc cref="FromEmbeddedResource(string, Assembly)"/>
        public static DataTemplate FromEmbeddedResource(string resourceName)
            => FromEmbeddedResource(resourceName, Assembly.GetCallingAssembly());
        #endregion Static Methods
    }
}
