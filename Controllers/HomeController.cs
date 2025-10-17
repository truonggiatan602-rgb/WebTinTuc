using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTinTuc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult WebTinTuc()
        {
            return View();
        }
        public ActionResult TinTuc()
        {
            return View();
        }
        public ActionResult ThoiTrang()
        {
            return View();
        }
        public ActionResult TienIch()
        {
            return View();
        }
        public ActionResult LoiSong()
        {
            return View();
        }
        public ActionResult DuLich()
        {
            return View();
        }
    }
}