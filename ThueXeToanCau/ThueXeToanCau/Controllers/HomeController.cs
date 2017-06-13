using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class HomeController : Controller
    {
        private thuexetoancauEntities db = new thuexetoancauEntities();
        public ActionResult Index()
        {
            ViewBag.name = Config.getCookie("name");
            ViewBag.phone = Config.getCookie("phone");
            //int price = 18000;
            //int factor = 100;
            //int km = 1624;
            //long total = (long)price * factor * km / 100;
            ViewBag.id_driver = Config.getCookie("id_driver");
            ViewBag.driver_number = Config.getCookie("driver_number");
            ViewBag.driver_phone = Config.getCookie("driver_phone");
            //Config.setCookie("id_driver", p2.id.ToString());
            //Config.setCookie("driver_number", p2.car_number != null ? p2.car_number.ToString() : "");
            //Config.setCookie("driver_phone", p2.phone != null ? p2.phone.ToString() : "");
            return View();
        }
        public ActionResult Policy()
        {
            return View();
        }

        public ActionResult About()
        {
            try { 
                ViewBag.about = db.infoes.FirstOrDefault().about;
            }
            catch
            {

            }
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
        public ActionResult ViewTrans()
        {
            return View();
        }
        public ActionResult ViewSalary()
        {
            return View();
        }
        public ActionResult ViewOwn()
        {
            return View();
        }
        public class expecel
        {
            
            public string car_number { get; set; }
            public double money { get; set; }
        }
        public void baocao(DateTime from_date,DateTime to_date)
        {
            string fts = "freetexttable";
            string query = "";
            StringBuilder rp = new StringBuilder();
            StringBuilder htmlContent = new StringBuilder();
            var filename = "";
            
            System.Web.HttpContext.Current.Response.ContentType = "application/force-download";
            System.Web.HttpContext.Current.Response.Write("<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");
            System.Web.HttpContext.Current.Response.Write("<head>");
            System.Web.HttpContext.Current.Response.Write("<META http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            System.Web.HttpContext.Current.Response.Write("<!--[if gte mso 9]><xml>");
            System.Web.HttpContext.Current.Response.Write("<x:ExcelWorkbook>");
            System.Web.HttpContext.Current.Response.Write("<x:ExcelWorksheets>");
            System.Web.HttpContext.Current.Response.Write("<x:ExcelWorksheet>");
            System.Web.HttpContext.Current.Response.Write("<x:Name>Report Data</x:Name>");
            System.Web.HttpContext.Current.Response.Write("<x:WorksheetOptions>");
            System.Web.HttpContext.Current.Response.Write("<x:Print>");
            System.Web.HttpContext.Current.Response.Write("<x:ValidPrinterInfo/>");
            System.Web.HttpContext.Current.Response.Write("</x:Print>");
            System.Web.HttpContext.Current.Response.Write("</x:WorksheetOptions>");
            System.Web.HttpContext.Current.Response.Write("</x:ExcelWorksheet>");
            System.Web.HttpContext.Current.Response.Write("</x:ExcelWorksheets>");
            System.Web.HttpContext.Current.Response.Write("</x:ExcelWorkbook>");
            System.Web.HttpContext.Current.Response.Write("</xml>");
            System.Web.HttpContext.Current.Response.Write("<![endif]--> ");
            System.Web.HttpContext.Current.Response.Write("</head>");
            try
            {
                query = "select * from (SELECT car_number, sum(money) as money from [thuexetoancau].[db_datareader].[transactions] where date>='" + from_date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "' and date<='" + to_date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "' ";
                query += "group by car_number) as A ";

                filename = "SaoKe_" + from_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + "_" + to_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                var p = db.Database.SqlQuery<expecel>(query).ToList();
                rp.Append("<tr><th>Biển số xe</th><th>Tổng số tiền</th><tr>");
                for (int i = 0; i < p.Count; i++)
                {
                    var item = p[i];
                    rp.Append("<tr><td>" + item.car_number + "</td><td>" + item.money + "</td><tr>");
                }
                htmlContent.Append("<h1>Thống kê từ ngày " + from_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " đến ngày " + to_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " </h1><table>" + rp.ToString() + "</table>");
                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");
                //System.Web.HttpContext.Current.Response.Write("<b>" + filename + "</b>");
                System.Web.HttpContext.Current.Response.Write(htmlContent.ToString());
                System.Web.HttpContext.Current.Response.Flush();

            }
            catch (Exception exmain)
            {
                return;
            }

        }
    }
}