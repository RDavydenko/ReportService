using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ReportService.WebApi.DTOs;
using ReportService.WebApi.Exceptions;
using ReportService.WebApi.Models;

namespace ReportService.WebApi.Services
{
	/// <summary>
	/// Реализация сервиса, работающего с пользователем
	/// </summary>
	public class UserAppService : IUserAppService
	{
		private readonly ServiceDbContext _db;
		private readonly IMapper _mapper;
		private readonly ILogger<UserAppService> _logger;

		public UserAppService(ServiceDbContext db, IMapper mapper, ILogger<UserAppService> logger)
		{
			_db = db;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<IEnumerable<UserDTO>> GetUsersAsync()
		{
			try
			{				
				var users = await _db.Users.AsNoTracking().ToListAsync();
				var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);
				return usersDTO;
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex, "Возникло исключение при получении списка пользователей");
				throw;
			}
		}

		public async Task<UserDTO> GetUserDetailsAsync(int userId)
		{
			try
			{
				var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
				if (user == null)
				{
					throw new EntityNotFoundException($"Не найден пользователь с идентификатором {userId}");
				}
				else
				{
					var userDTO = _mapper.Map<UserDTO>(user);
					return userDTO;
				}
			}
			catch (EntityNotFoundException)
			{
				// Не логируем, т.к. не является как таковой ошибкой в работе
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex, $"Возникло исключение при получении детальной информации о пользователе c id = {userId}");
				throw;
			}
		}

		public async Task<UserDTO> AddUserAsync(UserDTO addedUser)
		{
			try
			{
				var user = _mapper.Map<User>(addedUser);
				user.Id = 0; // Зануляем Id, чтобы БД не ругалась на то, что такой Id уже существует (рассчитает его сама БД)

				// Проверка уникальности Email
				bool emailUnique = !(await _db.Users.AnyAsync(x => x.Email == addedUser.Email));
				if (emailUnique == false)
				{
					throw new UniqueConstraintException("", nameof(User.Email));
				}

				await _db.Users.AddAsync(user);
				await _db.SaveChangesAsync();
				var userDTO = _mapper.Map<UserDTO>(user);
				return userDTO;
			}
			catch (UniqueConstraintException)
			{
				// Не логируем, т.к. не является как таковой ошибкой в работе
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex, $"Возникло исключение при добавлении нового пользователя");
				throw;
			}
		}

		public async Task<UserDTO> EditUserAsync(UserDTO editedUser)
		{
			try
			{
				bool userExists = await _db.Users.AnyAsync(x => x.Id == editedUser.Id);
				if (userExists == false)
				{
					throw new EntityNotFoundException($"Не найден пользователь с идентификатором {editedUser.Id}");
				}
				else
				{
					// Проверка уникальности нового (отредактированного) Email
					bool emailUniqueOrOwner = !(await _db.Users.AnyAsync(x => x.Email == editedUser.Email && x.Id != editedUser.Id));
					if (emailUniqueOrOwner == false)
					{
						throw new UniqueConstraintException("", nameof(User.Email));
					}

					var user = await _db.Users.FirstAsync(x => x.Id == editedUser.Id);
					var model = _mapper.Map<User>(editedUser);
					if (!string.IsNullOrWhiteSpace(model.Name) && !string.IsNullOrEmpty(model.Name))
					{
						user.Name = model.Name;
					}
					if (!string.IsNullOrWhiteSpace(model.Surname) && !string.IsNullOrEmpty(model.Surname))
					{
						user.Surname = model.Surname;
					}
					if (!string.IsNullOrWhiteSpace(model.Patronymic) && !string.IsNullOrEmpty(model.Patronymic))
					{
						user.Patronymic = model.Patronymic;
					}
					if (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrEmpty(model.Email))
					{
						user.Email = model.Email;
					}
					_db.Users.Update(user);
					await _db.SaveChangesAsync();

					var editedDTO = _mapper.Map<UserDTO>(user);
					return editedDTO;
				}
			}
			catch (EntityNotFoundException)
			{
				// Не логируем, т.к. не является как таковой ошибкой в работе
				throw;
			}
			catch (UniqueConstraintException)
			{
				// Не логируем, т.к. не является как таковой ошибкой в работе
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex, $"Возникло исключение при редактировании пользователя с id = {editedUser.Id}");
				throw;
			}
		}

		public async Task<bool> DeleteUserAsync(int userId)
		{
			try
			{
				bool userExists = await _db.Users.AnyAsync(x => x.Id == userId);
				if (userExists == false)
				{
					return false;
				}
				else
				{
					var deletedUser = await _db.Users.FirstAsync(x => x.Id == userId);
					_db.Users.Remove(deletedUser);
					await _db.SaveChangesAsync();
					return true;
				}
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex, $"Возникло исключение при удалении пользвателя с id = {userId}");
				throw;
			}
		}


		public async Task<IEnumerable<IdDTO>> GetReportsByMonthAsync(int userId, int month, int year = 2020)
		{
			try
			{
				// Рассчитываем начальный и конечный день указанного месяца
				DateTime firstDayOfMonth = new DateTime(year, month, 1);
				int days = DateTime.DaysInMonth(year, month);
				DateTime lastDayOfMonth = new DateTime(year, month, days, hour: 23, minute: 59, second: 59);

				bool userExists = _db.Users.Any(x => x.Id == userId);
				if (userExists == false)
				{
					throw new EntityNotFoundException($"Не найден пользователь с идентификатором {userId}");
				}
				var reports = await _db.Reports
					.AsNoTracking()
					.Include(x => x.User)
					.Where(x => x.User.Id == userId)
					.Where(x => x.Date >= firstDayOfMonth && x.Date <= lastDayOfMonth)
					.ToListAsync();

				var reportsDTO = _mapper.Map<IEnumerable<IdDTO>>(reports);
				return reportsDTO;
			}
			catch (EntityNotFoundException)
			{
				// Не логируем, т.к. не является как таковой ошибкой в работе
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex, $"Возникло исключение при получении списка отчетов у пользователя с id = {userId} за месяц {month} год {year}");
				throw;
			}
		}
	}
}
