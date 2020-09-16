using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReportServiceAPI.DTOs
{
	/// <summary>
	/// DTO класса <see cref="Models.User"/>
	/// </summary>
	public class UserDTO
	{
		/// <summary>
		/// Идентификатор
		/// </summary>		
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
	}
}
