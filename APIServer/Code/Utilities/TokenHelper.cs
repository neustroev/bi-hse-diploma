using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;
using APIServer.Code.ApiErrors;
using APIServer.Models;
using System.Text;

namespace APIServer.Code.Utilities
{
	public class TokenHelper
	{
		public static string GetUserId(string token)
		{
			try
			{
				return token.Split('|')[0];
			}
			catch
			{
				return "";
			}
		}

		public static TokenState VerifyToken(string token)
		{
			try
			{
				BookStoreEntities db = new BookStoreEntities();
				var idx = token.IndexOf('-');
				if (idx < 0)
					return TokenState.Invalid;

				int id = Convert.ToInt32(token.Substring(0, idx));
				string secret = token.Substring(idx + 1);
				var user = db.User.FirstOrDefault(o => o.Id == id);
				if (user == null)
					return TokenState.Invalid;
				var mt = GetMd5Hash(user.Email + user.Salt);
				if (secret == mt)
					return TokenState.Valid;
				return TokenState.Invalid;
			}
			catch
			{
				return TokenState.Invalid;
			}
		}

		public static string GetTokenFromUri(string uri)
		{
			return "";
		}

		public static string GetMd5Hash(string input)
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