using Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class TransactionController : Controller
    {
        public ActionResult Index()
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
        public JsonResult searchTran(string carNumber, string fDate, string tDate, bool isDetail)
        {
            try
            {
                using (var db = new thuexetoancauEntities())
                {
                    var trans = db.transactions.Where(f => f.car_number == carNumber);
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
                    trans = trans.OrderByDescending(f => f.date);
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
                             }).ToList();
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            } catch (Exception ex)
            {
                return Json(new { ErrMess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}