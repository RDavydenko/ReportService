using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using ReportServiceAPI.DTOs;
using ReportServiceAPI.Models;

namespace ReportServiceAPI.Configs
{
	/// <summary>
	/// Маппинг-профиль для класса <see cref="Report"/>
	/// </summary>
	public class ReportMappingProfile : Profile
	{
		public ReportMappingProfile()
		{
			// Из Report в ReportDTO
			CreateMap<Report, ReportDTO>()
				.ForMember(x => x.UserId, opt =>
				{
					// Если User не NULL, то записываем в поле UserId значение User.Id
					opt.Condition(x => x.User != null);
					opt.MapFrom(x => x.User.Id);
				});

			// Из ReportDTO в Report
			CreateMap<ReportDTO, Report>()
				// Создаем нового пользователя с полем Id из ReportDTO.UserId
				.ForMember(x => x.User, opt => opt.MapFrom(x => new User { Id = x.UserId.GetValueOrDefault() }))
				// Если в Date NULL, то берем системное время, иначе время, указанное в Date
				.ForMember(x => x.Date, opt => opt.MapFrom((rDTO, r) => rDTO.Date.HasValue ? rDTO.Date.Value : DateTime.Now)); ;

			// Из Report в IdDTO
			CreateMap<Report, IdDTO>();
		}
	}
}
