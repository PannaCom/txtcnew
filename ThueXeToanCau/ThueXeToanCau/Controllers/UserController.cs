using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index(int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            using (var db = new thuexetoancauEntities())
            {
                var users = db.users;
                var pageNumber = page ?? 1;
                var onePage = users.OrderBy(f => f.name).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        [HttpPost]
        public string addUpdateUser(user u)
        {
            return DBContext.addUpdateUser(u);
        }

        [HttpPost]
        public string deleteUser(int uId)
        {
            return DBContext.deleteUser(uId);
        }

        public string validateExistInfo(string name)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    user u = null;
                    if (!string.IsNullOrEmpty(name))
                    {
                        u = db.users.Where(f => f.name == name).FirstOrDefault();
                    }
                    if (u == null) return string.Empty;
                    return "Exist";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}