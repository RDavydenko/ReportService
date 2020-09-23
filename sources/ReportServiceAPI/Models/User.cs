using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReportService.WebApi.Models
{
	/// <summary>
	/// Пользователь
	/// </summary>
	public class User
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Адрес электронной почты
		/// </summary>
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		/// <summary>
		/// Имя
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// Фамилия
		/// </summary>
		[Required]
		public string Surname { get; set; }

		/// <summary>
		/// Отчество (необязательно)
		/// </summary>
		public string Patronymic { get; set; }

		/// <summary>
		/// Отчеты пользователя
		/// </summary>
		public IEnumerable<Report> Reports { get; set; }

		/// <summary>
		/// Конструктор класса
		/// </summary>
		public User()
		{
			Reports = new List<Report>();
		}
	}
}
