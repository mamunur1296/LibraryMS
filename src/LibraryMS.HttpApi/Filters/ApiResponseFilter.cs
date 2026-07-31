using LibraryMS.Application.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LibraryMS.HttpApi.Filters;

public sealed class ApiResponseFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        // Wrap only successful responses (2xx status codes)
        var statusCode = context.HttpContext.Response.StatusCode;
        if (statusCode >= 200 && statusCode < 300)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var value = objectResult.Value;
                if (value != null)
                {
                    var valueType = value.GetType();
                    // Avoid double wrapping if it's already ApiResponse<T> or ApiErrorResponse
                    if ((valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(ApiResponse<>)) ||
                        valueType == typeof(ApiErrorResponse))
                    {
                        return;
                    }
                }

                var apiResponse = ApiResponse<object>.SuccessResult(value ?? new object());
                objectResult.Value = apiResponse;
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(ApiResponse<object>.SuccessResult(new object()));
            }
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}
