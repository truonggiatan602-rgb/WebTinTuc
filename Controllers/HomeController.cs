using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTinTuc.Models; // Namespace chứa Article và DbContext

namespace WebTinTuc.Controllers
{
    public class HomeController : Controller
    {
        private MyNewsEntities db = new MyNewsEntities();

        // GET: Trang chủ
        public ActionResult Index()
        {
            // Lấy 4 bài viết mới nhất (Sắp xếp theo ngày đăng giảm dần)
            var articles = db.Articles.OrderByDescending(a => a.PublishDate).Take(4).ToList();

            return View(articles);
        }

        // GET: Xem chi tiết bài viết (Khi bấm vào hình/tiêu đề)
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var article = db.Articles.Find(id);
            if (article == null) return HttpNotFound();

            return View(article);
        }

        public ActionResult Category(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // 1. Lấy bài viết thuộc danh mục này
            var articles = db.Articles
                             .Where(s => s.TopicID == id)
                             .OrderByDescending(s => s.PublishDate)
                             .ToList();

            // 2. Lấy tên danh mục để hiện lên tiêu đề
            var category = db.Topics.Find(id);
            if (category != null)
            {
                ViewBag.Title = category.TopicName; // Ví dụ: "Thời trang"
            }
            else
            {
                ViewBag.Title = "Danh mục";
            }

            return View(articles);
        }
    }
    // ... Các Action khác (About, Contact...) giữ nguyên
}