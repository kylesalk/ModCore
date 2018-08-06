using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ModCore.Database;
using Newtonsoft.Json;
using Npgsql;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;

namespace ModCore.Entities
{
    public class Settings
    {
        [JsonProperty("token")]
        [YamlMember(Alias = "token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        [YamlMember(Alias = "prefix")]
        public string DefaultPrefix { get; private set; }

        [JsonProperty("shard_count")]
        [YamlMember(Alias = "shard_count")]
        public int ShardCount { get; private set; }

        [JsonProperty("use_perspective")]
        [YamlMember(Alias = "use_perspective")]
        public bool UsePerspective { get; private set; }

        [JsonProperty("perspective_token")]
        [YamlMember(Alias = "perspective_token")]
        public string PerspectiveToken { get; private set; }

        [JsonProperty("database")]
        [YamlMember(Alias = "database")]
        public DatabaseSettings Database { get; private set; }

		[JsonProperty("bot_managers")]
		[YamlMember(Alias = "bot_managers")]
		public List<ulong> BotManagers { get; private set; } = new List<ulong>();

        public static async Task<Settings> LoadAsync()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();
            var utf8 = new UTF8Encoding(false);
            
            if (!File.Exists("settings.yml"))
            {
                if (!File.Exists("settings.json"))
                {
                    // create default yaml file
                    var yaml = serializer.Serialize(new Settings());
                    await File.WriteAllTextAsync("settings.yml", yaml, utf8);
                    Console.WriteLine("Config file was not found, a new one was generated. Fill it with proper values and rerun this program");
                    Console.ReadKey();
                    return null;
                }

                // convert json to yaml
                var input = await File.ReadAllTextAsync("settings.json", utf8);
                var settings = JsonConvert.DeserializeObject<Settings>(input);
                await File.WriteAllTextAsync("settings.yml", serializer.Serialize(settings), utf8);
                File.Move("settings.json", "settings.json.bak");
                Console.WriteLine("Config file was converted from JSON to YAML. Backup available at settings.json.bak");
                return settings;
            }

            return deserializer.Deserialize<Settings>(await File.ReadAllTextAsync("settings.yml", utf8));
        }
    }
    
    public class DatabaseSettings
    {
        [JsonProperty("provider")]
        [YamlMember(Alias = "provider")]
        public DatabaseProvider Provider { get; private set; }

        /// <summary>
        /// Allows deserializing obsolete UseInMemoryProvider setting, but not serialization.
        /// </summary>
        [JsonProperty("in_memory")]
        [YamlIgnore]
        [Obsolete("Use " + nameof(Provider) + " instead!")]
        public bool UseInMemoryProvider
        {
            set { if (value) Provider = DatabaseProvider.InMemory; }
            // getter only for YamlDotNet
            //get => default;
        }

        [JsonProperty("hostname")]
        [YamlMember(Alias = "hostname")]
        public string Hostname { get; private set; }

        [JsonProperty("port")]
        [YamlMember(Alias = "port")]
        public int Port { get; private set; }

        [JsonProperty("database")]
        [YamlMember(Alias = "database")]
        public string Database { get; private set; }

        [JsonProperty("username")]
        [YamlMember(Alias = "username")]
        public string Username { get; private set; }

        [JsonProperty("password")]
        [YamlMember(Alias = "password")]
        public string Password { get; private set; }
        
        [JsonProperty("data_source")]
        [YamlMember(Alias = "data_source")]
        public string DataSource { get; private set; }

        public string BuildConnectionString()
        {
            switch (this.Provider)
            {
                case DatabaseProvider.InMemory:
                    return null;
                case DatabaseProvider.Sqlite:
                    return "Data Source=" + this.DataSource;
                default:
                    return new NpgsqlConnectionStringBuilder
                    {
                        Host = this.Hostname,
                        Port = this.Port,
                        Database = this.Database,
                        Username = this.Username,
                        Password = this.Password,

                        SslMode = SslMode.Prefer,
                        TrustServerCertificate = true,

                        Pooling = false
                    }.ConnectionString;
            }

        }

        public DatabaseContextBuilder CreateContextBuilder() =>
            new DatabaseContextBuilder(this.Provider, this.BuildConnectionString());
    }

    public enum DatabaseProvider : byte
    {
        PostgreSql,
        InMemory,
        Sqlite
    }
}
