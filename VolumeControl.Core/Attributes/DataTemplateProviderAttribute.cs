namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Indicates that this class is a <see cref="ITemplateProvider"/> or <see cref="ITemplateDictionaryProvider"/> for action setting DataTemplates.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DataTemplateProviderAttribute : Attribute { }
}
