using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Contracts;

namespace Shared.Infrastructure
{
    public abstract class HandlerValidator<T> : IHandlerValidator<T> where T : ICommand
    {
        public List<string> Validate(T commad)
        {
            return CreateValidationPipeline(commad).SelectMany(validationGroupHandler => validationGroupHandler())
                .Where(errorMsg => !string.IsNullOrEmpty(errorMsg)).ToList();
        }
        protected abstract IEnumerable<Func<IEnumerable<string>>> CreateValidationPipeline(T command);
    }
}