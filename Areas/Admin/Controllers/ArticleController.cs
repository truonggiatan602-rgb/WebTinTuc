using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebTinTuc.Models;            // Namespace chứa các Model (Article, Topic...)
using WebTinTuc.Models.ViewModel;  // Namespace chứa ViewModel (ArticleSearchVM...)
using PagedList;
using PagedList.Mvc;

namespace WebTinTuc.Areas.Admin.Controllers
{
    // Kế thừa BaseController để bảo mật (Admin mới được vào)
    public class ArticleController : BaseController
    {
        private MyNewsEntities db = new MyNewsEntities();

        // ==========================================
        // 1. DANH SÁCH BÀI VIẾT (INDEX)
        // ==========================================
        public ActionResult Index(string searchTerm, string sortOrder, int? page)
        {
            var model = new ArticleSearchVM();
            var articles = db.Articles.AsQueryable(); // Tạo truy vấn

            // 1.1. Tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                articles = articles.Where(p =>
                    p.Title.Contains(searchTerm) ||
                    p.Content.Contains(searchTerm) ||
                    p.Topic.TopicName.Contains(searchTerm));
            }

            // 1.2. Sắp xếp
            switch (sortOrder)
            {
                case "name_asc":
                    articles = articles.OrderBy(p => p.Title);
                    break;
                case "name_desc":
                    articles = articles.OrderByDescending(p => p.Title);
                    break;
                default:
                    // Mặc định sắp xếp bài mới nhất lên đầu
                    articles = articles.OrderByDescending(p => p.PublishDate);
                    break;
            }
            model.sortOrder = sortOrder;
            model.SearchTerm = searchTerm; // Lưu lại từ khóa để hiện trên View

            // 1.3. Phân trang
            int pageNumber = page ?? 1;
            int pageSize = 5; // Số bài viết trên 1 trang (bạn có thể sửa số này)

            model.Articles = articles.ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        // ==========================================
        // 2. CHI TIẾT BÀI VIẾT (DETAILS)
        // ==========================================
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Article article = db.Articles.Find(id);
            if (article == null) return HttpNotFound();

            return View(article);
        }

        // ==========================================
        // 3. TẠO BÀI VIẾT MỚI (CREATE)
        // ==========================================
        public ActionResult Create()
        {
            // Load danh sách chủ đề vào Dropdown
            ViewBag.TopicID = new SelectList(db.Topics, "TopicID", "TopicName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArticleID,TopicID,Title,Content,ImageURL")] Article article)
        {
            if (ModelState.IsValid)
            {
                // A. Tự động lấy ngày giờ hiện tại
                article.PublishDate = DateTime.Now;

                // B. Gán lượt xem mặc định = 0
                article.ViewCount = 0;

                // C. Lấy ID tác giả từ người đang đăng nhập (Session)
                if (Session["ReaderID"] != null)
                {
                    article.AuthorID = int.Parse(Session["ReaderID"].ToString());
                }
                else
                {
                    // Nếu Session hết hạn -> Đuổi về trang đăng nhập
                    return RedirectToAction("Login", "Account", new { area = "" });
                }

                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TopicID = new SelectList(db.Topics, "TopicID", "TopicName", article.TopicID);
            return View(article);
        }

        // ==========================================
        // 4. SỬA BÀI VIẾT (EDIT)
        // ==========================================
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Article article = db.Articles.Find(id);
            if (article == null) return HttpNotFound();

            ViewBag.TopicID = new SelectList(db.Topics, "TopicID", "TopicName", article.TopicID);
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArticleID,TopicID,Title,Content,ImageURL")] Article article)
        {
            if (ModelState.IsValid)
            {
                // Tìm bài cũ trong DB để update từng trường
                var articleInDb = db.Articles.Find(article.ArticleID);

                if (articleInDb != null)
                {
                    // Cập nhật các thông tin mới
                    articleInDb.Title = article.Title;
                    articleInDb.Content = article.Content;
                    articleInDb.ImageURL = article.ImageURL;
                    articleInDb.TopicID = article.TopicID;

                    // Cập nhật lại ngày sửa (Tùy chọn, nếu muốn giữ ngày cũ thì xóa dòng này)
                    articleInDb.PublishDate = DateTime.Now;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.TopicID = new SelectList(db.Topics, "TopicID", "TopicName", article.TopicID);
            return View(article);
        }

        // ==========================================
        // 5. XÓA BÀI VIẾT (DELETE)
        // ==========================================
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Article article = db.Articles.Find(id);
            if (article == null) return HttpNotFound();

            return View(article);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
            if (article == null) return HttpNotFound();

            try
            {
                // --- BƯỚC 1: Xóa các Comment liên quan trước ---
                var comments = db.Comments.Where(c => c.ArticleID == id).ToList();
                if (comments.Any())
                {
                    db.Comments.RemoveRange(comments);
                }

                // --- BƯỚC 2: Xóa bài viết ---
                db.Articles.Remove(article);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Bắt lỗi nếu có vấn đề (ví dụ khóa ngoại khác)
                ViewBag.Error = "Không thể xóa bài viết này. Lỗi hệ thống: " + ex.Message;
                if (ex.InnerException != null)
                {
                    ViewBag.Error += "<br>Chi tiết: " + ex.InnerException.Message;
                }
                return View(article);
            }
        }

        // Giải phóng tài nguyên
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}