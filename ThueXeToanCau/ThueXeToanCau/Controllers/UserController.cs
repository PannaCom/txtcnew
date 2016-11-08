using PagedList;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index(int? page)
        {
            using (var db = new thuexetoancauEntities())
            {
                var users = db.users;
                var pageNumber = page ?? 1;
                var onePage = users.OrderBy(f => f.name).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        [HttpPost]
        public string addUpdateUser(user u)
        {
            return DBContext.addUpdateUser(u);
        }

        [HttpPost]
        public string deleteUser(int uId)
        {
            return DBContext.deleteUser(uId);
        }
    }
}