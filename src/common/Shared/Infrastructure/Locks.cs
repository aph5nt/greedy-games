namespace Shared.Infrastructure
{
    public static class Locks
    {
        public static string GetLock(string userName)
        {
            return $"{userName}";
        }
    }
}