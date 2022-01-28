using Akka.Actor;
using Game.Minefield.Contracts.Commands;
using Game.Minefield.Contracts.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Shared.Model;
using System;
using System.Threading.Tasks;
using Web.Providers;

namespace Web.Controllers
{
    [Route("api/game/minefield")]
    [Authorize]
    public class GameApiController : Controller
    {
        private readonly IGameMinefieldProfider _minefieldProfider;

        public GameApiController(IGameMinefieldProfider minefieldProfider)
        {
            _minefieldProfider = minefieldProfider;
        }

        [HttpGet("{network}")]
        public async Task<IActionResult> Get(Network network)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userState = await _minefieldProfider.Provide().Ask<UserState>(new GetUserState(network, User.Identity.Name), TimeSpan.FromSeconds(30));
                return Json(userState);
            }

            return Unauthorized();
        }
        
        [HttpPost("{network}")]
        public async Task<IActionResult> Start(Network network, [FromBody] ClientSetting clientSetting)
        {
            if (User.Identity.IsAuthenticated)
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

                var result = await _minefieldProfider.Provide().Ask<Response<UserState>>(new Start(network, User.Identity.Name, settings), TimeSpan.FromSeconds(30));

                if (!result.IsValid)
                {
                    return BadRequest(result.Errors);
                }

                return Json(result.Data);
            }

            return Unauthorized();
        }

        [HttpPut("{network}/move/{gameId}")]
        public async Task<IActionResult> Move(Network network, string gameId, [FromBody] Position position)
        {
            if (User.Identity.IsAuthenticated)
            {
                var result = await _minefieldProfider.Provide().Ask<Response<UserState>>(new Move(network, User.Identity.Name, gameId, position), TimeSpan.FromSeconds(30));
                if (!result.IsValid)
                {
                    return BadRequest(result.Errors);
                }

                return Json(result.Data);
            }

            return Unauthorized();
 
        }

        [HttpPut("{network}/takeaway/{gameId}")]
        public async Task<IActionResult> TakeAway(Network network, string gameId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var result = await _minefieldProfider.Provide().Ask<Response<UserState>>(new TakeAway(network, User.Identity.Name, gameId), TimeSpan.FromSeconds(30));
                if (!result.IsValid)
                {
                    return BadRequest(result.Errors);
                }

                return Json(result.Data);
            }

            return Unauthorized();
        }
    }
}