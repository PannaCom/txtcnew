using PagedList;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class DriverController : Controller
    {
        public ActionResult Index(int? page)
        {
            using (var db = new thuexetoancauEntities())
            {
                var drivers = db.drivers;
                var pageNumber = page ?? 1;
                var onePage = drivers.OrderBy(f => f.name).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }
            ViewBag.cars = DBContext.getCars().Select(f => f.name).ToList();
            ViewBag.carTypes = DBContext.getCarTypes().Select(f => f.name).ToList();
            return View();
        }

        [HttpPost]
        public string addUpdateDriver(driver d)
        {
            return DBContext.addUpdateDriver(d);
        }

        [HttpPost]
        public string deleteDriver(int dId)
        {
            return DBContext.deleteDriver(dId);
        }
    }
}