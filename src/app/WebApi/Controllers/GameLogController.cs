using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("gamelogs")]
    public class GameLogController : BaseApiController
    {
        private readonly IGameLogService _gameLogService;

        public GameLogController(IGameLogService gameLogService)
        {
            _gameLogService = gameLogService;
        }

        [HttpGet("{network}/{userName}/{gameId}")]
        public async Task<IActionResult> Get(Network network, string userName, string gameId)
        {
            var result = await _gameLogService.GetAsync(network, userName, gameId);
            return Ok(result);
        }
    }
}