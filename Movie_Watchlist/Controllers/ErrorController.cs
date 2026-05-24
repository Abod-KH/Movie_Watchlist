using Microsoft.AspNetCore.Mvc;

namespace Movie_Watchlist.Presintation.Controllers
{
    public class ErrorController : Controller
    {

        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("NotFound");

                case 403:
                    return View("Forbidden");

                case 401:
                    return View("Unauthorized");

                case 500:
                    return View("ServerError");

                default:
                    return View("GenericError");
            }
        }
    }
}
