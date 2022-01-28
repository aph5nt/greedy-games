namespace Shared.Infrastructure
{
  
    public interface IAdapter<in TKey>
    {
        bool IsSupported(TKey type);
    }
}