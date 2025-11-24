// File: /Areas/Admin/Controllers/UserController.cs
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.Data.Entity;
using Microsoft.Ajax.Utilities;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.UI.WebControls;
using WebTinTuc;

namespace WebTinTuc.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private MyNewsEntities db = new MyNewsEntities();

        // 1. Danh sách User
        public ActionResult Index()
        {
            // Lấy danh sách Reader kèm thông tin User (Join bảng)
            // Lưu ý: Bạn cần include bảng User nếu có quan hệ trong Model
            // Hoặc đơn giản lấy list Users
            var users = db.Users.ToList();
            return View(users);
        }

        // 2. Chức năng nâng quyền (Promote)
        public ActionResult EditRole(string username)
        {
            if (username == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(username);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(User user)
        {
            if (ModelState.IsValid)
            {
                // Tìm user trong db
                var userInDb = db.Users.Find(user.Username);

                // Chỉ cập nhật Role
                userInDb.UserRole = user.UserRole; // Ví dụ sửa thành "Admin"

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }
    }
}