using Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class TransactionController : Controller
    {
        private thuexetoancauEntities db = new thuexetoancauEntities();
        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }
        public ActionResult Bank()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }
        public ActionResult Salary()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }
        public ActionResult Own()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }
        [HttpPost]
        public string uploadFile()
        {
            var rowNumber = 0;
            var dStr = string.Empty;
            try
            {
                var folder = Server.MapPath("~/App_Data");
                using (var db = new thuexetoancauEntities())
                {
                    foreach (string file in Request.Files)
                    {
                        var fileName = Request.Files[file].FileName;
                        if (db.duplicateFiles.Any(o => o.name == fileName))
                        {
                            throw new ArgumentException("File này đã tồn tại");
                        }
                        else
                        {
                            duplicateFile dF = new duplicateFile();
                            dF.name = fileName;
                            db.duplicateFiles.Add(dF);
                            db.SaveChanges();
                        }
                        var fileContent = Request.Files[file];
                        var filePath = Path.Combine(folder, file);
                        fileContent.SaveAs(filePath);
                        foreach (var worksheet in Workbook.Worksheets(filePath))
                        {
                            foreach (var row in worksheet.Rows)
                            {
                                if (rowNumber == 0 || row.Cells.Length < 5)
                                {
                                    rowNumber++;
                                    continue;
                                }
                                var tran = new transaction();
                                tran.type = row.Cells[0] == null ? "" : row.Cells[0].Text;
                                dStr = row.Cells[1] == null ? "" : row.Cells[1].Text;
                                DateTime dt = DateTime.MinValue;
                                if (!string.IsNullOrEmpty(dStr))
                                {
                                    double dbDate;
                                    if (double.TryParse(dStr, out dbDate))
                                    {
                                        dt = DateTime.FromOADate(dbDate);
                                    }
                                    else
                                    {
                                        DateTime.TryParseExact(dStr, "dd/MM/yyyy", CultureInfo.InstalledUICulture, DateTimeStyles.None, out dt);
                                    }
                                }
                                tran.date = dt;
                                tran.car_number = row.Cells[2] == null ? "" : row.Cells[2].Text;
                                tran.money = row.Cells[3] == null ? 0 : float.Parse(row.Cells[3].Text);
                                tran.note = row.Cells[4] == null ? "" : row.Cells[4].Text;
                                //var tr = db.transactions.Where(f => f.type == tran.type && f.money == tran.money && f.car_number == tran.car_number
                                //    && f.date.Day == tran.date.Day && f.date.Month == tran.date.Month && f.date.Year == tran.date.Year).FirstOrDefault();
                                if (tran.money != null && tran.note!=null)
                                {
                                    db.transactions.Add(tran);
                                    db.SaveChanges();
                                }
                                rowNumber++;
                            }
                        }
                    }
                }
                return "Thêm dữ liệu thành công!";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + rowNumber + ":" + dStr + " - " + ex.Message;
            }
        }
        [HttpPost]
        public string uploadFileBank(DateTime? from_date, DateTime? to_date)
        {
            var rowNumber = 0;
            var dStr = string.Empty;
            try
            {
                var folder = Server.MapPath("~/App_Data");
                using (var db = new thuexetoancauEntities())
                {
                    foreach (string file in Request.Files)
                    {
                        var fileContent = Request.Files[file];
                        var filePath = Path.Combine(folder, file);
                        fileContent.SaveAs(filePath);
                        foreach (var worksheet in Workbook.Worksheets(filePath))
                        {
                            foreach (var row in worksheet.Rows)
                            {
                                if (rowNumber == 0 || row.Cells.Length < 4)
                                {
                                    rowNumber++;
                                    continue;
                                }
                                var tran = new driver_bank();
                                tran.car_number = row.Cells[0] == null ? "" : row.Cells[0].Text;
                                tran.driver_name = row.Cells[1] == null ? "" : row.Cells[1].Text;
                                tran.bank_number = row.Cells[2] == null ? "" : row.Cells[2].Text;
                                tran.bank_name = row.Cells[3] == null ? "" : row.Cells[3].Text;

                                var tr = db.driver_bank.Any(f => f.car_number == tran.car_number);
                                //    && f.date.Day == tran.date.Day && f.date.Month == tran.date.Month && f.date.Year == tran.date.Year).FirstOrDefault();
                                if (!tr && tran.car_number != null && tran.car_number != "" && tran.car_number != "Biển xe" && tran.driver_name != null)
                                {
                                    db.driver_bank.Add(tran);
                                    db.SaveChanges();
                                }
                                rowNumber++;
                            }
                        }
                    }
                }
                return "Thêm dữ liệu thành công!";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + rowNumber + ":" + dStr + " - " + ex.Message;
            }
        }
        [HttpPost]
        public string uploadFileSalary(DateTime? from_date, DateTime? to_date)
        {
            var rowNumber = 0;
            var dStr = string.Empty;
            try
            {
                var folder = Server.MapPath("~/App_Data");
                using (var db = new thuexetoancauEntities())
                {
                    foreach (string file in Request.Files)
                    {
                        var fileName = Request.Files[file].FileName;
                        if (db.duplicateFiles.Any(o => o.name == fileName))
                        {
                            throw new ArgumentException("File này đã tồn tại");
                        }
                        else
                        {
                            duplicateFile dF = new duplicateFile();
                            dF.name = fileName;
                            db.duplicateFiles.Add(dF);
                            db.SaveChanges();
                        }

                        var fileContent = Request.Files[file];
                        var filePath = Path.Combine(folder, file);
                        fileContent.SaveAs(filePath);
                        foreach (var worksheet in Workbook.Worksheets(filePath))
                        {
                            try { 
                                foreach (var row in worksheet.Rows)
                                {
                                    try
                                    {
                                        if (rowNumber == 0 || row.Cells.Length < 5)
                                        {
                                            rowNumber++;
                                            continue;
                                        }
                                        var tran = new driver_salary();
                                        tran.car_number = row.Cells[0] == null ? "" : row.Cells[0].Text;
                                        tran.driver_name = row.Cells[1] == null ? "" : row.Cells[1].Text;
                                        tran.money = row.Cells[2] == null ? 0 : float.Parse(row.Cells[2].Text);
                                        tran.bank_number = row.Cells[3] == null ? "" : row.Cells[3].Text;
                                        tran.bank_name = row.Cells[4] == null ? "" : row.Cells[4].Text;
                                        tran.from_date = from_date;
                                        tran.to_date = to_date;
                                        var tr = db.driver_salary.Any(f => f.car_number == tran.car_number);
                                        //    && f.date.Day == tran.date.Day && f.date.Month == tran.date.Month && f.date.Year == tran.date.Year).FirstOrDefault();
                                        if (tran.car_number != null && tran.car_number != "" && tran.car_number != "Biển xe" && tran.driver_name != null)
                                        {
                                            db.driver_salary.Add(tran);
                                            db.SaveChanges();
                                        }
                                        rowNumber++;
                                    }
                                    catch (Exception v0)
                                    {

                                    }
                                }
                            }
                            catch (Exception v1)
                            {

                            }
                        }
                    }
                }
                return "Thêm dữ liệu thành công!";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + rowNumber + ":" + dStr + " - " + ex.Message;
            }
        }
        [HttpGet]
        public string searchCarNumber(string keyword = "")
        {
            var numbers = new List<string>();
            using (var db = new thuexetoancauEntities())
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    numbers = db.transactions.OrderBy(f=>f.car_number)
                        .Select(f => f.car_number).Distinct().Take(50).ToList();
                }
                else
                {
                    numbers = db.transactions.Where(f => f.car_number.Replace("-", "").Replace(".", "")
                        .StartsWith(keyword.Replace("-", "").Replace(".", "")))
                        .OrderBy(f => f.car_number).Select(f => f.car_number)
                        .Distinct().Take(50).ToList();
                }                               
            }
            return JsonConvert.SerializeObject(numbers);
        }
        [HttpGet]
        public string searchCarNumberBank(string keyword = "")
        {
            var numbers = new List<string>();
            using (var db = new thuexetoancauEntities())
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    numbers = db.driver_bank.OrderBy(f => f.car_number)
                        .Select(f => f.car_number).Distinct().Take(50).ToList();
                }
                else
                {
                    numbers = db.driver_bank.Where(f => f.car_number.Replace("-", "").Replace(".", "")
                        .StartsWith(keyword.Replace("-", "").Replace(".", "")))
                        .OrderBy(f => f.car_number).Select(f => f.car_number)
                        .Distinct().Take(50).ToList();
                }
            }
            return JsonConvert.SerializeObject(numbers);
        }
        [HttpPost]
        public string DelTransaction(int id,string fDate,string tDate)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from transactions where date>='" + fDate + "' and date<='" + tDate+"'");
                return "1";
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }
        [HttpPost]
        public string DelSalary(int id, string fDate, string tDate)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from driver_salary where from_date>='" + fDate + "' and to_date<='" + tDate + "'");
                return "1";
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }
        [HttpPost]
        public string DelAllBank()
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from driver_bank");
                return "1";
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }
        [HttpPost]
        public string DelBank(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from driver_bank where id=" + id);
                return "1";
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }
        [HttpGet]
        public JsonResult searchTran(string carNumber, string fDate, string tDate, bool isDetail)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (carNumber == null) carNumber = "";
                    var trans = db.transactions.Where(f => f.car_number.Contains(carNumber));
                    if(!string.IsNullOrEmpty(fDate))
                    {
                        DateTime fromDate;
                        if(DateTime.TryParse(fDate,out fromDate))
                        {
                            trans = trans.Where(f => f.date >= fromDate);
                        }
                    }
                    if (!string.IsNullOrEmpty(tDate))
                    {
                        DateTime toDate;
                        if (DateTime.TryParse(tDate, out toDate))
                        {
                            trans = trans.Where(f => f.date <= toDate);
                        }
                    }
                    if (carNumber != "")
                    {
                        trans = trans.OrderByDescending(f => f.date);
                    }
                    else {
                        trans = trans.Take(100).OrderByDescending(f => f.date);
                    }
                    if (isDetail)
                    {
                        return Json(trans.ToList(), JsonRequestBehavior.AllowGet);
                    }
                    var rs = (from tr in trans
                             group tr by tr.car_number into g
                             select new 
                             {
                                 carNum = g.FirstOrDefault().car_number,
                                 sum = g.Sum(f => f.money),
                                 count = g.Count(),
                             }).Take(1000).ToList();
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            } catch (Exception ex)
            {
                return Json(new { ErrMess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult searchTranBank(string carNumber)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (carNumber == null) carNumber = "";                    
                    var trans = db.driver_bank.Where(f => f.car_number.Contains(carNumber));
                    trans = trans.OrderByDescending(f => f.car_number).Take(500);
                    return Json(trans.ToList(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { ErrMess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult searchTranSalary(string carNumber, string fDate, string tDate)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    if (carNumber == null) carNumber = "";
                    var trans = db.driver_salary.Where(f => f.car_number.Contains(carNumber));
                    if (!string.IsNullOrEmpty(fDate))
                    {
                        DateTime fromDate;
                        if (DateTime.TryParse(fDate, out fromDate))
                        {
                            trans = trans.Where(f => f.from_date>= fromDate);
                        }
                    }
                    if (!string.IsNullOrEmpty(tDate))
                    {
                        DateTime toDate;
                        if (DateTime.TryParse(tDate, out toDate))
                        {
                            trans = trans.Where(f => f.to_date <= toDate);
                        }
                    }
                    trans = trans.OrderByDescending(f => f.car_number).Take(500);
                    return Json(trans.ToList(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { ErrMess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public class expecel
        {

            public string car_number { get; set; }
            public string driver_name { get; set; }          
            public double money { get; set; }
            public string bank_number { get; set; }
            public string bank_name { get; set; }
        }
        public void baocao(DateTime from_date, DateTime to_date,int? type)
        {
            string fts = "freetexttable";
            string query = "";
            StringBuilder rp = new StringBuilder();
            StringBuilder htmlContent = new StringBuilder();
            var filename = "";

            //System.Web.HttpContext.Current.Response.ContentType = "application/excel";//force-download
            //System.Web.HttpContext.Current.Response.Write("<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");
            //System.Web.HttpContext.Current.Response.Write("<head>");
            //System.Web.HttpContext.Current.Response.Write("<META http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            //System.Web.HttpContext.Current.Response.Write("<!--[if gte mso 9]><xml>");
            //System.Web.HttpContext.Current.Response.Write("<x:ExcelWorkbook>");
            //System.Web.HttpContext.Current.Response.Write("<x:ExcelWorksheets>");
            //System.Web.HttpContext.Current.Response.Write("<x:ExcelWorksheet>");
            //System.Web.HttpContext.Current.Response.Write("<x:Name>Report Data</x:Name>");
            //System.Web.HttpContext.Current.Response.Write("<x:WorksheetOptions>");
            //System.Web.HttpContext.Current.Response.Write("<x:Print>");
            //System.Web.HttpContext.Current.Response.Write("<x:ValidPrinterInfo/>");
            //System.Web.HttpContext.Current.Response.Write("</x:Print>");
            //System.Web.HttpContext.Current.Response.Write("</x:WorksheetOptions>");
            //System.Web.HttpContext.Current.Response.Write("</x:ExcelWorksheet>");
            //System.Web.HttpContext.Current.Response.Write("</x:ExcelWorksheets>");
            //System.Web.HttpContext.Current.Response.Write("</x:ExcelWorkbook>");
            //System.Web.HttpContext.Current.Response.Write("</xml>");
            //System.Web.HttpContext.Current.Response.Write("<![endif]--> ");
            //System.Web.HttpContext.Current.Response.Write("</head>");
            try
            {
                query+="select A.car_number,B.driver_name,A.money,B.bank_number,B.bank_name from ( ";
                query+=" SELECT car_number, sum(money) as money from [thuexetoancau].[db_datareader].[transactions] where date>='" + from_date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "' and date<='" + to_date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "' group by car_number) as A ";
                query += " inner join (select car_number,driver_name,bank_number,bank_name from driver_bank) as B on A.car_number=B.car_number order by money ";
 

                filename = "LuongTaiXe" + from_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + "_" + to_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                var p = db.Database.SqlQuery<expecel>(query).ToList();
                rp.Append("<tr><th>Biển Xe</th><th>Họ tên</th><th>Số tiền</th><th>Số tài khoản</th><th>Ngân hàng</th><tr>");
                for (int i = 0; i < p.Count; i++)
                {
                    var item = p[i];
                    double money = item.money;
                    if (type == 1) money = money - 600000;
                    rp.Append("<tr><td>" + item.car_number + "</td><td>" + item.driver_name + "</td><td>" + money + "</td><td> &nbsp;" + item.bank_number.ToString() + "</td><td>" + item.bank_name + "</td><tr>");
                }
                //htmlContent.Append("<h1>LỆNH CHUYỂN TIỀN " + from_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " đến ngày " + to_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " </h1><table cellspacing=0 cellpadding=0 border=\"1\">" + rp.ToString() + "</table>");
                //System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");
                ////System.Web.HttpContext.Current.Response.Write("<b>" + filename + "</b>");
                //System.Web.HttpContext.Current.Response.Write(htmlContent.ToString());
                //System.Web.HttpContext.Current.Response.Flush();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.BufferOutput = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment; filename=Salary.xls");
                Response.Write("<table cellspacing=0 cellpadding=0 border=\"1\">" + rp.ToString() + "</table>");
                //Response.Write(htmlContent.ToString());
                Response.Flush();
                Response.Close();
                Response.End();

            }
            catch (Exception exmain)
            {
                return;
            }

        }
    }
}