using Akka.Actor;
using Game.Minefield.Contracts.Commands;
using Game.Minefield.Contracts.Model;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Shared.Model;
using System;
using System.Threading.Tasks;
using WebApi.Configuration;
using WebApi.Providers;

namespace WebApi.Controllers
{
    [Route("game")]
    public class GameController : BaseApiController
    {
        private readonly IRemoteGameMinefieldProfider _minefieldProfider;
        private readonly WebSettings _settings;

        public GameController(IRemoteGameMinefieldProfider minefieldProfider, WebSettings settings)
        {
            _minefieldProfider = minefieldProfider;
            _settings = settings;
        }

        [HttpGet("{network}")]
        public async Task<IActionResult> Get(Network network)
        {
            var userState = await _minefieldProfider
                .Provide()
                .Ask<Response<UserState>>(new GetUserState(network, User.Identity.Name), TimeSpan.FromSeconds(_settings.AkkaSettings.ActorTimeout));

            return Json(userState.Data);
        }

        [HttpPost("{network}")]
        public async Task<IActionResult> Start(Network network, [FromBody] ClientSetting clientSetting)
        {

            var settings = new Game.Minefield.Contracts.Model.Settings
            {
                Id = Id.New(),
                UserName = User.Identity.Name,
                Network = network,
                Bet = clientSetting.Bet,
                GameType = GameTypes.Minefield,
                Seed = new Seed(clientSetting.Seed),
                Dimension = new Dimension
                {
                    X = clientSetting.X,
                    Y = clientSetting.Y
                }
            };

            var result = await _minefieldProfider.Provide()
                .Ask<Response<UserState>>(new Start(network, User.Identity.Name, settings), TimeSpan.FromSeconds(_settings.AkkaSettings.ActorTimeout));

            if (!result.IsValid)
            {
                return UnProcessableEntity(result.Errors);
            }

            return Json(result.Data);
        }

        [HttpPut("{network}/move/{gameId}")]
        public async Task<IActionResult> Move(Network network, string gameId, [FromBody] Position position)
        {
            var result = await _minefieldProfider.Provide().Ask<Response<UserState>>(
                new Move(network, User.Identity.Name, gameId, position),
                TimeSpan.FromSeconds(_settings.AkkaSettings.ActorTimeout));

            if (!result.IsValid)
            {
                return UnProcessableEntity(result.Errors);
            }

            return Json(result.Data);
        }

        [HttpPut("{network}/takeaway/{gameId}")]
        public async Task<IActionResult> TakeAway(Network network, string gameId)
        {
            var result = await _minefieldProfider.Provide().Ask<Response<UserState>>(
                new TakeAway(network, User.Identity.Name, gameId),
                TimeSpan.FromSeconds(_settings.AkkaSettings.ActorTimeout));

            if (!result.IsValid)
            {
                return UnProcessableEntity(result.Errors);
            }

            return Json(result.Data);

        }
    }
}