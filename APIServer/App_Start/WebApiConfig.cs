﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace APIServer
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.Routes.MapHttpRoute(
				name: "register",
				routeTemplate: "register",
				defaults: new { controller = "auth", action = "register"}
			);

			config.Routes.MapHttpRoute("DefaultApiGet",
				"{controller}/{id}",
				new { id = RouteParameter.Optional, action = "Get" },
				new { method = new System.Web.Http.Routing.HttpMethodConstraint(HttpMethod.Get, HttpMethod.Options) });

			config.Routes.MapHttpRoute("DefaultApiPost",
				"{controller}/{id}",
				new { id = RouteParameter.Optional, action = "Post" },
				new { method = new System.Web.Http.Routing.HttpMethodConstraint(HttpMethod.Post, HttpMethod.Options) });


			// Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
			// To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
			// For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
			//config.EnableQuerySupport();

			// To disable tracing in your application, please comment out or remove the following line of code

			// For more information, refer to: http://www.asp.net/web-api

			config.EnableSystemDiagnosticsTracing();
		}
	}
}
