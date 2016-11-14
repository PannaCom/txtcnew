using PagedList;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class WhoTypeController : Controller
    {
        public ActionResult Index(int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            using (var db = new thuexetoancauEntities())
            {
                var whoTypes = db.car_who_hire;
                var pageNumber = page ?? 1;
                var onePage = whoTypes.OrderBy(f => f.name).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        [HttpPost]
        public string addUpdateWhoType(car_who_hire wh)
        {
            return DBContext.addUpdateWhoType(wh);
        }

        [HttpPost]
        public string deleteWhoType(int id)
        {
            return DBContext.deleteWhoType(id);
        }
    }
}