using APIServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace APIServer.Controllers
{
	public class HomeController : Controller
	{
		private BookStoreEntities _db = new BookStoreEntities();

		public ActionResult Index()
		{
			return View();
		}

		public void Gen()
		{
			//List<User> users = new List<User>();
			//for (int i = 0; i < 50; i++)
			//{
			//	User user = 					new User()
			//	{
			//		Balance = 1000,
			//		Birthdate = new DateTime(1990,1,1),
			//		Email = "test@gmail.com",
			//		Name = ("Test#"  + i),
			//		Password = ("test#" + i),
			//	};
			//	_db.User.Add(user);
			//}
			//_db.SaveChanges();
			//var users = _db.User.ToList();
			//for (int i = 0; i < 100; i++)
			//{
			//	Random rand = new Random((int)DateTime.Now.Ticks);
			//	_db.Publication.Add(new Publication()
			//	{
			//		Category_Id = rand.Next(1, 3),
			//		Date = new DateTime(2013, rand.Next(1, 13), rand.Next(1, 29)),
			//		Name = ("TestPublication#" + i),
			//		UserId = users[rand.Next(1, 50)].Id,
			//	});
			//}
			//_db.SaveChanges();
			var pub = _db.Publication.ToList();
			//var gen = _db.Genre.ToList();
			//foreach (var item in pub)
			//{
			//	Random x = new Random((int)DateTime.Now.Ticks);
			//	for (int i = 0; i < x.Next(1, 4); i++)
			//	{
			//		_db.PublicationGenre.Add(new PublicationGenre()
			//		{
			//			Genre_Id = gen[x.Next(0, gen.Count)].Id,
			//			Publication_Id = item.Id
			//		});
			//	}
			//}
			//_db.SaveChanges();

			//foreach (var item in pub)
			//{
			//	Random x = new Random((int)DateTime.Now.Ticks);
			//	for (int i = 0; i < x.Next(1, 9); i++)
			//	{
			//		_db.Chapter.Add(new Chapter()
			//		{
			//			Publication_Id = item.Id,
			//			Name = (item.Name + "-Chapter#" + i),
			//			Price = x.NextDouble() * 100
			//		});
			//	}
			//}
			//_db.SaveChanges();
			//var users = _db.User.ToList();
			//for (int i = 0; i < users.Count; i++)
			//{
			//	Random x = new Random((int)DateTime.Now.Ticks);
			//	for (int j = 0; j < x.Next(1, 5); j++)
			//	{
			//		DateTime date = new DateTime(2013, x.Next(1, 5), x.Next(1, 28));
			//		var cur = pub[x.Next(0, pub.Count)];
			//		for (int k = 0; k < x.Next(0, cur.Chapter.Count); k++) {
			//			_db.Transaction.Add(new Transaction()
			//			{
			//				Chapter_Id = cur.Chapter.ElementAt(k).Id,
			//				Date = date,
			//				Interest = cur.Chapter.ElementAt(k).Price*0.1,
			//				UserId = users[i].Id
			//			});
			//			date = date.AddDays(x.Next(1, 100));
			//		}

			//	}
			//}
			//_db.SaveChanges();

		}
	}
}
