/*
 * TestLangLocalizationGenerator
 * 
 * This program creates a dummy locale with all of the values reversed for debugging purposes.
 */

using Newtonsoft.Json.Linq;
using System.Reflection;
using TestLangLocalizationGenerator;

Assembly asm = Assembly.GetExecutingAssembly();

string localeDir = string.Empty;

if (asm.GetCustomAttribute<ProjectPathAttribute>() is ProjectPathAttribute attr)
{
    const string localeDirExt = "VolumeControl/Localization";
    localeDir = Path.Combine(Path.GetDirectoryName(attr.Value), localeDirExt);
}

if (!Directory.Exists(localeDir))
    throw new FileNotFoundException(localeDir);

//Console.WriteLine(path);

List<string> LanguageIDs = new()
{
    "English (US/CA)"
};

string Reverse(string s)
{
    string rev = string.Empty;
    foreach (char c in s.Reverse())
        rev += c;
    return rev;
}

JObject Process(JObject o)
{
    JObject obj = (JObject)o.DeepClone();
    foreach ((string key, JToken? child) in o)
    {
        if (child is null)
            continue;
        if (LanguageIDs.Any(lid => lid.Equals(key, StringComparison.Ordinal)) && child.Type.Equals(JTokenType.String))
        {
            string value = (string)child!;
            if (obj.ContainsKey("TestLang"))
                obj["TestLang"] = Reverse(value);
            else
                obj.Add("TestLang", Reverse(value));
        }
        else if (child.Type.Equals(JTokenType.Object))
        {
            obj[key] = Process((JObject)child);
        }
    }
    return obj;
}

JObject StripLang(JObject o, params string[] langIDs)
{
    JObject obj = new();
    foreach ((string key, JToken? child) in o)
    {
        switch (child.Type)
        {
        case JTokenType.String:
            if (!langIDs.Any(langID => key.Equals(langID, StringComparison.Ordinal)))
                obj[key] = child;
            break;
        case JTokenType.Object:
            obj[key] = StripLang((JObject)child, langIDs);
            break;
        default:
            break;
        }

    }
    return obj;
}

foreach (var path in Directory.EnumerateFiles(localeDir, "*.loc.json"))
{ // doing this backwards because I'm that lazy rn
    JObject j = JObject.Parse(File.ReadAllText(path));
    string filename = Path.GetFileName(path);

    if (filename.StartsWith("TestLang"))
        continue;

    var jDebug = StripLang(Process(j), LanguageIDs.ToArray());
    File.WriteAllText(Path.Combine(localeDir, "TestLang.loc.json"), jDebug.ToString());
    j = StripLang(j, "TestLang");

    File.WriteAllText(path, j.ToString());
}

