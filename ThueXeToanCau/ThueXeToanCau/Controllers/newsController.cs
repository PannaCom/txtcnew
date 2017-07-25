using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThueXeToanCau.Models;
using PagedList;
namespace ThueXeToanCau.Controllers
{
    public class newsController : Controller
    {
        private thuexetoancauEntities db = new thuexetoancauEntities();

        // GET: news
        public ActionResult Index(int? pg, string search)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            var data = (from q in db.news select q);
            if (data == null)
            {
                return View(data);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.new_title.Contains(search));
                ViewBag.search = search;
            }

            data = data.OrderByDescending(x => x.ngay_tao);

            return View(data.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult List(int? pg, string search,int? cat_id)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;

            var data = (from q in db.news select q);
            if (data == null)
            {
                return View(data);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                data = data.Where(x => x.new_title.Contains(search));
                ViewBag.search = search;
            }
            if (cat_id != null && cat_id!=0)
            {
                data = data.Where(x => x.cat_id==cat_id);
            }

            data = data.OrderByDescending(x => x.ngay_tao);

            return View(data.ToPagedList(pageNumber, pageSize));
        }

        // GET: news/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: news/Create
        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            ViewBag.danhmuctin = danhmuctin();
            ViewBag.trang_thai = new List<SelectListItem>() {
                new SelectListItem() { Value = "1", Text = "Công khai" },
                new SelectListItem() { Value = "2", Text = "Lưu Nháp" }
            };
            ViewBag.quyen_hang = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "1", Text = "Công khai" },
                new SelectListItem() { Value = "2", Text = "Chỉ thành viên trong nhóm" },
                new SelectListItem() { Value = "3", Text = "Bạn bè" },
                new SelectListItem() { Value = "4", Text = "Chỉ mình tôi" },
            };
            ViewBag.isHot = new List<SelectListItem>() { 
                new SelectListItem() { Value = "0", Text = "Tin thường" },
                new SelectListItem() { Value = "1", Text = "Tin nổi bật" }
            };
            return View();
        }
        public List<SelectListItem> danhmuctin()
        {
            var newList = db.cats.Select(x => new SelectListItem()
            {
                Value = x.id.ToString(),
                Text = x.cat_name
            }).ToList();
            return newList;
        }


        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(news news)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (!ModelState.IsValid)
            {
                TempData["error"] = "error! please check all the fields.";
                return RedirectToAction("Create");
            }

            news baiviet = new news();
            baiviet.new_title = news.new_title ?? null;
            baiviet.new_flug = baiviet.new_title != null ? Config.unicodeToNoMark(baiviet.new_title) : null;
            baiviet.cat_id = news.cat_id ?? null;
            baiviet.isHot = news.isHot ?? null;
            baiviet.new_content = news.new_content ?? null;
            baiviet.new_img = news.new_img ?? null;
            baiviet.ngay_tao = DateTime.Now;
            baiviet.quyen_hang = news.quyen_hang ?? null;
            baiviet.trang_thai = news.trang_thai ?? null;
            baiviet.new_des = news.new_des ?? null;
            //var userId = User.Identity.GetUserId();
            baiviet.user_id = "admin";
            db.news.Add(baiviet);
            db.SaveChanges();
            TempData["update"] = "Gửi bài thành công!";
            return RedirectToAction("Create");

        }
        public ActionResult uploadimg()
        {
            var fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}images\\news", Server.MapPath(@"\")));
                        string strDay = DateTime.Now.ToString("yyyyMM");
                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), strDay);

                        var _fileName = Guid.NewGuid().ToString("N") + ".jpg";

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, _fileName);
                        //System.Drawing.Image bm = System.Drawing.Image.FromStream(file.InputStream);
                        // Thay đổi kích thước ảnh
                        //bm = ResizeBitmap((Bitmap)bm, 100, 100); /// new width, height
                        //// Giảm dung lượng ảnh trước khi lưu
                        //ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                        //ImageCodecInfo ici = null;
                        //foreach (ImageCodecInfo codec in codecs)
                        //{
                        //    if (codec.MimeType == "image/jpeg")
                        //        ici = codec;
                        //}
                        //EncoderParameters ep = new EncoderParameters();
                        //ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)80);
                        //bm.Save(path, ici, ep);
                        //bm.Save(path);
                        file.SaveAs(path);
                        FileInfo f2 = new FileInfo(path);
                        if (f2.Length > 100000)
                        {
                            int percent = 50;
                            if (f2.Length > 1000000) percent = 10;
                            else
                                if (f2.Length > 500000) percent = 20;
                                else if (f2.Length > 300000) percent = 30;
                            ImageProcessor.ImageFactory iFF = new ImageProcessor.ImageFactory();
                            iFF.Load(path).Quality(percent).Save(path);
                        }
                        fName = "/images/news/" + strDay + "/" + _fileName;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { Message = fName }, JsonRequestBehavior.AllowGet);
        }
        // GET: news/Edit/5
        public ActionResult Edit(long? id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            ViewBag.danhmuctin = danhmuctin();
            ViewBag.trang_thai = new List<SelectListItem>() {
                new SelectListItem() { Value = "1", Text = "Công khai" },
                new SelectListItem() { Value = "2", Text = "Lưu Nháp" }
            };
            ViewBag.quyen_hang = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "1", Text = "Công khai" },
                new SelectListItem() { Value = "2", Text = "Chỉ thành viên trong nhóm" },
                new SelectListItem() { Value = "3", Text = "Bạn bè" },
                new SelectListItem() { Value = "4", Text = "Chỉ mình tôi" },
            };
            ViewBag.isHot = new List<SelectListItem>() { 
                new SelectListItem() { Value = "0", Text = "Tin thường" },
                new SelectListItem() { Value = "1", Text = "Tin nổi bật" }
            };
            return View(news);
        }

        // POST: news/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,new_title,new_content,new_flug,new_img,cat_id,user_id,quyen_hang,trang_thai,isHot,ngay_tao,new_des")] news news)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var baiviet = db.news.Find(news.id);

            baiviet.new_title = news.new_title ?? null;
            baiviet.new_flug = baiviet.new_title != null ? Config.unicodeToNoMark(baiviet.new_title) : null;
            baiviet.cat_id = news.cat_id ?? null;
            baiviet.isHot = news.isHot ?? null;
            baiviet.new_content = news.new_content ?? null;
            baiviet.new_img = news.new_img ?? null;
            //baiviet.ngay_tao = DateTime.Now;
            baiviet.quyen_hang = news.quyen_hang ?? null;
            baiviet.trang_thai = news.trang_thai ?? null;
            baiviet.new_des = news.new_des ?? null;
            var userId = "admin";
            baiviet.user_id = userId;
            TempData["update"] = "Cập nhật bài viết thành công.";
            db.Entry(baiviet).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = news.id });

        }

        // GET: news/Delete/5
        public ActionResult Delete(long? id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: news/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            news news = db.news.Find(id);
            db.news.Remove(news);
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
