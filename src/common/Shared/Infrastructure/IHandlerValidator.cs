using System.Collections.Generic;
using Shared.Contracts;

namespace Shared.Infrastructure
{
    public interface IHandlerValidator<in T> where T : ICommand
    {
        List<string> Validate(T commad);
    }
}