using System.Linq;
using System.Windows.Automation;

namespace Toastify.Helpers
{
    public static class AutomationHelper
    {
        public static AutomationPattern GetSpecifiedPattern(AutomationElement element, string patternName)
        {
            AutomationPattern[] supportedPattern = element.GetSupportedPatterns();
            return supportedPattern.FirstOrDefault(pattern => pattern.ProgrammaticName == patternName);
        }
    }
}