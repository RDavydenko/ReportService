using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ReportServiceAPI.Configs;
using ReportServiceAPI.DTOs;
using ReportServiceAPI.Models;

namespace ReportServiceAPI.Controllers
{
	/// <summary>
	/// Контроллер для работы с отчетами
	/// </summary>
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class ReportsController : ControllerBase
	{
		private readonly ServiceDbContext _db;

		public ReportsController(ServiceDbContext db)
		{
			_db = db;
		}

		/// <summary>
		/// Получить список отчетов
		/// </summary>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = список идентификаторов отчетов</returns>
		[HttpGet]
		public async Task<IActionResult> GetReports()
		{
			var reportIds = await _db.Reports.Select(r => new { r.Id }).ToListAsync();
			return new JsonResult(
				new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = reportIds }
				);
		}

		/// <summary>
		/// Получить детальную информацию об отчете
		/// </summary>
		/// <param name="id">Идентификатор отчета</param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO отчета <see cref="ReportDTO"/></returns>
		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> GetReport(int? id)
		{
			if (id.HasValue == false)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 403, Description = "Неверный переданный параметр - " + nameof(id) }
					);
			}

			var report = await _db.Reports
				.Include(x => x.User)
				.Where(x => x.Id == id.Value)
				.FirstOrDefaultAsync();
			if (report == null)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 404, Description = "Отчет с таким id не найден" }
					);
			}

			var config = AutoMapperConfig.FromReportToReportDTO;
			var mapper = new Mapper(config);

			var reportDTO = mapper.Map<ReportDTO>(report);
			return new JsonResult(
				new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = reportDTO }
				);
		}

		/// <summary>
		/// Добавить новый отчет
		/// </summary>
		/// <param name="reportDTO">Объект типа <see cref="ReportDTO"/></param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO добавленного отчета <see cref="ReportDTO"/></returns>
		[HttpPost]
		[Route("add")]
		public async Task<IActionResult> AddReport(ReportDTO reportDTO)
		{
			if (ModelState.IsValid)
			{
				var config = AutoMapperConfig.FromReportDTOToReport;
				var mapper = new Mapper(config);

				var report = mapper.Map<Report>(reportDTO);
				report.Id = default; // Зануляем Id, чтобы БД не ругалась на то, что такой Id уже существует (рассчитает его сама БД)

				var user = await _db.Users.Where(u => u.Id == report.User.Id).FirstOrDefaultAsync();
				if (user == null)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
					);
				}

				report.User = user;

				await _db.Reports.AddAsync(report);
				await _db.SaveChangesAsync();

				var reportDTO2 = new Mapper(AutoMapperConfig.FromReportToReportDTO).Map<ReportDTO>(report);

				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = reportDTO2 }
				);
			}
			return new JsonResult(
					new Response { Ok = false, StatusCode = 403, Description = "Переданные данные не прошли валидацию" }
				);
		}

		/// <summary>
		/// Редактировать данные отчета
		/// </summary>
		/// <param name="id">Идентификатор отчета</param>
		/// <param name="reportDTO">Объект типа <see cref="ReportDTO"/></param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO отредактированного отчета <see cref="ReportDTO"/></returns>
		[HttpPost]
		[Route("{id}/edit")]
		public async Task<IActionResult> EditReport(int? id, ReportDTO reportDTO)
		{
			if (ModelState.IsValid)
			{
				if (id.HasValue == false)
				{
					return new JsonResult(
						   new Response { Ok = false, StatusCode = 403, Description = "Неверный переданный параметр - " + nameof(id) }
					   );
				}

				var report = await _db.Reports.Where(x => x.Id == id.Value).FirstOrDefaultAsync();
				if (report == null)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 404, Description = "Отчет с таким id не найден" }
					);
				}	

				var config = AutoMapperConfig.FromReportDTOToReport;
				var mapper = new Mapper(config);
				var editedReport = mapper.Map<Report>(reportDTO);

				var user = await _db.Users.Where(u => u.Id == editedReport.User.Id).FirstOrDefaultAsync();
				if (user == null)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 404, Description = "Пользователь с таким id не найден" }
					);
				}

				report.Remark = editedReport.Remark;
				report.Hours = editedReport.Hours;
				report.Date = editedReport.Date;
				report.User = user;
				
				_db.Reports.Update(report);
				await _db.SaveChangesAsync();

				var reportDTO2 = new Mapper(AutoMapperConfig.FromReportToReportDTO).Map<ReportDTO>(report);

				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = reportDTO2 }
				);
			}
			return new JsonResult(
					new Response { Ok = false, StatusCode = 403, Description = "Переданные данные не прошли валидацию" }
				);
		}

		/// <summary>
		/// Удалить отчет
		/// </summary>
		/// <param name="id">Идентификатор отчета</param>
		/// <returns><see cref="Response"/></returns>
		[HttpPost]
		[Route("{id}/delete")]
		public async Task<IActionResult> DeleteReport(int? id)
		{
			if (ModelState.IsValid)
			{
				if (id.HasValue == false)
				{
					return new JsonResult(
						   new Response { Ok = false, StatusCode = 403, Description = "Неверный переданный параметр - " + nameof(id) }
					   );
				}

				var report = await _db.Reports.Where(x => x.Id == id.Value).FirstOrDefaultAsync();
				if (report == null)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 404, Description = "Отчет с таким id не найден" }
					);
				}

				_db.Reports.Remove(report);
				await _db.SaveChangesAsync();

				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно" }
				);
			}
			return new JsonResult(
					new Response { Ok = false, StatusCode = 403, Description = "Переданные данные не прошли валидацию" }
				);
		}
	}
}
