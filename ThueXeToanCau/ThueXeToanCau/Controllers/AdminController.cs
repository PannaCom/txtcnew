using System;
using System.Web.Mvc;
using ThueXeToanCau.Models;
using Excel;
using System.IO;
using System.Linq;
using System.Globalization;

namespace ThueXeToanCau.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (Config.getCookie("logged") != "") return RedirectToAction("Index");            
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            user u = DBContext.validateLogin(model);
            if(u==null)
            {
                ModelState.AddModelError("error", "Bạn đã nhập sai Tên hoặc Mật Khẩu, vui lòng thử lại!");
                return View(model);
            }
            Session["user"] = u;           
            return RedirectToAction("Index");
        }

        [HttpPost]
        public string validateLogin(LoginModel model)
        {
            try
            {
                user u = DBContext.validateLogin(model);
                if (u == null)
                {
                    return "Bạn đã nhập sai Tên hoặc Mật Khẩu, vui lòng thử lại!";
                }
                Config.setCookie("logged", u.name);
                return string.Empty;
            } catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
        }

        // GET: Admin
        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        public ActionResult ImportExcel()
        {
            return View();
        }             
    }
}