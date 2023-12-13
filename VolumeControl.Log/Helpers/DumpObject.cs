using System.Text;

namespace VolumeControl.Log.Helpers
{
    public static class ObjectDebugger
    {
        [Flags]
        public enum DumpContents
        {
            None = 0,
            PublicProperties = 1,
            NonPublicProperties = 2,
            Properties = PublicProperties | NonPublicProperties,

            PublicFields = 4,
            NonPublicFields = 8,
            Fields = PublicProperties | NonPublicFields,

            All = Properties | Fields,
        }
        public static string ReflectionDump(object obj, DumpContents includes = DumpContents.All)
        {
            var type = obj.GetType();

            var sb = new StringBuilder();

            var objString = obj.ToString();
            var typeString = type.ToString();

            sb.AppendLine($"Object: {objString}{(objString?.Equals(typeString, StringComparison.Ordinal) ?? false ? string.Empty : $" ({typeString})")}");
            sb.AppendLine();
            // PROPERTIES
            if (includes.HasFlag(DumpContents.PublicProperties))
            {
                sb.AppendLine("  Public Properties:");
                foreach (var publicProperty in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    try
                    {
                        sb.AppendLine($"    \"{publicProperty.Name}\": \"{publicProperty.GetValue(obj)}\"");
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"    \"{publicProperty.Name}\": ( [GETTER EXCEPTION] {ex.ToString().ReplaceLineEndings(" [\\n] ")} )");
                    }
                }
            }
            if (includes.HasFlag(DumpContents.NonPublicProperties))
            {
                sb.AppendLine("  Non-Public Properties:");
                foreach (var nonPublicProperty in type.GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    try
                    {
                        sb.AppendLine($"    \"{nonPublicProperty.Name}\": \"{nonPublicProperty.GetValue(obj)}\"");
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"    \"{nonPublicProperty.Name}\": ( [GETTER EXCEPTION] {ex.ToString().ReplaceLineEndings(" [\\n] ")} )");
                    }
                }
            }
            // FIELDS
            if (includes.HasFlag(DumpContents.PublicFields))
            {
                sb.AppendLine("  Public Fields:");
                foreach (var publicField in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    try
                    {
                        sb.AppendLine($"    \"{publicField.Name}\": \"{publicField.GetValue(obj)}\"");
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"    \"{publicField.Name}\": ( [GETTER EXCEPTION] {ex.ToString().ReplaceLineEndings(" [\\n] ")} )");
                    }
                }
            }
            if (includes.HasFlag(DumpContents.NonPublicFields))
            {
                sb.AppendLine("  Non-Public Fields:");
                foreach (var nonPublicField in type.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    try
                    {
                        sb.AppendLine($"    \"{nonPublicField.Name}\": \"{nonPublicField.GetValue(obj)}\"");
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"    \"{nonPublicField.Name}\": ( [GETTER EXCEPTION] {ex.ToString().ReplaceLineEndings(" [\\n] ")} )");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
