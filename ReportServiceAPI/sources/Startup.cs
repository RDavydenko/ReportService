using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using ReportServiceAPI.Models;

namespace ReportServiceAPI
{
	public class Startup
	{
		public Startup(IWebHostEnvironment env)
		{
			// Настраиваем файл конфигурации (его местоположение и название)
			var builder = new ConfigurationBuilder()
				.SetBasePath(Path.Combine(env.ContentRootPath, "sources"))
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			Configuration = builder.Build();
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			string connection = Configuration.GetConnectionString("DefaultConnection"); // строка подключения к БД Postgres

			// Добавляем DB контекст
			services.AddDbContext<ServiceDbContext>(builder =>
			{
				// Postrgres
				builder.UseNpgsql(connection, npgsqlBuilder =>
				{
					
				});
			});

			services.AddControllers();

			services.AddSwaggerGen(opt =>
			{
				opt.SwaggerDoc("v1", new OpenApiInfo()
				{ 
					Version = "v1",
					Title = "ReportServiceAPI",
					Description = "REST API сервис для учета отработанного времени.",

				});

				// Добавляем xml комментарии из документации
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				opt.IncludeXmlComments(xmlPath);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();

			app.UseSwagger(opt =>
			{
				// Версия json 2.0 для корректной работы
				opt.SerializeAsV2 = true;
			});
			app.UseSwaggerUI(opt =>
			{
				opt.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			// Файловый сервер к документации Redoc
			app.UseFileServer(new FileServerOptions()
			{
				FileProvider = new PhysicalFileProvider(
					Path.Combine(Directory.GetCurrentDirectory(), "documentation")), // Папка documentation
				RequestPath = "", // При запросе по ссылке: /
				EnableDefaultFiles = true
			});

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
