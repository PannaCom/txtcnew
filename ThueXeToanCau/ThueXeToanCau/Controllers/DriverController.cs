using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class DriverController : Controller
    {
        public ActionResult Index(string phone,int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (phone == null) phone = "";
            using (var db = new thuexetoancauEntities())
            {
                var drivers = db.drivers;
                var pageNumber = page ?? 1;
                var onePage = drivers.Where(o => o.phone.Contains(phone) || o.car_number.Contains(phone)).OrderByDescending(f => f.id).ToPagedList(pageNumber, 20);
                ViewBag.onePage = onePage;
            }
            ViewBag.k = phone;
            ViewBag.cars = DBContext.getCars().Select(f => f.name).ToList();
            ViewBag.carTypes = DBContext.getListCarTypes().Select(f => f.name).ToList();
            return View();
        }
        public string searchCar(string keyword)
        {
            var numbers = new List<string>();
            using (var db = new thuexetoancauEntities())
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    numbers = db.drivers.OrderBy(f => f.car_number)
                        .Select(f => f.car_number).Distinct().Take(50).ToList();
                }
                else
                {
                    numbers = db.drivers.Where(f => f.car_number.Replace("-", "").Replace(".", "")
                        .StartsWith(keyword.Replace("-", "").Replace(".", "")))
                        .OrderBy(f => f.car_number).Select(f => f.car_number)
                        .Distinct().Take(50).ToList();
                }
            }
            return JsonConvert.SerializeObject(numbers);
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

        public string validateExistInfo(string card_identify, string car_number)
        {
            try {
                using (var db = new thuexetoancauEntities())
                {
                    driver dri = null;
                    if (!string.IsNullOrEmpty(card_identify))
                    {
                        dri = db.drivers.Where(f => f.card_identify == card_identify).FirstOrDefault();
                    }
                    else if (!string.IsNullOrEmpty(car_number))
                    {
                        dri = db.drivers.Where(f => f.car_number == car_number).FirstOrDefault();
                    }
                    if (dri == null) return string.Empty;
                    return "Exist";
                }
            } catch (Exception ex)
            {
                return ex.Message;
            }            
        }
    }
}