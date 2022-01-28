using System.Globalization;

namespace Shared.Infrastructure
{
    public static class CultureHelper
    {
        public static decimal ToDecimal(this string input)
        {
            try
            {
                return decimal.Parse(input.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0m;
            }
        }
    }
}