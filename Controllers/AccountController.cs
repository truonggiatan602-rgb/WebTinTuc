using System;
using System.Collections.Generic;
using System.Data.Entity.Validation; // Thư viện quan trọng để bắt lỗi Validation
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTinTuc.Models; // Đảm bảo namespace này đúng với project của bạn

namespace WebTinTuc.Controllers
{
    public class AccountController : Controller
    {
        // Khởi tạo kết nối Database
        // Lưu ý: Nếu bạn dùng MyNews1Entities thì sửa lại tên ở đây
        private MyNewsEntities db = new MyNewsEntities();

        // ==========================================
        // PHẦN ĐĂNG KÝ (REGISTER)
        // ==========================================

        // GET: Hiển thị form đăng ký
        public ActionResult Register()
        {
            return View();
        }

        // POST: Xử lý dữ liệu đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string username, string password, string confirmPassword, string fullName, string email)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra mật khẩu nhập lại
                if (password != confirmPassword)
                {
                    ViewBag.Error = "Mật khẩu nhập lại không khớp!";
                    return View();
                }

                // 2. Kiểm tra xem Username đã tồn tại chưa
                var checkUser = db.Users.FirstOrDefault(s => s.Username == username);
                if (checkUser == null)
                {
                    // --- BƯỚC A: Tạo User ---
                    var newUser = new User
                    {
                        Username = username,
                        Password = password, // Lưu ý: Thực tế nên mã hóa MD5/SHA
                        UserRole = "User"    // Mặc định là người dùng thường
                    };
                    db.Users.Add(newUser);

                    // --- BƯỚC B: Tạo Reader ---
                    var newReader = new Reader
                    {
                        ReaderName = fullName,
                        ReaderEmail = email,
                        Username = username, // Khóa ngoại liên kết với bảng User
                        IsAuthor = false,    // Mặc định chưa là tác giả

                        // ĐÃ XÓA DÒNG ReaderPhone Ở ĐÂY VÌ BẠN KHÔNG DÙNG NỮA

                        // Nếu cột ViewCount trong DB không cho phép Null, hãy bỏ comment dòng dưới:
                        // ViewCount = 0 
                    };
                    db.Readers.Add(newReader);

                    // --- BƯỚC C: Lưu vào Database (Có bắt lỗi chi tiết) ---
                    try
                    {
                        db.SaveChanges();
                        // Lưu thành công -> Chuyển qua trang đăng nhập
                        return RedirectToAction("Login");
                    }
                    // --- BẮT LỖI VALIDATION ---
                    catch (DbEntityValidationException ex)
                    {
                        string errorMessages = "";
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                errorMessages += $"- Lỗi tại <b>{validationError.PropertyName}</b>: {validationError.ErrorMessage} <br/>";
                            }
                        }
                        ViewBag.Error = errorMessages;
                    }
                    // --- BẮT LỖI KHÁC ---
                    catch (Exception ex)
                    {
                        ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
                        if (ex.InnerException != null)
                        {
                            ViewBag.Error += "<br/>Chi tiết: " + ex.InnerException.Message;
                        }
                    }
                }
                else
                {
                    ViewBag.Error = "Tên đăng nhập này đã có người dùng!";
                }
            }
            return View();
        }

        // ==========================================
        // PHẦN ĐĂNG NHẬP (LOGIN)
        // ==========================================

        // GET: Hiển thị form đăng nhập
        public ActionResult Login()
        {
            return View();
        }

        // POST: Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    // Lưu session
                    Session["User"] = user;
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    // Lấy thêm tên thật từ bảng Reader
                    var reader = db.Readers.FirstOrDefault(r => r.Username == username);
                    if (reader != null)
                    {
                        Session["ReaderID"] = reader.ReaderID;
                        Session["FullName"] = reader.ReaderName;
                    }

                    // Phân quyền chuyển hướng
                    if (user.UserRole == "Admin")
                    {
                        return RedirectToAction("Index", "Article", new { area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                }
            }
            return View();
        }

        // ==========================================
        // PHẦN ĐĂNG XUẤT (LOGOUT)
        // ==========================================
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}