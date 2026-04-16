using Microsoft.AspNetCore.Mvc;

namespace Movie_Watchlist.Presintation.Controllers
{
    public class ErrorController : Controller
    {
       
        [Route("NotFound")]
        public IActionResult NotFound()
        {
           
            return View("NotFound");
        }
    }
}
