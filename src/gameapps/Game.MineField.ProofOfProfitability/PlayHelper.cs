using System;
using GreedyGames.Game.Minefield.Commands;
using GreedyGames.Game.Minefield.Commands.Handlers;
using GreedyGames.Game.Minefield.Domain;
using GreedyGames.Shared.Model;
using GreedyGames.Types;

namespace Game.MineField.ProofOfProfitability
{
    public class PlayHelper
    {
        public Settings CreateSettings()
        {
            return new Settings
            {
                Network = Network.FREE,
                UserName = "aph5nt",
                Bet = 1,
                Dimension = new Dimension {X = 6, Y = 3},
                Seed = new Seed(Guid.NewGuid()),
                GameType = GameTypes.Minefield,
                Id = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19")
            };
        }

        public int MoveY(Settings settings)
        {
            var seed = Guid.NewGuid().GetHashCode();
            var rndMove = new Random(seed);
            return rndMove.Next(0, settings.Dimension.Y);
        }

        public void Run(Settings settings, int iteration, int turns)
        {
            if (iteration == turns)
            {
                var takeAwayHandler = new TakeAwayHandler();
                takeAwayHandler.Execute(new TakeAway
                {
                    Network = settings.Network,
                    GameId = settings.Id,
                    UserName = settings.UserName
                });
            }
            else if (iteration != turns && iteration < settings.Dimension.X)
            {
                var moveHandler = new MoveHandler();
                var result = moveHandler.Execute(new Move
                {
                    Network = settings.Network,
                    UserName = settings.UserName,
                    GameId = settings.Id,
                    Position = new Position {X = iteration, Y = MoveY(settings)}
                });

                if (result.Status == Status.Alive)
                    Run(settings, iteration + 1, turns);
            }
        }
    }
}