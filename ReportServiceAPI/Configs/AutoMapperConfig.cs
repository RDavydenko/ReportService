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

		private static Lazy<IConfigurationProvider> _fromUserDTOToUser= new Lazy<IConfigurationProvider>(new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()));

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
	}
}
