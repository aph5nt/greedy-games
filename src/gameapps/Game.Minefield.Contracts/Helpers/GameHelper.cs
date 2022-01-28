using Game.Minefield.Contracts.Model;
using Shared.Model;
using System;
using System.Collections.Generic;

namespace Game.Minefield.Contracts.Helpers
{
    public static class GameHelper
    {
        public static decimal[] GenerateMultiplicators(int xlen, int ylen)
        {
            var sum = xlen + ylen;

            switch (sum)
            {
                case 5:
                    // "3 x 2"
                    return new[] { 1.92m, 3.69m, 7.08m };
                case 9:
                    // "6 x 3"
                    return new[] { 1.44m, 2.07m, 2.99m, 4.3m, 6.19m, 8.92m };
                case 13:
                    // "9 x 4"
                    return new[] { 1.28m, 1.64m, 2.1m, 2.68m, 3.44m, 4.4m, 5.63m, 7.21m, 9.22m };
                default:
                    throw new NotImplementedException();
            }
        }

        public static Dictionary<Network, long> GetMaxBet =>
            new Dictionary<Network, long>
            {
                {Network.FREE, 10 * Money.Sathoshi},
                {Network.WAVES, 10 * Money.Sathoshi},
                {Network.GREEDYTEST, 10 * Money.Sathoshi}
            };

        public static Dictionary<Network, long> GetMinBet =>
            new Dictionary<Network, long>
            {
                {Network.FREE, (long) (0.01m * Money.Sathoshi)},
                {Network.WAVES, (long) (0.01m * Money.Sathoshi)},
                {Network.GREEDYTEST, (long) (0.01m * Money.Sathoshi)}
            };

        public static Dictionary<Network, long> GetDefaultBet =>
            new Dictionary<Network, long>
            {
                {Network.FREE, (long) (0.1 * Money.Sathoshi)},
                {Network.WAVES, (long) (0.1 * Money.Sathoshi)},
                {Network.GREEDYTEST, (long) (0.1 * Money.Sathoshi)}
            };

        public static FieldSize GetFieldSize(Dimension dimension)
        {
            var result = new FieldSize
            {
                Columns = dimension.X,
                Rows = dimension.Y,
                Multipiers = GenerateMultiplicators(dimension.X, dimension.Y),
                MaxBet = GetMaxBet,
                MinBet = GetMinBet,
                DefaultBet = GetDefaultBet
            };

            var sum = dimension.X + dimension.Y;

            switch (sum)
            {
                case 5:
                    result.Display = "3 x 2";
                    result.Size = "3x2";
                    break;
                case 9:
                    result.Display = "6 x 3";
                    result.Size = "6x3";
                    break;

                case 13:
                    result.Display = "9 x 4";
                    result.Size = "9x4";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        public static FieldSize DefaultFieldSize =>
            new FieldSize
            {
                Size = "6x3",
                Columns = 6,
                Rows = 3,
                Display = "6 x 3",
                Multipiers = new[] {1.44m, 2.07m, 2.99m, 4.3m, 6.19m, 8.92m}
            };

        public static List<FieldSize> FieldSizes()
        {
            return new List<FieldSize>
            {
                new FieldSize
                {
                    Size = "3x2",
                    Columns = 3,
                    Rows = 2,
                    Display = "3 x 2",
                    Multipiers = new[] { 1.92m, 3.69m, 7.08m}
                },
                DefaultFieldSize,
                new FieldSize
                {
                    Size = "9x4",
                    Columns = 9,
                    Rows = 4,
                    Display = "9 x 4",
                    Multipiers = new[] { 1.28m, 1.64m, 2.1m, 2.68m, 3.44m, 4.4m, 5.63m, 7.21m, 9.22m }
                }
            };
        }
    }
}
