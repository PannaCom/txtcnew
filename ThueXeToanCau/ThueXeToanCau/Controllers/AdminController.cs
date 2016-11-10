using System;
using System.Web;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            var user = Session["user"];
            if (user != null)
            {
                return RedirectToAction("Index");
            }
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
            //// create a cookie
            //HttpCookie newCookie = new HttpCookie("userName", u.name);
            //newCookie.Expires = DateTime.Today.AddMinutes(30);
            //Request.Cookies.Add(newCookie);
            //Response.Cookies.Add(newCookie);
            return RedirectToAction("Index");
        }

        // GET: Admin
        public ActionResult Index()
        {
            var user = Session["user"];
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
    }
}