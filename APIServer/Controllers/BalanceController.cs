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
	public class BalanceController : ApiController
	{
		private BookStoreEntities _db = new BookStoreEntities();

		public object Post([FromBody]Req req)
		{
			var u = _db.User.FirstOrDefault(o => o.Id == req.user);
			if (u == null)
				throw new HttpResponseException(Request.GenerateCustomError(ErrorEnum.InvalidQueryParameterValue));
			try
			{
				u.Balance += req.balance;
				_db.SaveChanges();
				return new
				{
					success = true
				};
			}
			catch (Exception ex)
			{
				return new
				{
					success = false,
					error = ex.Message
				};
			}

		}

		public class Req
		{
			public int user { get; set; }
			public int balance { get; set; }
		}
	}
}
