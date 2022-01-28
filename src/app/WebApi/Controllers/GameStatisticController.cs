using Microsoft.AspNetCore.Mvc;
using Shared.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("statistics")]
    public class GameStatisticController : BaseApiController
    {
        private readonly IGameStatisticService _gameStatisticService;


        public GameStatisticController(IGameStatisticService gameStatisticService)
        {
            _gameStatisticService = gameStatisticService;
        }

        [HttpGet("{network}")]
        public async Task<IActionResult> GetBy(Network network, bool showAll = true, int page = 1,  int pageSize = 33)
        {
            var result = showAll
                ? await _gameStatisticService.GetAsync(network, page, pageSize)
                : await _gameStatisticService.GetAsync(network, User.Identity.Name, page, pageSize);
            
            return Ok(new
            {
                Data = result.ToArray(),
                TotalPages = result.TotalPages,
                PageIndex = result.PageIndex,
                HasNextPage = result.HasNextPage,
                HasPreviousPage = result.HasPreviousPage
            });
        }
        
        [HttpGet("{network}/daily")]
        public async Task<IActionResult> GetDaily(Network network, bool showAll = true, int page = 1,  int pageSize = 33)
        {
            var result = showAll
                ? await _gameStatisticService.GetDailyAsync(network, page, pageSize)
                : await _gameStatisticService.GetDailyAsync(network, User.Identity.Name, page, pageSize);
            
            return Ok(new
            {
                Data = result.ToArray(),
                TotalPages = result.TotalPages,
                PageIndex = result.PageIndex,
                HasNextPage = result.HasNextPage,
                HasPreviousPage = result.HasPreviousPage
            });
        }

        [AllowAnonymous]
        [HttpGet("{network}/{userName}/{gameId}")]
        public async Task<IActionResult> Get(Network network, string userName, string gameId)
        {
            return Ok(await _gameStatisticService.GetAsync(network, userName, gameId));
        }
    }
}