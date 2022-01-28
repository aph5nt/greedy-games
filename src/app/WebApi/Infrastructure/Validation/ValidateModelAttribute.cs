using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Infrastructure.Validation
{
    #region

    #endregion

    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new ValidationFailedResult(context.ModelState);
            }
        }
    }
}