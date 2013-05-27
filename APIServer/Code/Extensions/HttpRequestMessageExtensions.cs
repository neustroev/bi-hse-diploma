using APIServer.Code.ApiErrors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace APIServer.Code.Extensions
{
	public static class HttpRequestMessageExtensions
	{
		public static HttpResponseMessage GenerateCustomError(this HttpRequestMessage request, ErrorEnum error)
		{
			var customError = new HttpError();
			var statusCode = 400;
			var errorMessage = "";
			switch (error)
			{
				case ErrorEnum.MissingRequiredQueryParameter:
					{
						statusCode = 400;
						errorMessage = "A required query parameter was not specified for this request.";
						break;
					};
				case ErrorEnum.UnsupportedQueryParameter:
					{
						statusCode = 400;
						errorMessage = "One of the query parameters specified in the request URI is not supported.";
						break;
					};
				case ErrorEnum.InvalidQueryParameterValue:
					{
						statusCode = 400;
						errorMessage = "An invalid value was specified for one of the query parameters in the request URI.";
						break;
					};
				case ErrorEnum.OutOfRangeQueryParameterValue:
					{
						statusCode = 400;
						errorMessage = "A query parameter specified in the request URI is outside the permissible range.";
						break;
					};
				case ErrorEnum.RequestUrlFailedToParse:
					{
						statusCode = 400;
						errorMessage = "The url in the request could not be parsed.";
						break;
					};
				case ErrorEnum.InvalidUri:
					{
						statusCode = 400;
						errorMessage = "The requested URI does not represent any resource on the server.";
						break;
					};
				case ErrorEnum.InvalidAuthenticationInfo:
					{
						statusCode = 400;
						errorMessage = "The authentication information was not provided in the correct format. Verify the value of Authorization header.";
						break;
					};
				case ErrorEnum.AuthenticationFailed:
					{
						statusCode = 403;
						errorMessage = "The server failed to authenticate the request. Verify that the value of authorization header is formed correctly and includes the signature.";
						break;
					};
				case ErrorEnum.ResourceNotFound:
					{
						statusCode = 404;
						errorMessage = "The specified resource does not exist.";
						break;
					};
				case ErrorEnum.AccountIsDisabled:
					{
						statusCode = 403;
						errorMessage = "The specified account is disabled.";
						break;
					};
				case ErrorEnum.InsufficientAccountPermissions:
					{
						statusCode = 403;
						errorMessage = "The account being accessed does not have sufficient permissions to execute this operation.";
						break;
					};
				case ErrorEnum.InternalError:
					{
						statusCode = 500;
						errorMessage = "The server encountered an internal error. Please retry the request.";
						break;
					};
				case ErrorEnum.OperationTimedOut:
					{
						statusCode = 500;
						errorMessage = "The operation could not be completed within the permitted time.";
						break;
					};
				case ErrorEnum.ServerBusy:
					{
						statusCode = 503;
						errorMessage = "The server is currently unable to receive requests. Please retry your request.";
						break;
					};
				case ErrorEnum.AccountExisting:
					{
						statusCode = 400;
						errorMessage = "User with account's Facebook or account's LinkedIn already exists.";
						break;
					}

			}
			customError["status_code"] = statusCode.ToString();
			customError["error_code"] = error.ToString();
			customError["error"] = errorMessage;
			HttpStatusCode code = HttpStatusCode.InternalServerError;
			switch (statusCode)
			{
				case 400:
					{
						code = HttpStatusCode.BadRequest;
						break;
					}
				case 404:
					{
						code = HttpStatusCode.NotFound;
						break;
					}
				case 403:
					{
						code = HttpStatusCode.Forbidden;
						break;
					}
				case 503:
					{
						code = HttpStatusCode.ServiceUnavailable;
						break;
					}
			}
			return request.CreateErrorResponse(code, customError);
		}

		public static string GetAccessTokenFromUri(this HttpRequestMessage request)
		{
			var keyValue = request.GetQueryNameValuePairs();
			var token = keyValue.FirstOrDefault(o => o.Key == "access_token");
			if (token.Key != null)
			{
				return token.Value;
			}
			else
				throw new HttpResponseException(request.GenerateCustomError(ErrorEnum.InvalidAuthenticationInfo)); 
		}

	}
}