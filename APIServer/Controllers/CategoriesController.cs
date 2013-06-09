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
	//[TokenAuth]
	public class CategoriesController : ApiController
	{
		private BookStoreEntities _db = new BookStoreEntities();

		public object Get(string ids = "")
		{
			if (!string.IsNullOrWhiteSpace(ids))
			{
				List<int> id = ids.Split(',').Select(o => Convert.ToInt32(o)).ToList();
				return _db.Category.Where(o => id.Contains(o.Id)).Select(o => new
						{
							id = o.Id,
							name = o.Name
						});
			}
			else
			{
				return _db.Category.Select(o => new
				{
					id = o.Id,
					name = o.Name
				});
			}
		}
	}
}
