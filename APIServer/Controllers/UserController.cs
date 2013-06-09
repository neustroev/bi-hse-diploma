using APIServer.Code.ApiErrors;
using APIServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIServer.Code.Extensions;
using APIServer.Code.Attribute;

namespace APIServer.Controllers
{
	public class UserController : ApiController
	{
		private BookStoreEntities _db = new BookStoreEntities();

		[TokenAuth]
		public object Get(int id)
		{
			var user = _db.User.FirstOrDefault(o => o.Id == id);
			if (user == null)
				throw new HttpResponseException(Request.GenerateCustomError(ErrorEnum.InvalidQueryParameterValue));
			return new
			{
				id = user.Id,
				name = user.Name,
				age = (int)((DateTime.Now - user.Birthdate).TotalDays / 365.255),
				balance = user.Balance
			};
		}

		[HttpGet]
		public object WriterRating(int top = 10)
		{
			return _db.User.OrderByDescending(o => o.Transaction.Where(x => x.UserId == o.Id).Count()).Take(top).Select(o => new
			{
				id = o.Id,
				name = o.Name,
				count = o.Transaction.Where(x => x.UserId == o.Id).Count()
			}).ToList();
		}
	}
}
