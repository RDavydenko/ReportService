using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportServiceAPI.sources.Models
{
	/// <summary>
	/// Ответ пользователю от сервера
	/// </summary>
	public class Response
	{
		/// <summary>
		/// Да или нет? Успешно выполнено действие или нет?
		/// </summary>
		public bool Ok { get; set; }

		/// <summary>
		/// Статусный код операции
		/// </summary>
		public int StatusCode { get; set; }

		/// <summary>
		/// Описание 
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Объект, который передаем
		/// </summary>
		public object Object { get; set; }
	}
}
