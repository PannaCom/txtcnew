using PagedList;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class NoticeController : Controller
    {
        public ActionResult Index(int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            using (var db = new thuexetoancauEntities())
            {
                var notices = db.notices;
                var pageNumber = page ?? 1;
                var onePage = notices.OrderBy(f => f.name).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        [HttpPost]
        public string addUpdateNotice(notice n)
        {
            return DBContext.addUpdateNotice(n);
        }

        [HttpPost]
        public string deleteNotice(int nId)
        {
            return DBContext.deleteNotice(nId);
        }
    }
}