using PagedList;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class CarTypeController : Controller
    {
        public ActionResult Index(int? page)
        {
            using (var db = new thuexetoancauEntities())
            {
                var carTypes = db.car_type;
                var pageNumber = page ?? 1;
                var onePage = carTypes.OrderBy(f => f.name).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        [HttpPost]
        public string addUpdateCarType(car_type ct)
        {
            return DBContext.addUpdateCarType(ct);
        }

        [HttpPost]
        public string deleteCarType(int id)
        {
            return DBContext.deleteCarType(id);
        }
    }
}