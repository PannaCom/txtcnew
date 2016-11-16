using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class BookingController : Controller
    {
        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            ViewBag.hireType = DBContext.getHireType();
            ViewBag.whoType = DBContext.getWhoType();
            return View();
        }

        [HttpGet]
        public string searchPhone(string keyword = "")
        {
            var rs = new List<string>();
            using (var db = new thuexetoancauEntities())
            {
                int phone;
                if (int.TryParse(keyword,out phone))
                {
                    rs = db.bookings.Where(f => f.phone.StartsWith(keyword)).OrderBy(f => f.name).Select(f => f.name + ", " + f.phone).Distinct().Take(50).ToList();
                }
                else
                {
                    rs = db.bookings.Where(f => f.name.StartsWith(keyword)).OrderBy(f => f.name).Select(f => f.name + ", " + f.phone).Distinct().Take(50).ToList();
                }
            }
            return JsonConvert.SerializeObject(rs);
        }

        [HttpGet]
        public string searchName(string keyword = "")
        {
            var rs = new List<string>();
            using (var db = new thuexetoancauEntities())
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    rs = db.bookings.OrderBy(f => f.name).Select(f => f.name).Distinct().Take(50).ToList();
                }
                else
                {
                    rs = db.bookings.Where(f => f.name.StartsWith(keyword)).OrderBy(f => f.name).Select(f => f.name).Distinct().Take(50).ToList();
                }
            }
            return JsonConvert.SerializeObject(rs);
        }

        [HttpGet]
        public JsonResult searchBooking(string keyword, string hireType, string whoType)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    
                    var arr = keyword.Split(',');
                    var name = arr[0].Trim();
                    IQueryable<booking> rs = db.bookings.Where(f => f.name == name);
                    //if (!string.IsNullOrEmpty(phone))
                    //{
                    //    rs = db.bookings.Where(f => f.phone == phone);
                    //} else
                    //{
                    //    rs = db.bookings.Where(f => f.name == name);
                    //}
                    if (hireType != "All")
                    {
                        rs = rs.Where(f => f.car_hire_type == hireType);
                    }
                    if (whoType != "All")
                    {
                        rs = rs.Where(f => f.car_who_hire == whoType);
                    }
                    var bookings = rs.OrderBy(f => f.name).Select(f=> new {f.phone, f.name,f.car_from,f.car_to,f.car_type,f.datebook,f.book_price }).ToList();
                    return Json(bookings, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { ErrMess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}