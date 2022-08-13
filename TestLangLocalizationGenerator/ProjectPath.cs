using AssemblyAttribute;

namespace TestLangLocalizationGenerator;

internal class ProjectPathAttribute : BaseAssemblyAttribute
{
    public ProjectPathAttribute(string value = "") : base(value) { }
    public new string Value => base.Value;
}

