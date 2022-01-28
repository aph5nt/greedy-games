using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApi.Infrastructure.Validation
{
    #region

    #endregion

    public class ValidationResultModel
    {
        public ValidationResultModel(ModelStateDictionary modelState)
        {
            if (modelState != null)
            {
                Errors = modelState.Keys.SelectMany(
                        key => modelState[key].Errors
                            .Select(x => new ValidationError(FirstCharToLower(key), x.ErrorMessage)))
                    .ToList();
            }
        }

        public List<ValidationError> Errors { get; }

        public static string FirstCharToLower(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return char.ToLowerInvariant(text[0]) + text.Substring(1);
        }
    }
}