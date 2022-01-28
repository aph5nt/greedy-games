using GreedyGames.Game.Minefield.Commands;
using GreedyGames.Game.Minefield.Commands.Handlers;
using GreedyGames.Game.Minefield.Domain;
using GreedyGames.Game.Minefield.Storage;

namespace Game.MineField.ProofOfProfitability
{
    public class Context
    {
        private readonly PlayHelper playHelper = new PlayHelper();
        public IGameStorage GameStorage { get; set; }

        public UserState Play(int turns)
        {
            var startHandler = new StartHandler();
            var settings = playHelper.CreateSettings();
            startHandler.Execute(new Start
            {
                Settings = settings
            });

            playHelper.Run(settings, 0, turns);

            return GameStorage.Get(settings.Network, settings.UserName, settings.Id).UserState;
        }
    }
}