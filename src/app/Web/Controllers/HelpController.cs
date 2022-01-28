using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("help")]
    public class HelpController : Controller
    {
        [HttpGet("support")]
        public IActionResult Support()
        {
            return View();
        }

        [HttpGet("faq")]
        public IActionResult Faq()
        {
            return View();
        }
    }
}