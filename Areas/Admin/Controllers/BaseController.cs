using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebTinTuc.Areas.Admin.Controllers
{
    // Class này dùng để kiểm tra quyền Admin
    public class BaseController : Controller
    {
        // Hàm này sẽ chạy trước mọi hành động khác
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1. Kiểm tra xem đã đăng nhập chưa
            if (Session["User"] == null)
            {
                // Chưa đăng nhập -> Chuyển hướng về trang Login
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "Login", area = "" })
                );
                return;
            }

            // 2. Kiểm tra xem có phải là Admin không
            // Lưu ý: Kiểm tra cả "Admin" và "A" (phòng trường hợp bạn dùng viết tắt)
            string role = Session["UserRole"].ToString();

            if (role != "Admin" && role != "A")
            {
                // Đã đăng nhập nhưng không phải Admin -> Đuổi về trang chủ khách hàng
                // Có thể gán thêm thông báo lỗi nếu muốn
                TempData["Error"] = "Bạn không có quyền truy cập vào trang Quản trị!";

                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Home", action = "Index", area = "" })
                );
                return;
            }

            // Nếu thỏa mãn cả 2 điều kiện trên thì cho phép chạy tiếp
            base.OnActionExecuting(filterContext);
        }
    }
}