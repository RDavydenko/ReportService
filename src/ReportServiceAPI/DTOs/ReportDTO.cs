﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using ReportService.WebApi.Models;

namespace ReportService.WebApi.DTOs
{
	/// <summary>
	/// DTO класса <see cref="Report"/>
	/// </summary>
	public class ReportDTO
	{
		/// <summary>
		/// Идентификатор
		/// </summary>		
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
		public DateTime? Date { get; set; }

		/// <summary>
		/// Идентификатор пользователя, которому принадлежит отчет
		/// </summary>
		public int? UserId { get; set; }

		/// <summary>
		/// Имя пользователя, которому принадлежит отчет
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Фамилия пользователя, которому принадлежит отчет
		/// </summary>
		public string UserSurname { get; set; }

		/// <summary>
		/// Конструктор по умолчанию
		/// </summary>
		public ReportDTO()
		{
		}
	}
}
