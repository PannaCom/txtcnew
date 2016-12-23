using PagedList;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class NationalDayController : Controller
    {
        public ActionResult Index(int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            using (var db = new thuexetoancauEntities())
            {
                var nalDays = db.NationalDays;
                var pageNumber = page ?? 1;
                var onePage = nalDays.OrderBy(f => f.DayName).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        [HttpPost]
        public string addUpdateNationalDay(NationalDay nalDay)
        {
            return DBContext.addUpdateNationalDay(nalDay);
        }

        [HttpPost]
        public string deleteNationalDay(int id)
        {
            return DBContext.deleteNationalDay(id);
        }
    }
}