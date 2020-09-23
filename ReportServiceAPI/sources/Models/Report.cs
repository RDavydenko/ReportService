using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReportServiceAPI.sources.Models
{
	/// <summary>
	/// Отчет о выполненной работе
	/// </summary>
	public class Report
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Примечание
		/// </summary>
		[Required]
		public string Remark { get; set; }

		/// <summary>
		/// Количество часов
		/// </summary>
		[Required]
		[Range(minimum: 0, maximum: int.MaxValue)]
		public int Hours { get; set; }

		/// <summary>
		/// Дата
		/// </summary>
		[Required]
		public DateTime Date { get; set; }

		/// <summary>
		/// Пользователь, которому принадлежит отчет
		/// </summary>
		public User User { get; set; }

		/// <summary>
		/// Конструктор по умолчанию
		/// </summary>
		public Report()
		{
		}
	}
}
