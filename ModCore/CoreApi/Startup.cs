﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

namespace ModCore.CoreApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc(option => option.EnableEndpointRouting = false);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			if (!Directory.Exists("wwwstatic"))
				Directory.CreateDirectory("wwwstatic");

			var fp = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwstatic"));

			var df = new DefaultFilesOptions();
			df.DefaultFileNames.Clear();
			df.DefaultFileNames.Add("index.html");
			df.FileProvider = fp;
			df.RequestPath = "";

			app.UseDefaultFiles(df);
			app.UseStaticFiles(new StaticFileOptions() { FileProvider = fp, RequestPath = "" });
			app.UseRouting();
		}
	}
}
