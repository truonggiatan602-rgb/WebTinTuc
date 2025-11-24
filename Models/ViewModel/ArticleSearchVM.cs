using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;

namespace WebTinTuc.Models.ViewModel
{
    public class ArticleSearchVM
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;

        public string sortOrder { get; set; }
        
        
        public PagedList.IPagedList<WebTinTuc.Article> Articles { get; set; }
    }
}