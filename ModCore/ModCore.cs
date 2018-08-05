using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.OData.Query.SemanticAst;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Api;
using ModCore.CoreApi;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Listeners;
using ModCore.Logic.HelpTemplating;
using Newtonsoft.Json;
using Startup = ModCore.CoreApi.Startup;

namespace ModCore
{
    public class ModCore
    {
		public Settings Settings { get; private set; }
		public SharedData SharedData { get; private set; }
		public List<ModCoreShard> Shards { get; set; }
	    
	    private DatabaseContextBuilder GlobalContextBuilder { get; set; }

        internal async Task InitializeAsync(string[] args)
        {
            if (!File.Exists("settings.json"))
            {
                var json = JsonConvert.SerializeObject(new Settings(), Formatting.Indented);
                File.WriteAllText("settings.json", json, new UTF8Encoding(false));
                Console.WriteLine("Config file was not found, a new one was generated. Fill it with proper values and rerun this program");
                Console.ReadKey();
                return;
            }

            var input = File.ReadAllText("settings.json", new UTF8Encoding(false));
            Settings = JsonConvert.DeserializeObject<Settings>(input);
	        
	        this.GlobalContextBuilder = Settings.Database.CreateContextBuilder();

            this.Shards = new List<ModCoreShard>();
	        
	        this.SharedData = new SharedData
	        {
		        CTS = new CancellationTokenSource(),
		        ProcessStartTime = Process.GetCurrentProcess().StartTime,
		        BotManagers = Settings.BotManagers,
		        DefaultPrefix = Settings.DefaultPrefix,
		        ModCore = this
	        };
	        if (args.Length == 2)
	        {
		        this.SharedData.StartNotify = (ulong.Parse(args[0]), ulong.Parse(args[1]));
	        }

	        var sharedServices = new SharedServices
	        {
		        Templater = await new Templater().Initialize("text-resources.yml"),
		        Perspective = new Perspective(Settings.PerspectiveToken)
	        };

	        // cnext data that is consistent across shards, so it's fine to share it
	        for (var i = 0; i < Settings.ShardCount; i++)
            {
                var shard = new ModCoreShard(this.Settings, i, this.SharedData);
                shard.Initialize(sharedServices);
	            this.Shards.Add(shard);
	            if (i == 0)
	            {
		            this.SharedData.Initialize(shard);
	            }
            }

	        await this.InitializeDatabaseAsync();

	        foreach (var shard in Shards)
		        await shard.RunAsync();

	        await this.BuildWebHost().RunAsync(this.SharedData.CTS.Token);

			await WaitForCancellation(this.SharedData.CTS);

			foreach (var shard in this.Shards)
				await shard.DisconnectAndDispose();

			this.SharedData.CTS.Dispose();
			this.SharedData.TimerData.Cancel.Cancel();
			this.SharedData.TimerSempahore.Dispose();
        }

	    private async Task InitializeDatabaseAsync()
	    {
		    // add command id mappings if they don't already exist
		    var modifications = new List<string>();
		    using (var db = this.CreateGlobalContext())
		    {
			    foreach (var (name, _) in this.SharedData.Commands)
			    {
				    if (db.CommandIds.FirstOrDefault(e => e.Command == name) != null) continue;
				    Console.WriteLine($"Registering new command in db: {name}");
				    
				    modifications.Add(name);
				    await db.CommandIds.AddAsync(new DatabaseCommandId()
				    {
					    Command = name
				    });
			    }
			    await db.SaveChangesAsync();
		    }
	    }
	    
        private static Task WaitForCancellation(CancellationTokenSource cts)
        {
	        var tcs = new TaskCompletionSource<bool>();
	        cts.Token.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
	        return tcs.Task;
        }

		private IWebHost BuildWebHost()
		{
			var container = new CoreContainer
			{
				mcore = this
			};

			var mservice = new ServiceDescriptor(container.GetType(), container);

			return WebHost.CreateDefaultBuilder(new string[0])
				.UseStartup<Startup>()
				.ConfigureServices(x => x.Add(mservice))
				.UseUrls("http://0.0.0.0:6969")
				.Build();
		}
	    
	    public DatabaseContext CreateGlobalContext()
	    {
		    return GlobalContextBuilder.CreateContext();
	    }
	}
}
