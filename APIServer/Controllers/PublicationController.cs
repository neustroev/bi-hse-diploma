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
using APIServer.Code.Attribute;

namespace APIServer.Controllers
{
	public class PublicationController : ApiController
	{
		private BookStoreEntities _db = new BookStoreEntities();

		public object Get(int? user = null, string query = "", int? category = null, int? genre = null, bool random = false, int? limit = null)
		{
			var output = _db.Publication.AsQueryable();

			if (user != null)
				output = output.Where(o => o.UserId == user);
			if (!string.IsNullOrWhiteSpace(query))
				output = output.Where(o => o.Name.Contains(query));
			if (category != null)
				output = output.Where(o => o.Category_Id == category);
			if (genre != null)
				output = output.Where(o => o.PublicationGenre.Any(x => x.Genre_Id == genre));
			if (random)
			{
				Random rand = new Random();
				var l = _db.Genre.ToList();
				var id = l[rand.Next(0, l.Count)].Id;
				output = output.Where(o => o.PublicationGenre.Any(x => x.Genre_Id == id)).OrderBy(o => Guid.NewGuid());
			}
			if (limit != null)
				output = output.Take((int)limit);
			List<object> obj = new List<object>();
			foreach (var item in output)
			{
				obj.Add(new
				{
					id = item.Id,
					name = item.Name,
					author = item.UserId,
					category = item.Category_Id,
					genre = item.PublicationGenre.Select(x => x.Genre_Id).ToList(),
					genre_name = item.PublicationGenre.Select(x => x.Genre.Name).ToList(),
					price = item.Chapter.Sum(x => x.Price),
					date = item.Date
				});
			}
			return obj;
		}

		[HttpGet]
		public object GenreRating(int genre, int top = 10)
		{
			return _db.Publication.Where(o => o.PublicationGenre.Any(x => x.Genre_Id == genre))
									.OrderByDescending(o => o.Chapter.Sum(x => x.Transaction.Count))
									.Take(top)
									.Select(o => new
									{
										id = o.Id,
										name = o.Name,
										author = o.UserId,
										category = o.Category_Id,
										price = o.Chapter.Sum(x => x.Price),
										date = o.Date,
										count = o.Chapter.Sum(x => x.Transaction.Count)
									}).ToList();
		}

		[TokenAuth]
		public object Post(string access_token, [FromBody]PubPost data)
		{
			int user_id = Convert.ToInt32(access_token.Split('-')[0]);
			try
			{
				if (string.IsNullOrWhiteSpace(data.name) || data.genre == null || data.genre.Count == 0 || data.category == 0)
					throw new HttpResponseException(Request.GenerateCustomError(ErrorEnum.InvalidQueryParameterValue));
				using (var transaction = new TransactionScope())
				{
					var pub = new Publication()
					{
						UserId = user_id,
						Name = data.name,
						Category_Id = data.category,
						Date = DateTime.Now
					};
					_db.Publication.Add(pub);
					_db.SaveChanges();
					for (int i = 0; i < data.genre.Count; i++)
					{
						_db.PublicationGenre.Add(new PublicationGenre()
						{
							Genre_Id = data.genre[i],
							Publication_Id = pub.Id
						});
					}
					_db.SaveChanges();
					transaction.Complete();
				}
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

		public class PubPost
		{
			public string name { get; set; }
			public int category { get; set; }
			public List<int> genre { get; set; }
		}
	}
}
