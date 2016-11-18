using PagedList;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class FactorController : Controller
    {
        public ActionResult Index(int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            using (var db = new thuexetoancauEntities())
            {
                var factors = db.factors;
                var pageNumber = page ?? 1;
                var onePage = factors.OrderBy(f => f.name).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        [HttpPost]
        public string addUpdateFactor(factor ft)
        {
            return DBContext.addUpdateFactor(ft);
        }

        [HttpPost]
        public string deleteFactor(int id)
        {
            return DBContext.deleteFactor(id);
        }
    }
}