using Excel;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ThueXeToanCau.Models;

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
            //// create a cookie
            //HttpCookie newCookie = new HttpCookie("userName", u.name);
            //newCookie.Expires = DateTime.Today.AddMinutes(30);
            //Request.Cookies.Add(newCookie);
            //Response.Cookies.Add(newCookie);
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
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        IExcelDataReader excelReader = null;
                        // get a stream
                        var stream = fileContent.InputStream;
                        var fileName = fileContent.FileName.ToLower();
                        if (fileName.EndsWith(".xls"))
                        {
                            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                        } else if (fileName.EndsWith(".xlsx"))
                        {
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        if (excelReader != null)
                        {
                            excelReader.IsFirstRowAsColumnNames = true;
                            DataSet result = excelReader.AsDataSet();
                            var sheet1 = result.Tables["HTX GT VT TOÀN CẦU"];
                            using (var db = new thuexetoancauEntities())
                            {
                                foreach (DataRow row in sheet1.Rows)
                                {
                                    var tran = new transaction();
                                    tran.type = row["NỘI DUNG"].ToString();
                                    dStr = row["NGÀY/THÁNG"].ToString();
                                    DateTime dt;
                                    bool isValidDate = true;
                                    if (!DateTime.TryParse(dStr, out dt))
                                    {
                                        if (!DateTime.TryParseExact(dStr,"dd/MM/yyyy",CultureInfo.InstalledUICulture,DateTimeStyles.None ,out dt))
                                        {
                                            isValidDate = false;
                                        }
                                    }
                                    if(isValidDate)
                                    {
                                        tran.date = dt;
                                    }
                                    tran.car_number = row["BIỂN SỐ XE"].ToString();
                                    tran.money = float.Parse(row["TỔNG TIỀN"].ToString());
                                    tran.note = row["GHI CHÚ"].ToString();
                                    rowNumber++;
                                    db.transactions.Add(tran);
                                }
                                db.SaveChanges();
                            }
                        }
                        else {
                            return "Lỗi: Xin hãy tải lên tệp tin Excel (.xls, .xlsx)!";
                        }                                                
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Lỗi: " + rowNumber + ":" + dStr + " - " + ex.Message;
            }
        }
    }
}