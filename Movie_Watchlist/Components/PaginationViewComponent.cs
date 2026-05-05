using Microsoft.AspNetCore.Mvc;
using Movie_Watchlist.Application.ViewModels;
using System.Collections.Generic;

namespace Movie_Watchlist.Presintation.Components
{
    public class PaginationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int currentPage, int totalPages, string actionName, string? controllerName = null, Dictionary<string, string>? routeParams = null)
        {
            var model = new PaginationViewModel
            {
                CurrentPage = currentPage,
                TotalPages = totalPages,
                ActionName = actionName ?? "Index",
                ControllerName = controllerName,
                RouteParams = routeParams ?? new Dictionary<string, string>()
            };

            return View(model);
        }
    }
}
