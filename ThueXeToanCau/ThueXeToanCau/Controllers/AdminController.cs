using System;
using System.Web.Mvc;
using ThueXeToanCau.Models;
using Excel;
using System.IO;
using System.Linq;
using System.Globalization;

namespace ThueXeToanCau.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (Config.getCookie("logged") != "") return RedirectToAction("Index");            
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            user u = DBContext.validateLogin(model);
            if(u==null)
            {
                ModelState.AddModelError("error", "Bạn đã nhập sai Tên hoặc Mật Khẩu, vui lòng thử lại!");
                return View(model);
            }
            Session["user"] = u;           
            return RedirectToAction("Index");
        }

        [HttpPost]
        public string validateLogin(LoginModel model)
        {
            try
            {
                user u = DBContext.validateLogin(model);
                if (u == null)
                {
                    return "Bạn đã nhập sai Tên hoặc Mật Khẩu, vui lòng thử lại!";
                }
                Config.setCookie("logged", u.name);
                return string.Empty;
            } catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
        }

        // GET: Admin
        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            //readExcelFile();
            return View();
        }

        public ActionResult ImportExcel()
        {
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
                                if(rowNumber == 0 || row.Cells.Length < 5)
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
                                    } else
                                    {
                                        DateTime.TryParseExact(dStr, "dd/MM/yyyy", CultureInfo.InstalledUICulture, DateTimeStyles.None, out dt);
                                    }
                                }
                                tran.date = dt;
                                tran.car_number = row.Cells[2] == null ? "" : row.Cells[2].Text;
                                tran.money = row.Cells[3] == null ? 0 : float.Parse(row.Cells[3].Text);
                                tran.note = row.Cells[4] == null ? "" : row.Cells[4].Text;
                                var tr = db.transactions.Where(f => f.type == tran.type && f.money == tran.money && f.car_number == tran.car_number
                                    && f.date.Day == tran.date.Day && f.date.Month == tran.date.Month && f.date.Year == tran.date.Year).FirstOrDefault();
                                if(tr == null)
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

        //public string readExcelFile()
        //{
        //    try
        //    {
        //        var filePath = Server.MapPath("~/Content/new.xlsx");
        //        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
        //        {
        //            WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
        //            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        //            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
        //            string text = string.Empty;
        //            foreach (Row r in sheetData.Elements<Row>())
        //            {
        //                foreach (Cell c in r.Elements<Cell>())
        //                {
        //                    text = "Not Found";
        //                    if (c != null && c.CellValue != null)
        //                    {
        //                        text = c.CellValue.Text;
        //                    }
        //                    Console.Write(text + " ");
        //                }
        //            }
        //        }
        //        //using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
        //        //{
        //        //    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
        //        //    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();

        //        //    OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
        //        //    string text;
        //        //    while (reader.Read())
        //        //    {
        //        //        if (reader.ElementType == typeof(CellValue))
        //        //        {
        //        //            text = reader.GetText();
        //        //            Console.Write(text + " ");
        //        //        }
        //        //    }
        //        //}
        //        return string.Empty;
        //    } catch(Exception ex)
        //    {
        //        return "Lỗi: " + ex.Message;
        //    }
        //}
    }
}