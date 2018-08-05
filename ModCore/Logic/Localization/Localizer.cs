using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModCore.Logic.Extensions;
using ModCore.Logic.Table;
using YamlDotNet.Serialization;

namespace ModCore.Logic.Localization
{
    public class Localizer : IDisposable
    {
        private Table<ModCoreLocale, string, string> _localeTokenMappings;
        private FileSystemWatcher _watcher;

        public async Task<Localizer> InitializeAll(string localesDir, string prefix, string extension)
        {
            _localeTokenMappings = new Table<ModCoreLocale, string, string>();

            var deserializer = new DeserializerBuilder().Build();

            foreach (ModCoreLocale locale in Enum.GetValues(typeof(ModCoreLocale)))
            {
                var name = locale.GetAttributeOfType<LocaleNameAttribute>().LocaleName;
                var value = await File.ReadAllTextAsync($@"{localesDir}\{prefix}{name}{extension}", Encoding.UTF8);

                //_tokens[locale] = new DeserializerBuilder().Build().Deserialize<Dictionary<string, string>>(value);
                var obj = deserializer.Deserialize<Dictionary<object, object>>(new StringReader(value));
                
                _localeTokenMappings.Put(locale, Flatten(obj));
            }
            
            InitializeWatcher(localesDir);

            return this;
        }

        [Conditional("DEBUG")]
        private void InitializeWatcher(string localesDir)
        {
            // https://stackoverflow.com/a/721743
            // Create a new FileSystemWatcher and set its properties.
            _watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(localesDir),
                // Watch for changes in LastAccess and LastWrite times, and the renaming of files or directories.
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName 
                               | NotifyFilters.DirectoryName,
                // Only watch text files.
                Filter = "*.yml"
            };
            // Add event handlers.
            _watcher.Changed += OnChanged;

            // Begin watching.
            _watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            //TODO
        }

        public string Localize(string text)
        {
            if (_localeTokenMappings == null) throw new InvalidDataException("Localizer is not initialized yet!");

            return null;
        }

        /// <summary>
        /// Transforms eg
        /// <code>
        ///     key:
        ///         value1: val
        ///     akey: valu
        ///     keya.keyb: vvva1
        /// </code>
        /// into
        /// <code>
        ///     key.value1: val
        ///     akey: valu
        ///     keya.keyb: vvva1
        /// </code>
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static IEnumerable<KeyValuePair<string, string>> Flatten(Dictionary<object, object> dict)
        {// TODO can maybe use List<string> instead of concatenation?
            foreach (var (k1, v1) in dict)
            {
                if (v1 is Dictionary<object, object> dict2)
                {
                    foreach (var (k2, v2) in Flatten(dict2))
                    {
                        yield return new KeyValuePair<string, string>($"{k1}.{k2}", v2);
                    }
                }
                else if (k1 is string kstr && v1 is string vstr)
                {
                    yield return new KeyValuePair<string, string>(kstr, vstr);
                }
                else
                {
                    throw new Exception(v1?.GetType().ToString() ?? "null type!");
                }
            }
        }

        public void Dispose() => _watcher?.Dispose();
    }

    public enum ModCoreLocale : ushort
    {
        [LocaleName("en-US")] English
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class LocaleNameAttribute : Attribute
    {
        public string LocaleName { get; }

        public LocaleNameAttribute(string localeName) => this.LocaleName = localeName;
    }
}