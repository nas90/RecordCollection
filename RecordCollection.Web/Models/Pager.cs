using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordCollection.Web.Models
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager(int totalItems, int? page, int pageSize = 10)
        {
            // calculate total, start and end pages
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            var startPage = currentPage - (int)Math.Ceiling((decimal)totalItems / 2);
            var endPage = currentPage + (int)Math.Ceiling((decimal)totalItems / 2) - 1;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > pageSize)
                {
                    startPage = endPage - pageSize - 1;
                }
            }
            
            if (currentPage > endPage)
                currentPage = endPage;

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
        
    }
}
