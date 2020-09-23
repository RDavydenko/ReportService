using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using ReportServiceAPI.DTOs;
using ReportServiceAPI.Exceptions;
using ReportServiceAPI.Models;

namespace ReportServiceAPI.Services
{
	/// <summary>
	/// Реализация сервиса, работающего с отчетами
	/// </summary>
	public class ReportWebService : IReportWebService
	{
		private readonly ServiceDbContext _db;
		private readonly IMapper _mapper;

		public ReportWebService(ServiceDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
		}

		public async Task<IEnumerable<IdDTO>> GetReportIdsAsync()
		{
			try
			{
				var reports = await _db.Reports.AsNoTracking().ToListAsync();
				var reportsDTO = _mapper.Map<IEnumerable<IdDTO>>(reports);
				return reportsDTO;
			}
			catch
			{
				throw;
			}
		}

		public async Task<ReportDTO> GetReportDetailsAsync(int reportId)
		{
			try
			{
				var report = await _db.Reports
					.Include(x => x.User)
					.AsNoTracking()
					.FirstOrDefaultAsync(x => x.Id == reportId);
				if (report == null)
				{
					throw new EntityNotFoundException($"Не найден отчет с идентификатором {reportId}");
				}
				else
				{
					var reportDTO = _mapper.Map<ReportDTO>(report);
					return reportDTO;
				}
			}
			catch
			{
				throw;
			}
		}

		public async Task<ReportDTO> AddReportAsync(ReportDTO added)
		{
			try
			{
				var report = _mapper.Map<Report>(added);
				report.Id = 0; // Зануляем Id, чтобы БД не ругалась на то, что такой Id уже существует (рассчитает его сама БД)

				var userExists = await _db.Users.AnyAsync(u => u.Id == report.User.Id);
				if (userExists == false) // Пытаемся установить отчету пользователя, которого не существует
				{
					throw new EntityNotFoundException("Пользователь с таким идентификатором не существует.");
				}

				var user = await _db.Users.FirstAsync(x => x.Id == report.User.Id);
				report.User = user;

				await _db.Reports.AddAsync(report);
				await _db.SaveChangesAsync();

				var reportDTO = _mapper.Map<ReportDTO>(report);
				return reportDTO;
			}
			catch
			{
				throw;
			}
		}

		public async Task<ReportDTO> EditReportAsync(ReportDTO edited)
		{
			try
			{
				bool reportExists = await _db.Reports.AnyAsync(x => x.Id == edited.Id);
				if (reportExists == false)
				{
					throw new EntityNotFoundException($"Не найден отчет с идентификатором {edited.Id}");
				}
				else
				{
					var report = await _db.Reports.FirstAsync(x => x.Id == edited.Id);
					var model = _mapper.Map<Report>(edited);

					var userExists = await _db.Users.AnyAsync(u => u.Id == model.User.Id);
					if (userExists == false) // Пытаемся установить отчету пользователя, которого не существует
					{
						throw new EntityNotFoundException("Пользователь с таким идентификатором не существует.");
					}
					else // Пользователь существует
					{
						var user = await _db.Users.FirstAsync(x => x.Id == model.User.Id);
						report.User = user;
					}

					if (!string.IsNullOrWhiteSpace(model.Remark) && !string.IsNullOrEmpty(model.Remark))
					{
						report.Remark = model.Remark;
					}

					// Добавляем дату только, если она была передана в теле запроса, иначе добавится системное время
					// Можно и не добавлять эту проверку, если цель - изменять время на время редактирования
					if (edited.Date.HasValue)
					{
						report.Date = edited.Date.Value;
					}
					report.Hours = model.Hours;

					_db.Reports.Update(report);
					await _db.SaveChangesAsync();

					var editedDTO = _mapper.Map<ReportDTO>(report);
					return editedDTO;
				}
			}
			catch
			{
				throw;
			}
		}

		public async Task<bool> DeleteReportAsync(int reportId)
		{
			try
			{
				bool reportExists = await _db.Reports.AnyAsync(x => x.Id == reportId);
				if (reportExists == false)
				{
					return false;
				}
				else
				{
					var deleted = await _db.Reports.FirstAsync(x => x.Id == reportId);
					_db.Reports.Remove(deleted);
					await _db.SaveChangesAsync();
					return true;
				}
			}
			catch
			{
				throw;
			}
		}
	}
}
