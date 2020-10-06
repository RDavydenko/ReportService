using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ReportService.WebApi.Configs;
using ReportService.WebApi.DTOs;
using ReportService.WebApi.Exceptions;
using ReportService.WebApi.Models;
using ReportService.WebApi.Services;

namespace ReportService.WebApi.Controllers
{
	/// <summary>
	/// Контроллер для работы с пользователями
	/// </summary>
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : Controller
	{
		private readonly IUserAppService _userWebService;

		public UsersController(IUserAppService userWebService)
		{
			_userWebService = userWebService;
		}

		/// <summary>
		/// Получить список пользователей
		/// </summary>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = список пользователей</returns>
		[ProducesResponseType(typeof(Response), 200)]
		[HttpGet]
		public async Task<IActionResult> GetUsers()
		{
			try
			{
				var users = await _userWebService.GetUsersAsync();
				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = users }
				);
			}
			catch (TimeoutException)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 500, Description = "База данных недоступна" }
				);
			}
		}

		/// <summary>
		/// Получить детальную информацию о пользователе
		/// </summary>
		/// <param name="id">Идентификатор пользователя</param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO пользователя <see cref="UserDTO"/></returns>
		[ProducesResponseType(typeof(Response), 200)]
		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> GetUser(int? id)
		{
			if (id.HasValue == false)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 403, Description = "Неверный переданный параметр - " + nameof(id) }
				);
			}
			try
			{
				var userDTO = await _userWebService.GetUserDetailsAsync(id.Value);
				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = userDTO }
				);
			}
			catch (EntityNotFoundException)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
				);
			}
			catch (TimeoutException)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 500, Description = "База данных недоступна" }
				);
			}
		}

		/// <summary>
		/// Добавить нового пользователя
		/// </summary>
		/// <param name="userDTO">Объект типа <see cref="UserDTO"/></param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO добавленного пользователя <see cref="UserDTO"/></returns>
		[ProducesResponseType(typeof(Response), 200)]
		[HttpPost]
		[Route("add")]
		public async Task<IActionResult> AddUser(UserDTO userDTO)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var addedUserDTO = await _userWebService.AddUserAsync(userDTO);
					return new JsonResult(
						new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = addedUserDTO }
					);
				}
				catch (UniqueConstraintException ex)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 403, Description = $"Пользователь с таким {ex.ColumnName} уже существует. Ошибка" }
					);
				}
				catch (TimeoutException)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 500, Description = "База данных недоступна" }
					);
				}
			}
			return new JsonResult(
				new Response { Ok = false, StatusCode = 403, Description = "Переданные данные не прошли валидацию" }
			);
		}

		/// <summary>
		/// Редактировать данные пользователя
		/// </summary>
		/// <param name="id">Идентификатор пользователя</param>
		/// <param name="userDTO">Объект типа <see cref="UserDTO"/></param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO отредактированного пользователя <see cref="UserDTO"/></returns>
		[ProducesResponseType(typeof(Response), 200)]
		[HttpPost]
		[Route("{id}/edit")]
		public async Task<IActionResult> EditUser(int? id, UserDTO userDTO)
		{
			if (ModelState.IsValid)
			{
				if (id.HasValue == false)
				{
					return new JsonResult(
						   new Response { Ok = false, StatusCode = 403, Description = "Неверный переданный параметр - " + nameof(id) }
					   );
				}

				userDTO.Id = id.Value;
				try
				{
					var editedUserDTO = await _userWebService.EditUserAsync(userDTO);
					return new JsonResult(
						new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = editedUserDTO }
					);
				}
				catch (UniqueConstraintException ex)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 403, Description = $"Пользователь с таким {ex.ColumnName} уже существует. Ошибка" }
					);
				}
				catch (EntityNotFoundException)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
					);
				}
				catch (TimeoutException)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 500, Description = "База данных недоступна" }
					);
				}
			}
			return new JsonResult(
				new Response { Ok = false, StatusCode = 403, Description = "Переданные данные не прошли валидацию" }
			);
		}

		/// <summary>
		/// Удалить пользователя
		/// </summary>
		/// <param name="id">Идентификатор пользователя</param>
		/// <returns><see cref="Response"/></returns>
		[ProducesResponseType(typeof(Response), 200)]
		[HttpPost]
		[Route("{id}/delete")]
		public async Task<IActionResult> DeleteUser(int? id)
		{
			if (ModelState.IsValid)
			{
				if (id.HasValue == false)
				{
					return new JsonResult(
						   new Response { Ok = false, StatusCode = 403, Description = "Неверный переданный параметр - " + nameof(id) }
					   );
				}
				try
				{
					bool isDeleted = await _userWebService.DeleteUserAsync(id.Value);
					if (isDeleted == false) // Не удален
					{
						return new JsonResult(
							new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
						);
					}
					else // Удален
					{
						return new JsonResult(
							new Response { Ok = true, StatusCode = 200, Description = "Успешно" }
						);
					}
				}
				catch (TimeoutException)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 500, Description = "База данных недоступна" }
					);
				}
			}
			return new JsonResult(
				new Response { Ok = false, StatusCode = 403, Description = "Переданные данные не прошли валидацию" }
			);
		}

		/// <summary>
		/// Получить список идентификаторов отчетов пользователя за месяц
		/// </summary>
		/// <param name="id">Идентификатор пользователя</param>
		/// <param name="month">Месяц</param>
		/// <param name="year">Год (по умолчанию 2020)</param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = список идентификаторов отчетов</returns>
		[ProducesResponseType(typeof(Response), 200)]
		[HttpGet]
		[Route("{id}/reports")]
		public async Task<IActionResult> GetReportsByMonth(int? id, int? month, int year = 2020)
		{
			if (id.HasValue == false)
			{
				return new JsonResult(
						   new Response { Ok = false, StatusCode = 403, Description = "Неверный переданный параметр - " + nameof(id) }
					   );
			}

			if (month.HasValue == false || month <= 0 || month > 12)
			{
				return new JsonResult(
						   new Response { Ok = false, StatusCode = 403, Description = "Неверный переданный параметр - " + nameof(month) }
					   );
			}

			if (year < 0)
			{
				return new JsonResult(
						   new Response { Ok = false, StatusCode = 403, Description = "Неверный переданный параметр - " + nameof(year) }
					   );
			}

			try
			{
				var reportIds = await _userWebService.GetReportsByMonthAsync(id.Value, month.Value, year);
				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = reportIds }
				);
			}
			catch (EntityNotFoundException)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
				);
			}
			catch (TimeoutException)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 500, Description = "База данных недоступна" }
				);
			}
		}
	}
}
