using AssemblyAttribute;

namespace TestLangLocalizationGenerator;

class ProjectPathAttribute : BaseAssemblyAttribute
{
    public ProjectPathAttribute(string value = "") : base(value) { }
    public new string Value => base.Value;
}

