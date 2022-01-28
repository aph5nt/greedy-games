namespace Shared.Model
{
    public class Money
    {
        public const long Sathoshi = 100000000;

        public const long Cent = 1000000;

        public const string CentPrefix = "&cent;";

        public static long CentToSatoshi(long value)
        {
            return value * Cent;
        }

        public static long SatoshiToCent(long value)
        {
            return value / Cent;
        }

        public static string DisplayAsCents(long satoshis)
        {
            return SatoshiToCent(satoshis) + CentPrefix;
        }
    }
}