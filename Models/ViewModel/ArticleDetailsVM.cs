using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTinTuc.Models.ViewModel
{
    public class ArticleDetailsVM
    {
        public Article article { get; set; }

        // Các thuộc tính hỗ trợ phân trang (nếu cần cho bài viết liên quan)
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 3;

        // Danh sách bài viết cùng danh mục (nếu cần)
        public PagedList.IPagedList<Article> RelatedArticles { get; set; }

        // Danh sách bài viết nổi bật (nếu cần)
        public PagedList.IPagedList<Article> TopArticles { get; set; }
    }
}