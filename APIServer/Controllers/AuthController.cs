﻿using APIServer.Code.ApiErrors;
using APIServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIServer.Code.Extensions;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Mvc;

namespace APIServer.Controllers
{
	public class AuthController : ApiController
	{
		private BookStoreEntities _db = new BookStoreEntities();

		public object Post([FromBody]dynamic data)
		{
			try
			{
				if (data == null)
					throw new HttpResponseException(Request.GenerateCustomError(ErrorEnum.InvalidQueryParameterValue));
				string email = data.email;
				string pss = data.password;

				var user = _db.User.FirstOrDefault(o => o.Email == email);
				if (user == null)
					throw new HttpResponseException(Request.GenerateCustomError(ErrorEnum.InvalidQueryParameterValue));
				if (GetMd5Hash(pss) != user.Password)
					throw new HttpResponseException(Request.GenerateCustomError(ErrorEnum.InvalidAuthenticationInfo));
				user.Salt = GenerateSalt();
				_db.SaveChanges();
				var mt = GetMd5Hash(user.Email + user.Salt);
				return new
				{
					access_token = user.Id.ToString() + '-' + mt,
					id = user.Id,
					name = user.Name
				};
			}
			catch (Exception ex)
			{
				return new
				{
					error = ex.Message,
					error2 = ex.InnerException.Message
				};
			}
		}

		public object Register([FromBody]dynamic data)
		{
			string email = data.email;
			string pss = data.password;
			string name = data.name;
			string birthday = data.birthday;
			DateTime birth = new DateTime();
			if (!DateTime.TryParse(birthday, CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out birth))
				birth = DateTime.Now;
			var hash = GetMd5Hash(pss);
			_db.User.Add(new User()
			{
				Balance = 1000,
				Birthdate = birth,
				Email = email,
				Name = name,
				Password = hash
			});
			try
			{
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

		private string GenerateSalt()
		{
			string str = "";
			Random rand = new Random();
			for (int i = 0; i < 8; i++)
			{
				str += (char)(rand.Next(0, 26) + 'a');
			}
			return str;
		}

		private string GetMd5Hash(string input)
		{
			byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
		}
	}
}
