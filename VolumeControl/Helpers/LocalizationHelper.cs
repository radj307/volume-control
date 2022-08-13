using CodingSeb.Localization;
using CodingSeb.Localization.Loaders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using VolumeControl.Core;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Helpers
{
    internal class LocalizationHelper
    {
        #region Constructor
        public LocalizationHelper(bool overwriteDefaultLangConfigs = false)
        {
            if (_initialized)
                return;

            _initialized = true;

            FileLoaders.AddIfUnique(new JsonFileLoader());
            FileLoaders.AddIfUnique(new YamlFileLoader());

            if (Settings.CreateDefaultTranslationFiles) //< never create default files when this (and this alone) is false!
                CreateDefaultFiles(overwriteDefaultLangConfigs);

            ReloadLanguageConfigs();

            Loc.CurrentLanguage = Settings.LanguageName;

            Loc.CurrentLanguageChanged += (s, e) =>
            {
                if (!e.NewLanguageId.Equals(Settings.LanguageName, StringComparison.Ordinal))
                {
                    Settings.LanguageName = e.NewLanguageId;
                    Log.Info($"{nameof(Settings.LanguageName)} was changed to '{Settings.LanguageName}' (was '{e.OldLanguageId}')");
                }
            };
        }
        #endregion Constructor

        #region Fields
        private static readonly char[] _invalidPathChars = Path.GetInvalidPathChars();
        private static bool _initialized = false;
        #endregion Fields

        #region Properties
        private static LogWriter Log => FLog.Log;
        private static Config Settings => (Config.Default as Config)!;
        private static LocalizationLoader Loader => LocalizationLoader.Instance;
        private static List<ILocalizationFileLoader> FileLoaders => Loader.FileLanguageLoaders;
        private static string DefaultPath { get; } = Path.Combine(PathFinder.LocalAppData, "Localization");
        private static Loc Loc => Loc.Instance;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Reloads all language config files from the disk, calling <see cref="LocalizationLoader.ClearAllTranslations(bool)"/> to clear the cache, then re-enumerating the <see cref="Config.CustomLocalizationDirectories"/> list and reloading each file.
        /// </summary>
        /// <remarks>Before this method returns, it will attempt to re-select the current language config using the value of the <see cref="Config.LanguageName"/> setting.</remarks>
        public static void ReloadLanguageConfigs()
        {
            // clear all loaded translations, and available languages
            Loader.ClearAllTranslations(true);

            // check default directory
            LoadTranslationsFromDirectory(DefaultPath);

            // check custom directories
            foreach (string dir in Settings.CustomLocalizationDirectories)
            {
                if (dir.Any(c => _invalidPathChars.Contains(c)))
                    Log.Error($"{nameof(Settings.CustomLocalizationDirectories)} specifies directory path with illegal characters: '{dir}'");
                else
                    LoadTranslationsFromDirectory(dir);
            }
        }

        /// <summary>
        /// Checks if <paramref name="filename"/> appears to specify a language config file using a regular expression.
        /// </summary>
        /// <remarks><b>This does not check if the file actually exists.</b></remarks>
        /// <param name="filename">A filename or filepath to validate.</param>
        /// <returns><see langword="true"/> when <paramref name="filename"/> specifies a valid language config file; otherwise <see langword="false"/></returns>
        private static bool ValidateLanguageConfigFilename(string filename)
            => Path.HasExtension(filename) && Regex.Match(filename, "^.+?\\.loc\\.(?:json|yaml|yml)$", RegexOptions.Compiled).Success;

        private static void LoadTranslationsFromDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string filepath in Directory.EnumerateFiles(path))
                {
                    string filename = Path.GetFileName(filepath);
                    if (!ValidateLanguageConfigFilename(filename))
                        continue;

                    try
                    {
                        Loader.AddFile(filepath);
                        Log.Debug($"Successfully loaded language config '{filepath}'");
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to load language config '{filepath}' because an exception was thrown!", ex);
                    }
                }
            }
            else
            {
                Log.Error($"{nameof(LoadTranslationsFromDirectory)} cannot load directory '{path}' because it doesn't exist!");
            }
        }

        /// <summary>
        /// To add additional default localizations, put the "*.loc.json" file in VolumeControl/Localization, then mark it as an "Embedded resource":<br/><b>Solution Explorer => Properties => Build Action => Embedded resource</b>
        /// </summary>
        private static void CreateDefaultFiles(bool overwrite = false)
        {
            if (!Directory.Exists(DefaultPath))
                _ = Directory.CreateDirectory(DefaultPath);

            var asm = Assembly.GetExecutingAssembly();
            const string resourcePath = "VolumeControl.Localization"; //< specifies the directory/namespace where localization files are located (relative to the solution dir)

            foreach (string embeddedResourceName in asm.GetManifestResourceNames())
            {
                if (embeddedResourceName.StartsWith(resourcePath, StringComparison.Ordinal) && ValidateLanguageConfigFilename(embeddedResourceName))
                {
                    string
                        embeddedResourceFilename = embeddedResourceName[(resourcePath.Length + 1)..],
                        filepath = Path.Combine(DefaultPath, embeddedResourceFilename);

                    if (overwrite || !File.Exists(filepath))
                    {
                        using Stream? stream = asm.GetManifestResourceStream(embeddedResourceName);

                        if (stream is null || stream.Length.Equals(0))
                            continue;

                        using (var sw = new StreamWriter(File.Open(filepath, FileMode.Create, FileAccess.Write, FileShare.None)))
                        {
                            stream?.CopyTo(sw.BaseStream);
                            sw.Flush();
                            sw.Close();
                        };
                    }
                }
            }
        }
        #endregion Methods
    }
    /// <summary>
    /// Custom <see cref="ILocalizationFileLoader"/> implementation that emulates <see cref="JsonFileLoader"/>, with a few extra features.
    /// </summary>
    public class TestFileLoader : ILocalizationFileLoader
    {
        private static LogWriter Log => FLog.Log;
        private string _fileName = string.Empty;
        private readonly Stack<string> _currentPath = new();
        private readonly Dictionary<string, string> _langIndex = new();

        private string CurrentPath
        {
            get
            {
                string path = string.Empty;
                bool fst = true;
                foreach (string segment in _currentPath.Reverse())
                {
                    if (fst)
                        fst = false;
                    else
                        path += '.';
                    path += segment;
                }
                return path;
            }
            set
            {
                _currentPath.Clear();
                _ = value.Split('.').ForEach(_currentPath.Push);
            }
        }

        public bool CanLoadFile(string fileName) => fileName.EndsWith(".loc.json", StringComparison.Ordinal) && File.Exists(fileName);
        public void LoadFile(string fileName, LocalizationLoader loader)
        {
            this.Reset();
            _fileName = fileName;
            try
            {
                var root = (JObject)JToken.Parse(File.ReadAllText(fileName));
                if (!root.ContainsKey("LanguageName"))
                    throw new JsonException("Language Config files must contain a 'LanguageName' metadata section!");
                this.SetLanguageMetadataFromObject(ref root);
                this.LoadFromObject(root, loader);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void SetLanguageMetadataFromObject(ref JObject root)
        {
            JToken? meta = root["LanguageName"];
            if (meta is null || !meta.Type.Equals(JTokenType.Object))
                throw new JsonException($"Invalid 'LanguageName' metadata section in language config file '{_fileName}'!");

            foreach ((string key, JToken? valueToken) in (JObject)meta)
            {
                if (valueToken is null || !valueToken.Type.Equals(JTokenType.String)) continue;

                _langIndex.Add(key, (string?)valueToken ?? string.Empty);
            }

            _ = root.Remove("LanguageName");
        }

        private void LoadFromObject(JObject obj, LocalizationLoader loader)
        {
            foreach ((string key, JToken? val) in obj)
            {
                if (val is null) continue;

                string? s = null;

                switch (val.Type)
                {
                case JTokenType.Array:
                    s = string.Empty;
                    _ = ((JArray)val).ForEach(item =>
                    {
                        if (item.Type.Equals(JTokenType.String))
                        {
                            s += (string?)item;
                            s += '\n';
                        }
                    });
                    goto is_value_type;
                case JTokenType.String:
                    s = (string?)val;

                is_value_type:
                    if (!_langIndex.ContainsKey(key))
                        throw new Exception($"Language '{key}' is missing a metadata entry!");

                    if (s is not null && s.Length > 0)
                        loader.AddTranslation(this.CurrentPath, _langIndex[key], s, _fileName);

                    break;
                case JTokenType.Object:
                    _currentPath.Push(key);
                    this.LoadFromObject((JObject)val, loader); //< recurse
                    _ = _currentPath.Pop();
                    break;
                default: break;
                }
            }
        }
        private void Reset()
        {
            _fileName = string.Empty;
            _currentPath.Clear();
            _langIndex.Clear();
        }
    }
}
