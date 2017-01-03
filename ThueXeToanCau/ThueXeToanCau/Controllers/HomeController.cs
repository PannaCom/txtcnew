using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.name = Config.getCookie("name");
            ViewBag.phone = Config.getCookie("phone");
            //int price = 18000;
            //int factor = 100;
            //int km = 1624;
            //long total = (long)price * factor * km / 100;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult RegisterDriver() {
            ViewBag.cars = DBContext.getCars().Select(f => f.name).ToList();
            //ViewBag.carModels = DBContext.getCarModels().Select(f=>f.name).ToList();
            ViewBag.carTypes = DBContext.getListCarTypes().Select(f => f.name).ToList();
            return View();
        }

        [HttpPost]
        public string addUpdateDriver(driver dri)
        {
            return DBContext.addUpdateDriver(dri);
        }

        [HttpGet]
        public ActionResult ViewTrans()
        {
            return View();
        }
    }
}