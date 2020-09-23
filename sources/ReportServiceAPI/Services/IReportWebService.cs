using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ReportService.WebApi.DTOs;

namespace ReportService.WebApi.Services
{
	/// <summary>
	/// Интерфейс сервиса, работающего с отчетами
	/// </summary>
	public interface IReportWebService
	{
		/// <summary>
		/// Возвращает список идентификаторов отчетов
		/// </summary>
		/// <returns>Список идентификаторов отчетов</returns>
		Task<IEnumerable<IdDTO>> GetReportIdsAsync();

		/// <summary>
		/// Получить детальную информацию об отчете
		/// </summary>
		/// <param name="reportId">Идентификатор отчета</param>
		/// <returns>Детальная информация об отчете <see cref="ReportDTO"/></returns>
		Task<ReportDTO> GetReportDetailsAsync(int reportId);

		/// <summary>
		/// Добавить новый отчет
		/// </summary>
		/// <param name="added">Новый отчет</param>
		/// <returns>Добавленные отчет</returns>
		Task<ReportDTO> AddReportAsync(ReportDTO added);

		/// <summary>
		/// Редактировать существующий отчет
		/// </summary>
		/// <param name="edited">Отредактированный отчет</param>
		/// <returns>Отредактированный отчет</returns>
		Task<ReportDTO> EditReportAsync(ReportDTO edited);

		/// <summary>
		/// Удалить отчет
		/// </summary>
		/// <param name="reportId">Идентификатор отчета</param>
		/// <returns>Успешная операция или нет</returns>
		Task<bool> DeleteReportAsync(int reportId);
	}
}
