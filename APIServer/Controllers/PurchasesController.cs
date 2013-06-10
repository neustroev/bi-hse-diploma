using APIServer.Code.Attribute;
using APIServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIServer.Code.Extensions;
using APIServer.Code.ApiErrors;
using System.Transactions;

namespace APIServer.Controllers
{
	[TokenAuth]
	public class PurchasesController : ApiController
	{
		private BookStoreEntities _db = new BookStoreEntities();

		public object Get(string access_token)
		{
			int user_id = Convert.ToInt32(access_token.Split('-')[0]);
			return _db.Transaction.Where(o => o.UserId == user_id).Select(o => new
			{
				id = o.Id,
				price = o.Chapter.Price,
				date = o.Date,
				name = o.Chapter.Name
			}).ToList();
		}

		public object Post(string access_token, [FromBody]dynamic data)
		{
			try
			{

				int user_id = Convert.ToInt32(access_token.Split('-')[0]);
				var user = _db.User.FirstOrDefault(o => o.Id == user_id);
				string d = data.chapter;
				int ch = 0;
				if (user == null || !int.TryParse(d, out ch))
					throw new HttpResponseException(Request.GenerateCustomError(ErrorEnum.InvalidQueryParameterValue));
				var chapter = _db.Chapter.FirstOrDefault(o => o.Id == ch);
				if (chapter == null)
					throw new HttpResponseException(Request.GenerateCustomError(ErrorEnum.InvalidQueryParameterValue));
				if (user.Balance < chapter.Price)
					return new
					{
						success = false,
						error = "Недостаточно денег"
					};
				using (var transaction = new TransactionScope())
				{
					_db.Transaction.Add(new APIServer.Models.Transaction()
						{
							Chapter_Id = ch,
							Date = DateTime.Now,
							UserId = user_id,
							Interest = chapter.Price * 0.1
						});
					_db.SaveChanges();
					user.Balance -= chapter.Price;
					_db.SaveChanges();
					transaction.Complete();
				}
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
	}
}
