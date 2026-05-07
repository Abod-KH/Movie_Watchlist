using System;
using System.Collections.Generic;
using System.Linq;

namespace Movie_Watchlist.Application.Helpers
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }

    public static class PaginationExtensions
    {

        public static PagedResult<T> ToPagedResultServer<T>(this IEnumerable<T> items, int page, int totalItems, int pageSize = 20)
        {
            var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);

            return new PagedResult<T>
            {
                Items = items,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}
