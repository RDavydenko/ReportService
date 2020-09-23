using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace ReportServiceAPI.sources.Models
{
	/// <summary>
	/// Контекст базы данных приложения
	/// </summary>
	public class ServiceDbContext : DbContext
	{
		/// <summary>
		/// Пользователи
		/// </summary>
		public DbSet<User> Users { get; set; }

		/// <summary>
		/// Отчеты пользователей
		/// </summary>
		public DbSet<Report> Reports { get; set; }

		public ServiceDbContext(DbContextOptions<ServiceDbContext> options)
			:base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(builder =>
			{
				// PK - Id
				builder.HasKey(u => u.Id);

				// Email - уникальный
				builder.HasIndex(u => u.Email).IsUnique(true);
			});

			modelBuilder.Entity<Report>(builder =>
			{
				// PK - Id
				builder.HasKey(r => r.Id);

				// User (1) ко многим Reports. При удалении User - удаляются все связанные Reports
				builder.HasOne(r => r.User)
						.WithMany(u => u.Reports)
						.OnDelete(DeleteBehavior.Cascade);

				// Default Report.Date 
				builder.Property(r => r.Date)
						.HasDefaultValueSql("now()::timestamp");
			});

			base.OnModelCreating(modelBuilder);
		}
	}
}
