using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApi.Infrastructure.Validation;

namespace WebApi.Controllers
{
    [ValidateModel]
    [Authorize(Policy = AppPolicy.GameUser)]
    public class BaseApiController : Controller
    {
        public const string BaseRouting = "api";

        public ObjectResult UnProcessableEntity(string message)
        {
            return new ObjectResult(new {errors = new[] { message }})
            {
                StatusCode = 422,
            };
        }

        public ObjectResult UnProcessableEntity(List<string> messages)
        {
            return new ObjectResult(new { errors = messages })
            {
                StatusCode = 422,
            };
        }

        public ObjectResult UnProcessableEntity(string propertyName, string message)
        {
            return new ObjectResult(new
            {
                errors = new[] {new ValidationError(ValidationResultModel.FirstCharToLower(propertyName), message)}
            })
            {
                StatusCode = 422
            };
        }

        public ObjectResult Forbidden(string message)
        {
            return new ObjectResult(new { error = message })
            {
                StatusCode = 403,
            };
        }
    }
}
