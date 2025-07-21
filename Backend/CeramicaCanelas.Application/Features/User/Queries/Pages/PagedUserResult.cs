using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeramicaCanelas.Application.Features.User.Queries.Pages
{
    public class PagedUserResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public PagedUserResult() { }

        public PagedUserResult(IEnumerable<T> items, int totalItems, int page, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            Page = page;
            PageSize = pageSize;
        }
    }
}
