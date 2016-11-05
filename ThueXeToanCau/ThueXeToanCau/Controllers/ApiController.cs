using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThueXeToanCau.Models;
using Newtonsoft.Json;
using System.Data.Entity;
using Microsoft.Owin.BuilderProperties;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
namespace ThueXeToanCau.Controllers
{
    public class ApiController : Controller
    {
        private thuexetoancauEntities db = new thuexetoancauEntities();
        // GET: Api
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public string booking(string car_from, string car_to, int? car_type, string car_hire_type, string car_who_hire, DateTime? from_datetime, DateTime? to_datetime, double lon1,double lat1,double lon2,double lat2)
        {
            try
            {
                //get km distance
                int price_max = getD(lon1, lat1, lon2, lat2, car_hire_type, car_type, from_datetime);
                int reduce = Config.reduct1;
                int price = price_max - price_max * reduce / 100;

                booking bo = new booking();
                bo.car_from = car_from;
                bo.car_hire_type = car_hire_type;
                bo.car_to = car_to;
                bo.car_type = car_type;
                bo.car_who_hire = car_who_hire;
                bo.from_datetime = from_datetime;
                bo.geo1 = Config.CreatePoint(lat1, lon1);
                bo.geo2 = Config.CreatePoint(lat2, lon2);
                bo.lat1 = lat1;
                bo.lat2 = lat2;
                bo.lon1 = lon1;
                bo.lon2 = lon2;
                bo.to_datetime = to_datetime;
                bo.datebook = DateTime.Now;
                bo.book_price = price;
                bo.book_price_max = price_max;
                bo.time_to_reduce = 15 * 60;
                db.bookings.Add(bo);
                db.SaveChanges();
                
                return "1";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        //Class for "distance" and "duration" which has the "text" and "value" properties.
        public class CepElementNode
        {
            public string text { get; set; }

            public string value { get; set; }
        }

        //Class for "distance", "duration" and "status" nodes of "elements" node 
        public class CepDataElement
        {
            public CepElementNode distance { get; set; }

            public CepElementNode duration { get; set; }

            public string status { get; set; }
        }

        //Class for "elements" node
        public class CepDataRow
        {
            public List<CepDataElement> elements { get; set; }
        }

        //Class which wrap the json response
        public class RequestCepViewModel
        {
            public List<string> destination_addresses { get; set; }

            public List<string> origin_addresses { get; set; }

            public List<CepDataRow> rows { get; set; }

            public string status { get; set; }
        }
        //Hà Nội - Ninh Bình http://localhost:58046/api/getD?lat1=20.9740873&lon1=105.3724793&lat2=20.1877591&lon2=105.5745668
        public int getD(double lon1, double lat1, double lon2, double lat2, string car_hire_type,int? car_type,DateTime? date)
        {
            //get km distance
            try { 
                string address = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + lat1 + "," + lon1 + "&destinations=" + lat2 + "," + lon2 + "&key=AIzaSyDLPSKQ4QV4xGiQjnZDUecx-UEr3D0QePY";
                string result = new System.Net.WebClient().DownloadString(address);
                var viewModel = new JavaScriptSerializer().Deserialize<RequestCepViewModel>(result);
                var dt = viewModel.rows[0].elements[0].distance.value;
                //return rs.rows.elements.distance.value;
                //JavaScriptSerializer jss = new JavaScriptSerializer();
                //var rs = jss.Deserialize<dynamic>(result);
                //var a=rs["rows"][0]["elements"][0]["distance"];//[0][1].ToString()
                //return a[1];
                int km= int.Parse(dt)/1000;
                int factor = 100;
                int price=6000;
                int total = price*factor*km;
                if (car_hire_type.ToLowerInvariant().Contains("một chiều"))
                {
                    if (Config.isHoliDay(date))
                    {
                        if (car_type == 5) { 
                            factor = Config.factorHoliday1;
                            price = Config.price1;
                        }
                        if (car_type == 8) { 
                            factor = Config.factorHoliday2;
                            price = Config.price2;
                        }
                        if (car_type == 16) { 
                            factor = Config.factorHoliday3;
                            price = Config.price3;
                        }
                        if (car_type == 30) { 
                            factor = Config.factorHoliday4;
                            price = Config.price4;
                        }
                        if (car_type == 45) { 
                            factor = Config.factorHoliday5;
                            price = Config.price5;
                        }
                    }
                    else
                    {
                        if (car_type == 5)
                        {
                            factor = Config.factor1;
                            price = Config.price1;
                        }
                        if (car_type == 8)
                        {
                            factor = Config.factor2;
                            price = Config.price2;
                        }
                        if (car_type == 16)
                        {
                            factor = Config.factor3;
                            price = Config.price3;
                        }
                        if (car_type == 30)
                        {
                            factor = Config.factor4;
                            price = Config.price4;
                        }
                        if (car_type == 45)
                        {
                            factor = Config.factor5;
                            price = Config.price5;
                        }
                    }
                    total = price * factor * km/100;
                    return total;
                }
                return 1;
             }catch(Exception ex){
                 return 0;
             }
        }
        public string driverRegister(int? id, string name, string phone, string pass,string car_made, string car_model, int car_size, int car_year, string car_type, string card_identify, string license)
        {
            try
            {
                if (id == null)
                {
                    driver r = new driver();
                    r.name = name;
                    r.phone = phone;
                    MD5 md5Hash = MD5.Create();
                    string hash = Config.GetMd5Hash(md5Hash, pass);
                    r.pass = hash;
                    r.car_made = car_made;
                    r.car_model = car_model;
                    r.car_size = car_size;
                    r.car_years = car_year;
                    r.car_type = car_type;
                    r.date_time = DateTime.Now;
                    r.card_identify = card_identify;
                    r.license = license;
                    db.drivers.Add(r);
                    db.SaveChanges();
                    return r.id.ToString();
                }
                else
                {
                    driver r = db.drivers.Find(id);
                    db.Entry(r).State = EntityState.Modified;
                    r.name = name;
                    r.phone = phone;
                    r.car_made = car_made;
                    r.car_model = car_model;
                    r.car_size = car_size;
                    r.car_years = car_year;
                    r.car_type = car_type;
                    r.date_time = DateTime.Now;
                    r.card_identify = card_identify;
                    r.license = license;
                    db.SaveChanges();
                }

                return "1";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        public class gbol
        {
            public string car_from { get; set; }
            public string car_to { get; set; }
            public string car_type { get; set; }
            public string car_hire_type { get; set; }
            public string car_who_hire { get; set; }
            public DateTime? from_datetime { get; set; }
            public DateTime? to_datetime { get; set; }
            public DateTime? datebook { get; set; }
            public int? book_price { get; set; }
            public int? book_price_max { get; set; }
            public int? time_to_reduce { get; set; }
            public double lon1 { get; set; }
            public double lat1 { get; set; }
            public double lon2 { get; set; }
            public double lat2 { get; set; }
            public double D { get; set; }
        }
        public string getBooking(double lon,double lat,int order)
        {
            string query="select * from ";
            query += "(select car_from,car_to, car_type,car_hire_type,car_who_hire,from_datetime,to_datetime,datebook,book_price,book_price_max,time_to_reduce,lon1,lat1,lon2,lat2,ACOS(SIN(PI()*" + lat + "/180.0)*SIN(PI()*lat1/180.0)+COS(PI()*" + lat + "/180.0)*COS(PI()*lat1/180.0)*COS(PI()*lon1/180.0-PI()*" + lon + "/180.0))*6371 As D from booking) as A where D<300 ";
            if (order == null || order == 0)
            {
                query += " order by D ";
            }
            else
            {
                query += " order by datebook desc";
            }
            var p = db.Database.SqlQuery<gbol>(query);
            return JsonConvert.SerializeObject(p.ToList());
        }

        #region Drivers - duyvt

        public JsonResult getCarModelList(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return Json(db.list_car_model.Select(f => f.name ).ToList(), JsonRequestBehavior.AllowGet);
            }
            else {
                var data = db.list_car_model.Where(f => f.name.ToLower().IndexOf(keyword.ToLower()) != -1).Select(f => f.name).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}