using System;
using System.Collections.Generic;
using System.Linq;

namespace Movie_Watchlist.Application.Helpers
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }

    public static class PaginationExtensions
    {

        public static PagedResult<T> ToPagedResultServer<T>(this IEnumerable<T> items, int page, int totalItems, int pageSize = 20)
        {
            return new PagedResult<T>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}
