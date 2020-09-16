using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ReportServiceAPI.Models;

namespace ReportServiceAPI.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : Controller
	{
		private readonly ServiceDbContext _db;

		public UsersController(ServiceDbContext db)
		{
			_db = db;
		}

		[HttpGet]
		public IActionResult Get()
		{
			_db.Users.Count();
			_db.Users.Add(new User { Email = "admin@mail.ru", Name = "Вася", Surname = "Васин" });
			return Content("abc");
		}
	}
}
