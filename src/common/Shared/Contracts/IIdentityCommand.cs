using Shared.Model;

namespace Shared.Contracts
{
    public interface IIdentityCommand : ICommand
    {
        Network Network { get; }
        string UserName { get; }
    }
}