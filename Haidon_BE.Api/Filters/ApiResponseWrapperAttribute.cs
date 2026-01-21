using Haidon_BE.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Haidon_BE.Api.Filters;

public class ApiResponseWrapperAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            // N?u ?ã là ApiResponse thì không wrap l?i
            if (objectResult.Value is ApiResponse<object> || (objectResult.Value?.GetType().IsGenericType == true && objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>)))
                return;
            // N?u là NotFoundResult ho?c StatusCodeResult thì không wrap
            if (objectResult.StatusCode is >= 400 and < 600 && objectResult.Value == null)
                return;
            objectResult.Value = ApiResponse<object>.SuccessResponse(objectResult.Value);
        }
        else if (context.Result is EmptyResult)
        {
            context.Result = new ObjectResult(ApiResponse<object>.SuccessResponse(null));
        }
        base.OnResultExecuting(context);
    }
}
