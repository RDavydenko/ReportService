using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using ReportServiceAPI.sources.DTOs;
using ReportServiceAPI.sources.Exceptions;
using ReportServiceAPI.sources.Models;

namespace ReportServiceAPI.sources.Services
{

	public class UserWebService : IUserWebService
	{
		private readonly ServiceDbContext _db;
		private readonly IMapper _mapper;

		public UserWebService(ServiceDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
		}

		public async Task<IEnumerable<IdDTO>> GetUsersIdsAsync()
		{
			List<User> users = null;
			try
			{
				users = await _db.Users.AsNoTracking().ToListAsync();
			}
			catch
			{
				throw;
			}
			var usersDTO = _mapper.Map<IEnumerable<IdDTO>>(users);
			return usersDTO;
		}

		public async Task<UserDTO> GetUserDetailsAsync(int userId)
		{
			User user = null;
			try
			{
				user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
			}
			catch
			{
				throw;
			}

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

		public async Task<UserDTO> AddUserAsync(UserDTO addedUser)
		{
			try
			{
				var user = _mapper.Map<User>(addedUser);
				user.Id = 0; // Зануляем Id, чтобы БД не ругалась на то, что такой Id уже существует (рассчитает его сама БД)

				// Проверка уникальности Email
				bool emailUnique = !(await _db.Users.AnyAsync(x => x.Email == addedUser.Email));
				if (emailUnique ==false)
				{
					throw new UniqueConstraintException("", nameof(User.Email));
				}

				await _db.Users.AddAsync(user);
				await _db.SaveChangesAsync();
				var userDTO = _mapper.Map<UserDTO>(user);
				return userDTO;
			}
			catch
			{
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
			catch
			{
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
			catch
			{
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
			catch
			{
				throw;
			}
		}

	}
}
