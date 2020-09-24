using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

namespace ReportServiceAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false)
				.Build(); // Конфигурация приложения

			// Получаем значения из конфига appsettings.json
			var serilogConfig = config.GetSection("Logging").GetSection("Serilog");
			var rollingInterval = serilogConfig.GetValue<RollingInterval>("RollingInterval", RollingInterval.Infinite);
			var sizeLimitBytes = serilogConfig.GetValue<long>("FileSizeLimitBytes", 1024 * 1024 * 50);
			var rollOnFileSizeLimit = serilogConfig.GetValue<bool>("RollOnFileSizeLimit", false);

			string logPathFull = Path.Combine(Directory.GetCurrentDirectory(), "logs", ".log"); // %dir%\logs\{logName}.log
			
			Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.WriteTo.File(path: logPathFull,							
							rollingInterval: rollingInterval,			
							fileSizeLimitBytes: sizeLimitBytes,			
							rollOnFileSizeLimit: rollOnFileSizeLimit)	
				.CreateLogger();

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog() // Добавить логгирование
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
