using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class AirportWayController : Controller
    {
        public ActionResult Index(int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            using (var db = new thuexetoancauEntities())
            {
                var aw = db.airport_way;
                var pageNumber = page ?? 1;
                var onePage = aw.OrderBy(f => f.airport_name).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        public JsonResult getAirportWay(long aId)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    return Json(db.airport_way.Find(aId), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { errMess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public string addUpdateAirportWay(airport_way aw)
        {
            return DBContext.addUpdateAirportWay(aw);
        }

        [HttpPost]
        public string deleteAirportWay(int id)
        {
            return DBContext.deleteAirportWay(id);
        }
    }
}