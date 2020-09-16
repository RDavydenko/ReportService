using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportServiceAPI.Models
{
	/// <summary>
	/// Отчет о выполненной работе
	/// </summary>
	public class Report
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Примечание
		/// </summary>
		public string Remark { get; set; }

		/// <summary>
		/// Количество часов
		/// </summary>
		public int Hours { get; set; }

		/// <summary>
		/// Дата
		/// </summary>
		public DateTime Date { get; set; }
	}
}
