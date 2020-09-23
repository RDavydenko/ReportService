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
	/// Маппинг-профиль для класса <see cref="User"/>
	/// </summary>
	public class UserMappingProfile : Profile
	{
		public UserMappingProfile()
		{
			// Из User в UserDTO
			CreateMap<User, UserDTO>();

			// Из UserDTO в User
			CreateMap<UserDTO, User>();

			// Из User в IdDTO
			CreateMap<User, IdDTO>();
		}
	}
}
