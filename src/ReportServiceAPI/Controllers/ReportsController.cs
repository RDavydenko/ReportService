using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Http;
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
	/// Контроллер для работы с отчетами
	/// </summary>
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class ReportsController : ControllerBase
	{	
		private readonly IReportAppService _reportWebService;
		private readonly ILogger<ReportsController> _logger; 

		public ReportsController(IReportAppService reportWebService, ILogger<ReportsController> logger)
		{			
			_reportWebService = reportWebService;
			_logger = logger;
		}

		/// <summary>
		/// Получить список отчетов
		/// </summary>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = список отчетов</returns>
		[ProducesResponseType(typeof(Response), 200)]
		[HttpGet]
		public async Task<IActionResult> GetReports()
		{
			try
			{
				var reports = await _reportWebService.GetReportsAsync();
				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = reports }
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
		/// Получить детальную информацию об отчете
		/// </summary>
		/// <param name="id">Идентификатор отчета</param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO отчета <see cref="ReportDTO"/></returns>
		[ProducesResponseType(typeof(Response), 200)]
		[HttpGet]
		[Route("{id}")]		
		public async Task<IActionResult> GetReport(int? id)
		{
			if(id.HasValue == false)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 403, Description = "Неверный переданный параметр - " + nameof(id) }
				);
			}
			try
			{
				var reportDTO = await _reportWebService.GetReportDetailsAsync(id.Value);
				return new JsonResult(
					new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = reportDTO }
				);
			}
			catch (EntityNotFoundException)
			{
				return new JsonResult(
					new Response { Ok = false, StatusCode = 404, Description = "Отчет с таким id не найден" }
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
		/// Добавить новый отчет
		/// </summary>
		/// <param name="reportDTO">Объект типа <see cref="ReportDTO"/></param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO добавленного отчета <see cref="ReportDTO"/></returns>
		[ProducesResponseType(typeof(Response), 200)]
		[HttpPost]
		[Route("add")]
		public async Task<IActionResult> AddReport(ReportDTO reportDTO)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var addedReportDTO = await _reportWebService.AddReportAsync(reportDTO);
					return new JsonResult(
						new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = addedReportDTO }
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
		/// Редактировать данные отчета
		/// </summary>
		/// <param name="id">Идентификатор отчета</param>
		/// <param name="reportDTO">Объект типа <see cref="ReportDTO"/></param>
		/// <returns><see cref="Response"/> с <see cref="Response.Object"/> = DTO отредактированного отчета <see cref="ReportDTO"/></returns>
		[ProducesResponseType(typeof(Response), 200)]
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

				reportDTO.Id = id.Value;
				try
				{
					var editedDTO = await _reportWebService.EditReportAsync(reportDTO);
					return new JsonResult(
						new Response { Ok = true, StatusCode = 200, Description = "Успешно", Object = editedDTO }
					);
				}				
				catch (EntityNotFoundException ex)
				{
					return new JsonResult(
						new Response { Ok = false, StatusCode = 404, Description = ex.Message }
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
		/// Удалить отчет
		/// </summary>
		/// <param name="id">Идентификатор отчета</param>
		/// <returns><see cref="Response"/></returns>
		[ProducesResponseType(typeof(Response), 200)]
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
				try
				{
					bool isDeleted = await _reportWebService.DeleteReportAsync(id.Value);
					if (isDeleted == false) // Не удален
					{
						return new JsonResult(
							new Response { Ok = false, StatusCode = 404, Description = "Отчет с таким id не найден" }
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
	}
}
