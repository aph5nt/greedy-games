using Microsoft.AspNetCore.Mvc;
using Persistance.Model.Statistics;
using Shared.Model;
using System.Threading.Tasks;
using Game.Minefield.Storage;
using Web.Services;

namespace Web.Controllers
{
    [Route("statistics/minefield")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticService _statisticService;
        private readonly IGameLogService _gameLogService;

        public StatisticsController(IStatisticService statisticService, IGameLogService gameLogService)
        {
            _statisticService = statisticService;
            _gameLogService = gameLogService;
        }

        [HttpGet("{network}")]
        public async Task<IActionResult> Index(Network network, [FromQuery]string show = "my", [FromQuery] int page = 1)
        {
            var viewModel = new StatisticViewModel
            {
                Show = show,
                Network = network
            };

            switch (show)
            {
                case "my":
                    var myResult = await _statisticService.GetMinefieldStatAsync(network, User.Identity.Name, page, 33);
                    viewModel.Result = myResult;
                    break;
                default:
                    var allResult = await _statisticService.GetMinefieldStatAsync(network, page, 33);
                    viewModel.Result = allResult;
                    break;
            }

            return View(viewModel);
        }

        [HttpGet("{network}/details/{userName}/{gameId}")]
        public async Task<IActionResult> Details(Network network, string userName, string gameId)
        {
            var result = await _gameLogService.GetAsync(network, userName, gameId);
            return View("Details", result);
        }

    }

    public class StatisticViewModel
    {
        public PaginatedList<MinefieldStat> Result { get; set; }
        public string Show { get; set; }
        public Network Network { get; set; }
    }
}