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
        public string booking(string car_from, string car_to, int? car_type, string car_hire_type, string car_who_hire, DateTime from_datetime, DateTime to_datetime, double? lon1,double? lat1,double? lon2,double? lat2,string name,string phone,string airport_name,string airport_way)
        {
            try
            {
                //get km distance
                int dts = 0;
                int price_basic=0;
                int price_per_day = 0;
                int price_max = getD(lon1, lat1, lon2, lat2, car_hire_type, car_type, from_datetime, to_datetime, airport_name, airport_way, ref dts, ref price_basic, ref price_per_day);
                int reduce = Config.reduct1;
                int price = price_max - price_max * reduce / 100;
                booking bo = new booking();
                bo.car_from = car_from;
                bo.car_hire_type = car_hire_type;
                bo.car_to = car_to;
                bo.car_type = car_type;
                bo.car_who_hire = car_who_hire;
                bo.from_datetime = from_datetime;
                if (!car_hire_type.ToLowerInvariant().Contains("sân bay")) { 
                    bo.geo1 = Config.CreatePoint(lat1, lon1);
                    bo.geo2 = Config.CreatePoint(lat2, lon2);
                    bo.lat1 = lat1;
                    bo.lat2 = lat2;
                    bo.lon1 = lon1;
                    bo.lon2 = lon2;
                }
                bo.to_datetime = to_datetime;
                bo.datebook = DateTime.Now;
                bo.book_price = price;
                bo.book_price_max = price_max;
                bo.time_to_reduce = 15 * 60;
                bo.name = name;
                bo.phone = phone;
                db.bookings.Add(bo);
                db.SaveChanges();
                return price_max.ToString() + "_" + dts + "_" + price_basic + "_" + price_per_day;
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }
        public string setcookie(string name,string phone)
        {
            Config.setCookie("name", name);
            Config.setCookie("phone", phone);
            return "1";
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
        public int getD(double? lon1, double? lat1, double? lon2, double? lat2, string car_hire_type, int? car_type, DateTime from_date, DateTime to_date, string airport_name, string airport_way,ref int dts,ref int price_basic,ref int price_per_day)
        {
            //get km distance
            try {
                string address = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + lat1 + "," + lon1 + "&destinations=" + lat2 + "," + lon2 + "&mode=driving&key=AIzaSyDLPSKQ4QV4xGiQjnZDUecx-UEr3D0QePY";
                string result = new System.Net.WebClient().DownloadString(address);
                var viewModel = new JavaScriptSerializer().Deserialize<RequestCepViewModel>(result);
                var dt = viewModel.rows[0].elements[0].distance.value;
                //return rs.rows.elements.distance.value;
                //JavaScriptSerializer jss = new JavaScriptSerializer();
                //var rs = jss.Deserialize<dynamic>(result);
                //var a=rs["rows"][0]["elements"][0]["distance"];//[0][1].ToString()
                //return a[1];
                int km= int.Parse(dt)/1000;
                dts = km;
                int factor = 100;
                int price=6000;
                int total = price*factor*km;
                int pricePerDay = price*200;
                int factorHoliday0 = (int)db.car_price.Where(o => o.car_size == car_type).FirstOrDefault().multiple;//Hệ số ngày lễ, Tết
                int factorHoliday1 = (int)db.car_price.Where(o=>o.car_size==5).FirstOrDefault().multiple;
                int factorHoliday2 = (int)db.car_price.Where(o => o.car_size == 8).FirstOrDefault().multiple;
                int factorHoliday3 = (int)db.car_price.Where(o => o.car_size == 16).FirstOrDefault().multiple;
                int factorHoliday4 = (int)db.car_price.Where(o => o.car_size == 30).FirstOrDefault().multiple;
                int factorHoliday5 = (int)db.car_price.Where(o => o.car_size == 45).FirstOrDefault().multiple;
                int price0 = (int)db.car_price.Where(o => o.car_size == car_type).FirstOrDefault().price;//bảng giá xe tương ứng số chỗ khứ hồi
                int price1 = (int)db.car_price.Where(o => o.car_size == 5).FirstOrDefault().price;
                int price2 = (int)db.car_price.Where(o => o.car_size == 8).FirstOrDefault().price;
                int price3 = (int)db.car_price.Where(o => o.car_size == 16).FirstOrDefault().price;
                int price4 = (int)db.car_price.Where(o => o.car_size == 30).FirstOrDefault().price;
                int price5 = (int)db.car_price.Where(o => o.car_size == 45).FirstOrDefault().price;
                int factor0 = (int)db.car_price.Where(o => o.car_size == car_type).FirstOrDefault().multiple2;//Hệ số xe một chiều
                int factor1 = (int)db.car_price.Where(o => o.car_size == 5).FirstOrDefault().multiple2;
                int factor2 = (int)db.car_price.Where(o => o.car_size == 8).FirstOrDefault().multiple2;
                int factor3 = (int)db.car_price.Where(o => o.car_size == 16).FirstOrDefault().multiple2;
                int factor4 = (int)db.car_price.Where(o => o.car_size == 30).FirstOrDefault().multiple2;
                int factor5 = (int)db.car_price.Where(o => o.car_size == 45).FirstOrDefault().multiple2;
                int factorBackWay_GoWith = Config.factorBackWay_GoWith;
                if (!car_hire_type.ToLowerInvariant().Contains("sân bay") && (car_hire_type.ToLowerInvariant().Contains("một chiều") || car_hire_type.ToLowerInvariant().Contains("chiều về") || car_hire_type.ToLowerInvariant().Contains("đi chung")))
                {
                    if (Config.isHoliDay(from_date))
                    {
                        factor = factorHoliday0;
                        price = price0 * factor0 / 100;
                        //if (car_type == 5) { 
                        //    factor = factorHoliday1;
                        //    price = price1 * factor1/100;
                        //}
                        //if (car_type == 8) { 
                        //    factor = factorHoliday2;
                        //    price = price2 * factor2/100;
                        //}
                        //if (car_type == 16) { 
                        //    factor = factorHoliday3;
                        //    price = price3 * factor3/100;
                        //}
                        //if (car_type == 30) { 
                        //    factor = factorHoliday4;
                        //    price = price4 * factor4/100;
                        //}
                        //if (car_type == 45) { 
                        //    factor = factorHoliday5;
                        //    price = price5 * factor5/100;
                        //}
                    }
                    else
                    {
                        factor = 100;
                        price = price0 * factor0 / 100;
                        //if (car_type == 5)
                        //{
                        //    factor = 100;
                        //    price = price1 * factor1 / 100;
                        //}
                        //if (car_type == 8)
                        //{
                        //    factor = 100;
                        //    price = price2 * factor2/ 100;
                        //}
                        //if (car_type == 16)
                        //{
                        //    factor = 100;
                        //    price = price3 * factor3/ 100;
                        //}
                        //if (car_type == 30)
                        //{
                        //    factor = 100;
                        //    price = price4 * factor4/ 100;
                        //}
                        //if (car_type == 45)
                        //{
                        //    factor = 100;
                        //    price = price5 * factor5/ 100;
                        //}
                    }
                    total = price * factor * km/100;
                    if (car_hire_type.ToLowerInvariant().Contains("chiều về") || car_hire_type.ToLowerInvariant().Contains("đi chung"))
                    {
                        total = total * factorBackWay_GoWith / 100;
                    }
                    price_basic = price;
                    return total;
                } else
                if (car_hire_type.ToLowerInvariant().Contains("sân bay"))
                {
                    if (Config.isHoliDay(from_date))
                    {
                        factor = factorHoliday0;
                        //if (car_type == 5)
                        //{
                        //    factor = factorHoliday1;
                            
                        //}
                        //if (car_type == 8)
                        //{
                        //    factor = factorHoliday2;
                           
                        //}
                        //if (car_type == 16)
                        //{
                        //    factor = factorHoliday3;
                           
                        //}
                        //if (car_type == 30)
                        //{
                        //    factor = factorHoliday4;
                            
                        //}
                        //if (car_type == 45)
                        //{
                        //    factor = factorHoliday5;
                           
                        //}
                    }
                    else
                    {
                        factor = 100;
                        //if (car_type == 5)
                        //{
                        //    factor = 100;
                            
                        //}
                        //if (car_type == 8)
                        //{
                        //    factor = 100;
                           
                        //}
                        //if (car_type == 16)
                        //{
                        //    factor = 100;
                          
                        //}
                        //if (car_type == 30)
                        //{
                        //    factor = 100;
                          
                        //}
                        //if (car_type == 45)
                        //{
                        //    factor = 100;
                           
                        //}
                    }
                       
                        if (airport_way.Contains("đi sân bay"))
                        {
                            price = (int)db.car_price_airport.Where(o => o.airport_name.Contains(airport_name)).Where(o => o.car_size == car_type).FirstOrDefault().price_go_way;
                        }
                        else
                            if (airport_way.Contains("sân bay về"))
                            {
                                price = (int)db.car_price_airport.Where(o => o.airport_name.Contains(airport_name)).Where(o => o.car_size == car_type).FirstOrDefault().price_back_way;
                            }
                            else
                                if (airport_way.Contains("khứ hồi"))
                                {
                                    price = (int)db.car_price_airport.Where(o => o.airport_name.Contains(airport_name)).Where(o => o.car_size == car_type).FirstOrDefault().price_two_way;
                                }
                   
                    
                    total = price * factor/ 100;
                    price_basic = price;
                    return total;
                }
                else
                if (car_hire_type.ToLowerInvariant().Contains("khứ hồi"))
                { 
                    
                    if (Config.isHoliDay(from_date))
                    {
                        factor = factorHoliday0;
                        price = price0;
                        //if (car_type == 5) { 
                        //    factor = factorHoliday1;
                        //    price = price1;                            
                        //}
                        //if (car_type == 8) { 
                        //    factor = factorHoliday2;
                        //    price = price2;
                        //}
                        //if (car_type == 16) { 
                        //    factor = factorHoliday3;
                        //    price = price3;
                        //}
                        //if (car_type == 30) { 
                        //    factor = factorHoliday4;
                        //    price = price4;
                        //}
                        //if (car_type == 45) { 
                        //    factor = factorHoliday5;
                        //    price = price5;
                        //}
                    }
                    else
                    {
                        factor = 100;
                        price = price0;
                        //if (car_type == 5)
                        //{
                        //    factor = 100;
                        //    price = price1;
                        //}
                        //if (car_type == 8)
                        //{
                        //    factor = 100;
                        //    price = price2;
                        //}
                        //if (car_type == 16)
                        //{
                        //    factor = 100;
                        //    price = price3;
                        //}
                        //if (car_type == 30)
                        //{
                        //    factor = 100;
                        //    price = price4;
                        //}
                        //if (car_type == 45)
                        //{
                        //    factor = 100;
                        //    price = price5;
                        //}
                    }
                    price_basic = price;
                    int days=Config.dateDiff(from_date, to_date)+1;
                    if (to_date.Day - from_date.Day==1) days=2;
                    pricePerDay = price * 200;
                    price_per_day = pricePerDay;
                    if (days >= 2)//Đi dưới 2 ngày, về trong ngày
                    {
                        km = km * 2;
                        if (km <= 200)
                        {
                            if (km < 200) km = 200;
                        }
                        total = price * factor * km / 100 + pricePerDay * factor * (days-1) / 100;
                        //6000*100*200*3/100=3600000, Hà Nội, Ninh Bình 3 ngày, 100km khứ hồi
                        return total;
                    }
                    else
                    {
                        km = km * 2;
                        if (km <= 200)
                        {
                            if (km < 100) km = 100;
                        }
                        total = price * factor * km / 100;
                        return total;
                    }
                }
                return 1;
             }catch(Exception ex){
                 return 0;
             }
        }
        public int getKm(double lon1, double lat1, double lon2, double lat2)
        {
            string address = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + lat1 + "," + lon1 + "&destinations=" + lat2 + "," + lon2 + "&mode=driving&key=AIzaSyDLPSKQ4QV4xGiQjnZDUecx-UEr3D0QePY";
            string result = new System.Net.WebClient().DownloadString(address);
            var viewModel = new JavaScriptSerializer().Deserialize<RequestCepViewModel>(result);
            var dt = viewModel.rows[0].elements[0].distance.value;
            int km = int.Parse(dt) / 1000;
            return km;
        }
        public string getLonLatAirport(string airport)
        {
            var p = (from q in db.airport_way select q).Where(o => o.airport_name.Contains(airport));
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string driverRegister(int? id, string name, string phone, string pass, string car_made, string car_model, int car_size, int car_year, string car_number, string car_type, string card_identify, string license,string regId,int? os)
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
                    r.car_number = car_number;
                    r.car_type = car_type;
                    r.date_time = DateTime.Now;
                    r.card_identify = card_identify;
                    r.license = license;
                    r.reg_id = regId;
                    r.os = os;
                    db.drivers.Add(r);
                    db.SaveChanges();
                    notify nt = new notify();
                    nt.os = os;
                    nt.reg_id = regId;
                    nt.tobject = 1;
                    db.notifies.Add(nt);
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
                    r.car_number = car_number;
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
            public long id { get; set; }
            public string car_from { get; set; }
            public string car_to { get; set; }
            public int? car_type { get; set; }
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
        public string getBooking(double lon,double lat,string car_hire_type,int? order)
        {
            string query="select top 100 * from ";
            query += "(select id,car_from,car_to, car_type,car_hire_type,car_who_hire,from_datetime,to_datetime,datebook,book_price,book_price_max,time_to_reduce,lon1,lat1,lon2,lat2,ACOS(SIN(PI()*" + lat + "/180.0)*SIN(PI()*lat1/180.0)+COS(PI()*" + lat + "/180.0)*COS(PI()*lat1/180.0)*COS(PI()*lon1/180.0-PI()*" + lon + "/180.0))*6371 As D from booking) as A where D<300 ";
            if (car_hire_type != null && car_hire_type != "")
            {
                query += " and car_hire_type=N'" + car_hire_type+"' ";
            }
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
        public class cname
        {
            public string name { get; set; }
        }
        public string getAirportName()
        {
            string query = "select distinct airport_name as name from car_price_airport order by name";
            var p = db.Database.SqlQuery<cname>(query);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getCarHireType()
        {
            var p = (from q in db.car_hire_type select q.name);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getCarType()
        {
            var p = (from q in db.car_type select q.name);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getListCarType()
        {
            var p = (from q in db.list_car_type select q.name);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getCarSize()
        {
            var p = (from q in db.car_type select q.name);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getCarWhoHire()
        {
            var p = (from q in db.car_who_hire select q.name);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public class car_model_made
        {
            public string name { get; set; }
        }
        public string getCarMadeList()
        {
            string query = "SELECT  name FROM [thuexetoancau].[dbo].[list_car] where name is not null order by no";
            var p = db.Database.SqlQuery<car_model_made>(query);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getCarModelListFromMade(string keyword)
        {
            if (keyword == null) keyword = "";
            string query = "SELECT  distinct model as name FROM [thuexetoancau].[dbo].[car_made_model] where made is not null and made like N'%" + keyword + "%' order by model";
            var p = db.Database.SqlQuery<car_model_made>(query);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string bookingFinal(long id_driver, string driver_number, string driver_phone, int price, long id_booking)
        {
            try
            {
                booking_final bf = new booking_final();
                bf.date_time = DateTime.Now;
                bf.driver_number = driver_number;
                bf.driver_phone = driver_phone;
                bf.id_booking = id_booking;
                bf.id_driver = id_driver;
                bf.price = price;
                db.booking_final.Add(bf);
                db.SaveChanges();
                return "1";
            }
            catch (Exception ex)
            {
                return "0";
            }
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