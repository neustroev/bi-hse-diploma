using APIServer.Code.ApiErrors;
using APIServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIServer.Code.Extensions;
using System.Transactions;
using System.Text;
using APIServer.Code.Attribute;

namespace APIServer.Controllers
{
	public class ChaptersController : ApiController
	{
		private BookStoreEntities _db = new BookStoreEntities();

		public object Get(int publication, int? id = null)
		{
			if (id == null)
			{
				return _db.Chapter.Where(o => o.Publication_Id == publication).Select(o => new
				{
					id = o.Id,
					name = o.Name,
					price = o.Price
				}).ToList();
			}
			else
			{
				var p = _db.Chapter.FirstOrDefault(o => o.Id == id);
				if (p == null)
					throw new HttpResponseException(Request.GenerateCustomError(ErrorEnum.InvalidQueryParameterValue));
				return new
				{
					id = p.Id,
					name = p.Name,
					price = p.Price,
					content = Encoding.UTF8.GetString(p.Content)
				};
			}
		}

		[TokenAuth]
		public object Post(string access_token, [FromBody]ChPost data)
		{
			int user_id = Convert.ToInt32(access_token.Split('-')[0]);
			try
			{
				if (string.IsNullOrWhiteSpace(data.name) || data.publication_id == 0 || string.IsNullOrWhiteSpace(data.content))
					throw new HttpResponseException(Request.GenerateCustomError(ErrorEnum.InvalidQueryParameterValue));
				var chapter = new Chapter()
				{
					Name = data.name,
					Publication_Id = data.publication_id,
					Price = data.price,
					Content = Encoding.UTF8.GetBytes(data.content)
				};
				_db.Chapter.Add(chapter);
				_db.SaveChanges();
				return new
				{
					succss = true
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

		public class ChPost
		{
			public string name { get; set; }
			public int publication_id { get; set; }
			public string content { get; set; }
			public double price { get; set; }
		}
	}
}
