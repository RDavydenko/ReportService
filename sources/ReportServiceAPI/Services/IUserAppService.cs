using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ReportService.WebApi.DTOs;

namespace ReportService.WebApi.Services
{
	/// <summary>
	/// Интерфейс сервиса, работающего с пользователем
	/// </summary>
	public interface IUserAppService
	{
		/// <summary>
		/// Возвращает список идентификаторов пользователей
		/// </summary>
		/// <returns>Список идентификаторов пользователей</returns>
		Task<IEnumerable<IdDTO>> GetUsersIdsAsync();

		/// <summary>
		/// Возвращает детальную информацию о пользователе
		/// </summary>
		/// <param name="userId">Идентификатор пользователя</param>
		/// <returns>Модель данных <see cref="UserDTO"/></returns>
		Task<UserDTO> GetUserDetailsAsync(int userId);

		/// <summary>
		/// Добавляет нового пользователя
		/// </summary>
		/// <param name="addedUser">Новый пользователь</param>
		/// <returns>Модель добавленного пользователя</returns>
		Task<UserDTO> AddUserAsync(UserDTO addedUser);

		/// <summary>
		/// Редактирует информацию о существующем пользователе
		/// </summary>
		/// <param name="editedUser">Отредактированный пользователь</param>
		/// <returns>Модель отредактированного пользователя</returns>
		Task<UserDTO> EditUserAsync(UserDTO editedUser);

		/// <summary>
		/// Удалить пользователя
		/// </summary>
		/// <param name="userId">Идентификатор пользователя</param>
		/// <returns>Успешна операция или нет</returns>
		Task<bool> DeleteUserAsync(int userId);

		/// <summary>
		/// Получить список идентификаторов отчетов пользователя за конкретный месяц
		/// </summary>
		/// <param name="userId">Идентификатор пользователя</param>
		/// <param name="month">Месяц</param>
		/// <param name="year">Год</param>
		/// <returns>Список идентификаторов отчетов</returns>
		Task<IEnumerable<IdDTO>> GetReportsByMonthAsync(int userId, int month, int year);
	}
}
