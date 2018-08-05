using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ModCore.Logic.HelpTemplating
{
    public class Templater : IDisposable
    {
        private IDictionary<string, string> _tokenMappings;
        private FileSystemWatcher _watcher;
        private static readonly Regex TokenRegex = new Regex(@"\$([a-zA-Z0-9\.]+)", RegexOptions.Compiled);

        public async Task<Templater> Initialize(string templatePath)
        {
            _tokenMappings = new Dictionary<string, string>();

            var value = await File.ReadAllTextAsync($@"{templatePath}", Encoding.UTF8);
            var obj = new DeserializerBuilder().Build().Deserialize<Dictionary<object, object>>(new StringReader(value));

            foreach (var entry in Flatten(obj))
                _tokenMappings.Add(entry);

            return this;
        }

        public string Transform(string text)
        {
            if (_tokenMappings == null) throw new InvalidDataException("Localizer is not initialized yet!");

            return TokenRegex.Replace(text, m =>
            {
                var str = m.Groups[1].Value;

                if (!_tokenMappings.TryGetValue(str, out var val))
                    throw new InvalidDataException($"No value found for token [[${str}]]");

                return val;
            });
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
}