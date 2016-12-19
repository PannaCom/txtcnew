using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThueXeToanCau.Models;
using PagedList;
namespace ThueXeToanCau.Controllers
{
    public class SaoKeController : Controller
    {
        private thuexetoancauEntities db = new thuexetoancauEntities();
        public class sk
        {
            public long? id { get; set; }
            public long? id_driver { get; set; }
            public string driver_name { get; set; }
            public string driver_phone { get; set; }
            public int? money { get; set; }
            public DateTime? date_time { get; set; }
            public int? status { get; set; }
            public string phone { get; set; }
            public string car_from { get; set; }
            public string car_to { get; set; }
            public string cumstomer_phone { get; set; }
            public DateTime? datebook { get; set; }
            public string car_hire_type { get; set; }
        }
        // GET: SaoKe
        public ActionResult Index(string phone,int? page)
        {
            string query = "  select id,id_driver,driver_name,driver_phone,money,date_time,status,car_from,car_to,cumstomer_phone,datebook,car_hire_type from ";
                    query += "(select id,id_driver,id_booking,money,date_time,status from saoke) as A inner join ";
                    query += "(select name as driver_name,phone as driver_phone,id as id_driver2 from drivers) as B on A.id_driver=B.id_driver2 inner join ";
                    query+=" (select car_from,car_to,phone as cumstomer_phone,datebook,id as id_booking2,car_hire_type from booking) as C on A.id_booking=C.id_booking2 ";
                    query+=" where 1=1 ";
                    if (phone != null) query += " and driver_phone=N'" + phone + "' ";
                    query+=" order by date_time desc ";
                    var p = db.Database.SqlQuery<sk>(query);
                    int pageSize = 25;
                    int pageNumber = 1;
                    ViewBag.k = phone;
                    ViewBag.page = page;
                    return View(p.ToPagedList(pageNumber, pageSize)); 
        }

        // GET: SaoKe/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            saoke saoke = db.saokes.Find(id);
            if (saoke == null)
            {
                return HttpNotFound();
            }
            return View(saoke);
        }

        // GET: SaoKe/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SaoKe/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_driver,id_booking,money,date_time")] saoke saoke)
        {
            if (ModelState.IsValid)
            {
                db.saokes.Add(saoke);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(saoke);
        }

        // GET: SaoKe/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            saoke saoke = db.saokes.Find(id);
            if (saoke == null)
            {
                return HttpNotFound();
            }
            return View(saoke);
        }

        // POST: SaoKe/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_driver,id_booking,money,date_time")] saoke saoke)
        {
            if (ModelState.IsValid)
            {
                db.Entry(saoke).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(saoke);
        }

        // GET: SaoKe/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            saoke saoke = db.saokes.Find(id);
            if (saoke == null)
            {
                return HttpNotFound();
            }
            return View(saoke);
        }

        // POST: SaoKe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            saoke saoke = db.saokes.Find(id);
            db.saokes.Remove(saoke);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
