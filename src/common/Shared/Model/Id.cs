using System;

namespace Shared.Model
{
    public class Id
    {
        public static string New()
        {
            return $"{DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks:D19}";
        }
    }
}