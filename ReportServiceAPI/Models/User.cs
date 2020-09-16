using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportServiceAPI.Models
{
	/// <summary>
	/// Пользователь
	/// </summary>
	public class User
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Адрес электронной почты
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Имя
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Фамилия
		/// </summary>
		public string Surname { get; set; }

		/// <summary>
		/// Отчество (необязательно)
		/// </summary>
		public string Patronymic { get; set; }
	}
}
