using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Brunozec.Common.Extensions.Http.Filters;

public sealed class BFZValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errorsInModelState = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage).ToArray());

            var errorResponse = new ErrorResponse();

            foreach (var error in errorsInModelState)
            {
                foreach (var subError in error.Value)
                {
                    var errorModel = new ErrorModel
                    {
                        FieldName = error.Key
                        , Message = subError
                    };

                    errorResponse.ErrorModels.Add(errorModel);
                    errorResponse.Errors.Add(subError);
                }

                context.Result = new BadRequestObjectResult(errorResponse);
                return;
            }
        }

        await next();
    }

    public sealed class ErrorModel
    {
        public string FieldName { get; set; }
        public string Message { get; set; }
    }

    public sealed class ErrorResponse
    {
        public List<ErrorModel> ErrorModels { get; set; } = new List<ErrorModel>();
        public List<string> Errors { get; set; } = new List<string>();

        public bool IsValid { get; set; }
    }
}