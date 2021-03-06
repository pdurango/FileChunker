using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChunkServiceHandler;
using DAL;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//Add-Migration NewMigration -Project DAL
			services.AddDbContext<DataContext>(
				opts => opts.UseSqlServer(Configuration.GetConnectionString("FileChunkerDB")));

			services.AddSingleton<ChunkService>();

			services.AddControllers().AddNewtonsoftJson(options =>
				options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			);

			services.Configure<FormOptions>(options =>
			{
				options.MemoryBufferThreshold = Int32.MaxValue;
			});

			services.AddControllers();


			/*services.AddControllers().AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
			});*/

			//https://andrewlock.net/handling-web-api-exceptions-with-problemdetails-middleware/
			services.AddProblemDetails(opts =>
			{
				// Control when an exception is included
				opts.IncludeExceptionDetails = (ctx, ex) =>
				{
					// Fetch services from HttpContext.RequestServices
					var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
					return env.IsDevelopment() || env.IsStaging();
				};
			});

			services.AddCors(o => o.AddPolicy(MyAllowSpecificOrigins, builder =>
			{
				builder.AllowAnyOrigin()
					   .AllowAnyMethod()
					   .AllowAnyHeader()
					   .WithExposedHeaders("Content-Disposition");
			}));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			/*if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			else
				app.UseProblemDetails();*/
			app.UseProblemDetails();

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors(MyAllowSpecificOrigins);

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
