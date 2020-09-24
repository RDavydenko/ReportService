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
			string logPathFull = Path.Combine(Directory.GetCurrentDirectory(), "logs", ".log"); // %dir%\logs\{logName}.log

			Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.WriteTo.File(path: logPathFull,							
							rollingInterval: RollingInterval.Day,	// ���-���� ��������� �� ���� (����� ���� -> ����� ���-����)
							fileSizeLimitBytes: 1024 * 1024 * 50,	// ����� �� ������� 50 ��
							rollOnFileSizeLimit: true)				// ������� ����� ��� ���������� ������ �� �������
				.CreateLogger();

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog() // �������� ������������
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
