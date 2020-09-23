using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ReportServiceAPI.sources.DTOs;

namespace ReportServiceAPI.sources.Services
{
	/// <summary>
	/// Интерфейс сервиса, работающего с пользователем
	/// </summary>
	public interface IUserWebService
	{
		/// <summary>
		/// Возвращает список идентификаторов пользователей
		/// </summary>
		/// <returns>Список идентификаторов пользователей</returns>
		Task<IEnumerable<IdDTO>> GetUsersIdsAsync();

		/// <summary>
		/// Возвращает детальную информацию о пользователе или <see langword="null"/>
		/// </summary>
		/// <param name="userId">Идентификатор пользователя</param>
		/// <returns>Модель данных <see cref="UserDTO"/></returns>
		Task<UserDTO> GetUserDetailsOrDefaultAsync(int userId);

		/// <summary>
		/// Добавляет нового пользователя
		/// </summary>
		/// <param name="addedUser">Новый пользователь</param>
		/// <returns>Успешна операция или нет</returns>
		Task<bool> AddUserAsync(UserDTO addedUser);

		/// <summary>
		/// Редактирует информацию о существующем пользователе
		/// </summary>
		/// <param name="editedUser">Отредактированный пользователь</param>
		/// <returns>Успешна операция или нет</returns>
		Task<bool> EditUserAsync(UserDTO editedUser);

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
