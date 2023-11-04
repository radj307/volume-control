using System.Collections;
using System.Windows;
using System.Windows.Controls;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input.Actions.Settings;

namespace VolumeControl.SDK.DataTemplates
{
    /// <summary>
    /// Provides all of the default <see cref="DataTemplate"/> instances used by the built-in hotkey actions.
    /// </summary>
    [DataTemplateProvider]
    public partial class DataTemplateDictionary : ResourceDictionary, ITemplateDictionaryProvider
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="DataTemplateDictionary"/> instance.
        /// </summary>
        public DataTemplateDictionary()
        {
            InitializeComponent();
        }
        #endregion Constructor

        #region Properties
        private static VCAPI VCAPI => VCAPI.Default;
        #endregion Properties

        #region IMultiDataTemplateProvider Implementation
        /// <inheritdoc/>
        public ActionSettingDataTemplate? ProvideDataTemplate(string key) => base[key] as ActionSettingDataTemplate;
        /// <inheritdoc/>
        public ActionSettingDataTemplate? ProvideDataTemplate(Type valueType)
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
        #endregion IMultiDataTemplateProvider Implementation

        #region EventHandlers

        #region ActionTargetSpecifierDataTemplate_RemoveItemButton
        private void ActionTargetSpecifierDataTemplate_RemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var listBox = (ListBox)button.Tag;

            var setting = (IActionSettingInstance)listBox.DataContext;
            var specifier = (ActionTargetSpecifier)setting.Value!;

            var index = listBox.ItemContainerGenerator.IndexFromContainer((ListBoxItem)button.DataContext);

            specifier.Targets.RemoveAt(index);
        }
        #endregion ActionTargetSpecifierDataTemplate_RemoveItemButton

        #region ActionTargetSpecifierDataTemplate_AddTargetBox
        /// <summary>
        /// Adds the clicked suggestion to the list of target overrides.
        /// </summary>
        private void ActionTargetSpecifierDataTemplate_AddTargetBox_SuggestionClicked(object sender, WPF.Controls.TextBoxWithCompletionOptions.SuggestionClickedEventArgs e)
        {
            var addTargetBox = (WPF.Controls.TextBoxWithCompletionOptions)sender;

            var setting = (IActionSettingInstance)((ListBox)addTargetBox.Tag).DataContext;
            var specifier = (ActionTargetSpecifier)setting.Value!;

            specifier.Targets.Add(e.SuggestionText);
        }
        /// <summary>
        /// Adds (and attempts to resolve) the committed text to the list of target overrides.
        /// </summary>
        private void ActionTargetSpecifierDataTemplate_AddTargetBox_CommittedText(object sender, WPF.Controls.TextBoxWithCompletionOptions.CommittedTextEventArgs e)
        {
            var addTargetBox = (WPF.Controls.TextBoxWithCompletionOptions)sender;

            if (addTargetBox.Text.Trim().Length == 0)
            {
                e.Handled = true;
                return;
            }

            var setting = (IActionSettingInstance)((ListBox)addTargetBox.Tag).DataContext;
            var specifier = (ActionTargetSpecifier)setting.Value!;

            if (VCAPI.Default.AudioSessionManager.FindSessionWithProcessName(addTargetBox.Text, StringComparison.OrdinalIgnoreCase) is CoreAudio.AudioSession session)
            {
                specifier.Targets.Add(session.ProcessName);
            }
            else // add whatever text was entered
            {
                specifier.Targets.Add(addTargetBox.Text);
            }

            addTargetBox.Text = string.Empty;
        }
        /// <summary>
        /// Removes the last item in the list of target overrides.
        /// </summary>
        private void ActionTargetSpecifierDataTemplate_AddTargetBox_BackPressed(object sender, RoutedEventArgs e)
        {
            var addTargetBox = (WPF.Controls.TextBoxWithCompletionOptions)sender;
            var listBox = (ListBox)addTargetBox.Tag;

            if (listBox.Items.Count == 0) return;

            var setting = (IActionSettingInstance)listBox.DataContext;
            var specifier = (ActionTargetSpecifier)setting.Value!;

            specifier.Targets.RemoveAt(listBox.Items.Count - 1);//< remove the last item in the list
        }
        #endregion ActionTargetSpecifierDataTemplate_AddTargetBox

        #endregion EventHandlers
    }
}
