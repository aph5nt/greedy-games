namespace Shared.Providers
{
    public interface IRemoteActorProvider : IActorProvider
    {
        string Address { get; }
    }
}