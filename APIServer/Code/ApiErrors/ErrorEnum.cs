using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIServer.Code.ApiErrors
{
	public enum ErrorEnum
	{
		MissingRequiredQueryParameter,
		UnsupportedQueryParameter,
		InvalidQueryParameterValue,
		OutOfRangeQueryParameterValue,
		RequestUrlFailedToParse,
		InvalidUri,
		InvalidAuthenticationInfo,
		AuthenticationFailed,
		ResourceNotFound,
		AccountIsDisabled,
		InsufficientAccountPermissions,
		InternalError,
		OperationTimedOut,
		ServerBusy,
		None,
		AccountExisting
	}
}