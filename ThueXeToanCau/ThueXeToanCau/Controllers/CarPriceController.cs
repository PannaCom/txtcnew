using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class CarPriceController : Controller
    {
        public ActionResult Index(int? page)
        {
            using (var db = new thuexetoancauEntities())
            {
                var carPrices = db.car_price;
                var pageNumber = page ?? 1;
                var onePage = carPrices.OrderBy(f => f.car_size).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        [HttpPost]
        public string addUpdateCarPrice(car_price cp)
        {
            return DBContext.addUpdateCarPrice(cp);
        }

        [HttpPost]
        public string deleteCarPrice(int cpId)
        {
            return DBContext.deleteCarPrice(cpId);
        }
    }
}