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
        public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, int page)
        {
            int pageSize = 20;
            var total = source.Count();
            var totalPages = total == 0 ? 1 : (int)Math.Ceiling((double)total / pageSize);
            
            var items = source
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>
            {
                Items = items,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }
    }
}
