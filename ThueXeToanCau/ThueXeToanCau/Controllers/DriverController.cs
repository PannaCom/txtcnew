using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using ThueXeToanCau.Models;
using Newtonsoft.Json;
namespace ThueXeToanCau.Controllers
{
    public class DriverController : Controller
    {
        private thuexetoancauEntities db = new thuexetoancauEntities();
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
        public ActionResult ListWin(int? page)
        {
            //if (Config.getCookie("id_driver") == "") return RedirectToAction("Log", "Driver");
            string driver_phone = Config.getCookie("driver_phone");
            //using (var db = new thuexetoancauEntities())
            //{
            //    var booking_final = db.booking_final;
            //    var pageNumber = page ?? 1;
            //    var onePage = booking_final.Where(o => o.id_driver == id_driver).OrderByDescending(f => f.id).ToPagedList(pageNumber, 20);
            //    ViewBag.onePage = onePage;
            //}
            ViewBag.driver_phone = driver_phone;
            return View();
        }
        public class drvol
        {
            public long id { get; set; }
            public string name { get; set; }
            public string phone { get; set; }
            public string email { get; set; }
            public string car_model { get; set; }
            public string car_made { get; set; }
            public int car_years { get; set; }
            public int car_size { get; set; }
            public string car_number { get; set; }
            public string car_type { get; set; }
            public string address { get; set; }
            public double? lon { get; set; }
            public double? lat { get; set; }
            public double D { get; set; }
        }
        public string DriverOnline(double? lon,double? lat,int? car_size)
        {
            try { 
                string query = "select top 10 * from ";
                       query+="(";
                       query += "SELECT id,name,phone,email,car_model,car_made,car_years,car_size,car_number,car_type,address,lon,lat,ACOS(SIN(PI()*" + lat + "/180.0)*SIN(PI()*lat/180.0)+COS(PI()*" + lat + "/180.0)*COS(PI()*lat/180.0)*COS(PI()*lon/180.0-PI()*" + lon + "/180.0))*6371 as D ";
                       query+="FROM thuexetoancau.dbo.drivers as A inner join ";
                       query+="(select lon,lat,phone as phone2 from list_online) as B on A.phone=B.phone2 ";
                       query+=" ) as C where 1=1 ";
                if (lon!=null){
                    query+=" and D<300";
                }
                if (car_size != null)
                {
                    query += " and car_size=" + car_size;
                }
                query += " order by D";
                return JsonConvert.SerializeObject(db.Database.SqlQuery<drvol>(query).ToList());
            }
            catch
            {
                return "0";
            }
        }
        public ActionResult Reg()
        {
            
            ViewBag.cars = DBContext.getCars().Select(f => f.name).ToList();
            ViewBag.carTypes = DBContext.getListCarTypes().Select(f => f.name).ToList();
            return View();
        }
        public ActionResult Log()
        {
            return View();
        }
        public string login(string phone, string pass)
        {
            try
            {
                MD5 md5Hash = MD5.Create();
                string hash = Config.GetMd5Hash(md5Hash, pass);
                pass = hash;
                bool p = db.drivers.Any(x=>x.phone==phone && x.pass==pass);
                if (p) {
                    var p2 = db.drivers.Where(x => x.phone == phone && x.pass == pass).FirstOrDefault();
                    Config.setCookie("id_driver", p2.id.ToString());
                    Config.setCookie("driver_number", p2.car_number!=null?p2.car_number.ToString():"");
                    Config.setCookie("driver_phone", p2.phone!=null?p2.phone.ToString():"");
                    
                    return "1"; 
                } else { 
                    return "0"; 
                }
            }
            catch (Exception ex)
            {
                return "-1";
            }
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
        public string addUpdateDriver2(list_online lo)
        {
            return DBContext.addUpdateDriver2(lo);
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