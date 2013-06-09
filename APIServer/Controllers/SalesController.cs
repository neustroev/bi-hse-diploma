using APIServer.Code.Attribute;
using APIServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace APIServer.Controllers
{
	[TokenAuth]
    public class SalesController : ApiController
    {
		private BookStoreEntities _db = new BookStoreEntities();

		public object Get(string access_token)
		{
			int user_id = Convert.ToInt32(access_token.Split('-')[0]);
			return _db.Transaction.Where(o => o.Chapter.Publication.UserId == user_id).Select(o => new
			{
				id = o.Id,
				price = o.Chapter.Price,
				date = o.Date,
				name = o.Chapter.Name
			}).ToList();
		}
    }
}
