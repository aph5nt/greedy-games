using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;

namespace Web.Controllers
{
    [Route("game/minefield")]
    public class GameController : Controller
    {
        [AllowAnonymous]
        [HttpGet("{network}")]
        public IActionResult Index(Network network)
        {
            return View();
        }
    }
}
