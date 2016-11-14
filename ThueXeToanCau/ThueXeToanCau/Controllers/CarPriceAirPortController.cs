using PagedList;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class CarPriceAirportController : Controller
    {
        public ActionResult Index(int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            using (var db = new thuexetoancauEntities())
            {
                var CarPriceAirports = db.car_price_airport;
                var pageNumber = page ?? 1;
                var onePage = CarPriceAirports.OrderBy(f => f.car_size).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        [HttpPost]
        public string addUpdateCarPriceAirport(car_price_airport cp)
        {
            return DBContext.addUpdateCarPriceAirport(cp);
        }

        [HttpPost]
        public string deleteCarPriceAirport(int cpId)
        {
            return DBContext.deleteCarPriceAirport(cpId);
        }
    }
}