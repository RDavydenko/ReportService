using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ReportServiceAPI.Configs;
using ReportServiceAPI.DTOs;
using ReportServiceAPI.Models;

namespace ReportServiceAPI.Controllers
{
	/// <summary>
	/// Контроллер для работы с пользователями
	/// </summary>
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : Controller
	{
		private readonly ServiceDbContext _db;

		public UsersController(ServiceDbContext db)
		{
			_db = db;
		}

		/// <summary>
		/// Получить список пользователей
		/// </summary>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = список идентификаторов пользователей</returns>
		[HttpGet]
		public async Task<IActionResult> GetUsers()
		{
			var usersIds = await _db.Users.Select(x => new { x.Id }).ToListAsync();

			return new JsonResult(
				new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = usersIds }
				);
		}

		/// <summary>
		/// Получить детальную информацию о пользователе
		/// </summary>
		/// <param name="id">Идентификатор пользователя</param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO пользователя <see cref="UserDTO"/></returns>
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

			var user = await _db.Users.Where(x => x.Id == id.Value).FirstOrDefaultAsync();
			if (user == null)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
					);
			}

			var config = AutoMapperConfig.FromUserToUserDTO;
			var mapper = new Mapper(config);

			var userDTO = mapper.Map<UserDTO>(user);
			return new JsonResult(
				new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = userDTO }
				);
		}

		/// <summary>
		/// Добавить нового пользователя
		/// </summary>
		/// <param name="userDTO">Объект типа <see cref="UserDTO"/></param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO добавленного пользователя <see cref="UserDTO"/></returns>
		[HttpPost]
		[Route("add")]
		public async Task<IActionResult> AddUser(UserDTO userDTO)
		{
			if (ModelState.IsValid)
			{
				var config = AutoMapperConfig.FromUserDTOToUser;
				var mapper = new Mapper(config);

				var user = mapper.Map<User>(userDTO);
				user.Id = default; // Зануляем Id, чтобы БД не ругалась на то, что такой Id уже существует (рассчитает его сама БД)

				// Можем добавить, только если Email уникальный (не существует подобный в БД)
				bool isEmailUnique = _db.Users.Any(u => u.Email == user.Email) == false;
				if (isEmailUnique == false)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 403, Description = "Пользователь с таким Email уже существует. Ошибка" }
					);
				}

				await _db.Users.AddAsync(user);
				await _db.SaveChangesAsync();

				var userDTO2 = new Mapper(AutoMapperConfig.FromUserToUserDTO).Map<UserDTO>(user);

				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = userDTO2 }
				);
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

				var user = await _db.Users.Where(x => x.Id == id.Value).FirstOrDefaultAsync();
				if (user == null)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
					);
				}

				var config = AutoMapperConfig.FromUserDTOToUser;
				var mapper = new Mapper(config);
				var editedModel = mapper.Map<User>(userDTO);

				// Можем сменить, только на уникальный Email (не существует подобный в БД)
				bool isEmailUnique = _db.Users.Any(u => u.Email == editedModel.Email && u.Id != user.Id) == false;
				if (isEmailUnique == false)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 403, Description = "Пользователь с таким Email уже существует. Ошибка" }
					);
				}

				user.Email = editedModel.Email;
				user.Name = editedModel.Name;
				user.Surname = editedModel.Surname;
				user.Patronymic = editedModel.Patronymic;

				_db.Users.Update(user);
				await _db.SaveChangesAsync();

				var userDTO2 = new Mapper(AutoMapperConfig.FromUserToUserDTO).Map<UserDTO>(user);

				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = userDTO2 }
				);
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

				var user = await _db.Users.Where(x => x.Id == id.Value).FirstOrDefaultAsync();
				if (user == null)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
					);
				}

				_db.Users.Remove(user);
				await _db.SaveChangesAsync();

				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно" }
				);
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

			// Рассчитываем начальный и конечный день указанного месяца
			DateTime firstDayOfMonth = new DateTime(year, month.Value, 1);
			int days = DateTime.DaysInMonth(year, month.Value);
			DateTime lastDayOfMonth = new DateTime(year, month.Value, days, hour: 23, minute: 59, second: 59);

			// Существует пользователь или нет
			bool isUserExist = _db.Users.Any(x => x.Id == id.Value);
			if (isUserExist == false)
			{
				return new JsonResult(
						new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
					);
			}

			// Отчеты, которые были написаны в указанном месяце (с 1 по последний день включительно)
			var reportIds = await _db.Reports
				.Include(x => x.User)
				.Where(x => x.User.Id == id.Value && x.Date >= firstDayOfMonth && x.Date <= lastDayOfMonth)
				.Select(x => new { x.Id })
				.ToListAsync();

			return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = reportIds }
				);
		}
	}
}
