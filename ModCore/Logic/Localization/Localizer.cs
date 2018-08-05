using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModCore.Logic.Extensions;
using ModCore.Logic.Table;
using YamlDotNet.Serialization;

namespace ModCore.Logic.Localization
{
    public static class Localizer
    {
        private static Table<ModCoreLocale, string, string> _tokens;

        public static async Task InitializeAll(string prefix, string extension)
        {
            _tokens = new Table<ModCoreLocale, string, string>();

            var deserializer = new DeserializerBuilder().Build();

            foreach (ModCoreLocale locale in Enum.GetValues(typeof(ModCoreLocale)))
            {
                var name = locale.GetAttributeOfType<LocaleNameAttribute>().LocaleName;
                var value = await File.ReadAllTextAsync($"{prefix}{name}{extension}", Encoding.UTF8);

                //_tokens[locale] = new DeserializerBuilder().Build().Deserialize<Dictionary<string, string>>(value);
                var obj = deserializer.Deserialize<Dictionary<object, object>>(new StringReader(value));
                // TODO
                foreach (var (k1, v1) in Flatten(obj))
                {
                    Console.WriteLine($"{k1}: {v1}");
                }
            }

#if DEBUG
            foreach (var (locale, key, value) in _tokens)
            {
                Console.WriteLine($"{locale}: ${key} = {value}");
            }
#endif
        }

        public static string Localize(string text)
        {
            if (_tokens == null) throw new InvalidDataException("Localizer is not initialized yet!");

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
        {
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