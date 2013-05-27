using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using APIServer.Code.ApiErrors;
using APIServer.Code.Extensions;
using APIServer.Code.Utilities;

namespace APIServer.Code.Attribute
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class TokenAuthAttribute : System.Web.Http.Filters.AuthorizationFilterAttribute
	{
		public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			var query = actionContext.Request.GetQueryNameValuePairs();
			var token = query.FirstOrDefault(o => o.Key == "access_token");
			TokenState verify = TokenState.Invalid;
			if (token.Key == null)
			{
				throw new HttpResponseException(actionContext.Request.GenerateCustomError(ErrorEnum.InvalidAuthenticationInfo));
			}
			else
			{
				verify = TokenHelper.VerifyToken(token.Value);
			}

			if (verify == TokenState.Invalid)
			{
				throw new HttpResponseException(actionContext.Request.GenerateCustomError(ErrorEnum.InvalidAuthenticationInfo));
			}
			base.OnAuthorization(actionContext);
		}
	}
}