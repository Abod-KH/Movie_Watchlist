using System.Collections.Generic;

namespace Movie_Watchlist.Application.ViewModels
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string ActionName { get; set; } = "Index";
        public string? ControllerName { get; set; }
        public Dictionary<string, string> RouteParams { get; set; } = new Dictionary<string, string>();
    }
}
