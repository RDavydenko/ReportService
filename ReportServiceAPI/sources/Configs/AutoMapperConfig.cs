using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using ReportServiceAPI.sources.DTOs;
using ReportServiceAPI.sources.Models;

namespace ReportServiceAPI.sources.Configs
{
	/// <summary>
	/// Конфигурация для AutoMapper'а
	/// </summary>
	public static class AutoMapperConfig
	{
		private static Lazy<IConfigurationProvider> _fromUserToUserDTO = new Lazy<IConfigurationProvider>(new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()));

		/// <summary>
		/// Конфигурация для перехода из <see cref="User"/> в <see cref="UserDTO"/>
		/// </summary>
		public static IConfigurationProvider FromUserToUserDTO
		{
			get
			{
				return _fromUserToUserDTO.Value;
			}
		}

		private static Lazy<IConfigurationProvider> _fromUserDTOToUser = new Lazy<IConfigurationProvider>(new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()));

		/// <summary>
		/// Конфигурация для перехода из <see cref="UserDTO"/> в <see cref="User"/>
		/// </summary>
		public static IConfigurationProvider FromUserDTOToUser
		{
			get
			{
				return _fromUserDTOToUser.Value;
			}
		}

		private static Lazy<IConfigurationProvider> _fromReportToReportDTO = new Lazy<IConfigurationProvider>(new MapperConfiguration(cfg =>
		{
			cfg.CreateMap<Report, ReportDTO>()
				.ForMember(x => x.UserId, opt =>
				{
					// Если User не NULL, то записываем в поле UserId значение User.Id
					opt.Condition(x => x.User != null);
					opt.MapFrom(x => x.User.Id);
				});

		}));

		/// <summary>
		/// Конфигурация для перехода из <see cref="Report"/> в <see cref="ReportDTO"/>
		/// </summary>
		public static IConfigurationProvider FromReportToReportDTO
		{
			get
			{
				return _fromReportToReportDTO.Value;
			}
		}

		private static Lazy<IConfigurationProvider> _fromReportDTOToReport = new Lazy<IConfigurationProvider>(new MapperConfiguration(cfg =>
		{
			cfg.CreateMap<ReportDTO, Report>()
				// Создаем нового пользователя с полем Id из ReportDTO.UserId
				.ForMember(x => x.User, opt => opt.MapFrom(x => new User { Id = x.UserId.GetValueOrDefault() }))
				// Если в Date NULL, то берем системное время, иначе время, указанное в Date
				.ForMember(x => x.Date, opt => opt.MapFrom((rDTO, r) => rDTO.Date.HasValue ? rDTO.Date.Value : DateTime.Now));				
		}));

		/// <summary>
		/// Конфигурация для перехода из <see cref="ReportDTO"/> в <see cref="Report"/>
		/// </summary>
		public static IConfigurationProvider FromReportDTOToReport
		{
			get
			{
				return _fromReportDTOToReport.Value;
			}
		}
	}
}
