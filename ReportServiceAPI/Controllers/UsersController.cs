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

		[HttpGet]
		public async Task<IActionResult> GetUsers()
		{
			var usersIds = await _db.Users.Select(x => new { x.Id }).ToListAsync();

			return new JsonResult(
				new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = usersIds }
				);
		}

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

		[HttpPost]
		[Route("add")]
		public async Task<IActionResult> AddUser(UserDTO userDTO)
		{
			if (ModelState.IsValid)
			{
				var config = AutoMapperConfig.FromUserDTOToUser;
				var mapper = new Mapper(config);

				var user = mapper.Map<User>(userDTO);

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

				bool isEmailUnique = _db.Users.Any(u => u.Email == editedModel.Email) == false;
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

			DateTime firstDayOfMonth = new DateTime(year, month.Value, 1);
			int days = DateTime.DaysInMonth(year, month.Value);
			DateTime lastDayOfMonth = new DateTime(year, month.Value, days, hour: 23, minute: 59, second: 59);

			var user = await _db.Users.Include(u => u.Reports).Where(u => u.Id == id.Value).FirstOrDefaultAsync();
			if (user == null)
			{
				return new JsonResult(
						new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
					);
			}

			var reports = user.Reports.Where(r => r.Date >= firstDayOfMonth && r.Date <= lastDayOfMonth).ToList();
			var mapper = new Mapper(AutoMapperConfig.FromReportToReportDTO);
			var reportsDTO = mapper.Map<IEnumerable<ReportDTO>>(reports);

			return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = reportsDTO }
				);
		}
	}
}
