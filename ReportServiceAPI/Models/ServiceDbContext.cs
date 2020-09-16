using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace ReportServiceAPI.Models
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
				builder.HasKey(u => u.Id);

				builder.HasIndex(u => u.Email).IsUnique(true);
			});

			modelBuilder.Entity<Report>(builder =>
			{
				builder.HasKey(r => r.Id);

				builder.HasOne(r => r.User)
						.WithMany(u => u.Reports)
						.OnDelete(DeleteBehavior.Cascade);

				builder.Property(r => r.Date)
						.HasDefaultValueSql("now()::timestamp");
			});

			base.OnModelCreating(modelBuilder);
		}
	}
}
