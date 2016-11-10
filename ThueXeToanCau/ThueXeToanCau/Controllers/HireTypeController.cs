using PagedList;
using System.Linq;
using System.Web.Mvc;
using ThueXeToanCau.Models;

namespace ThueXeToanCau.Controllers
{
    public class HireTypeController : Controller
    {
        public ActionResult Index(int? page)
        {
            using (var db = new thuexetoancauEntities())
            {
                var hireTypes = db.car_hire_type;
                var pageNumber = page ?? 1;
                var onePage = hireTypes.OrderBy(f => f.name).ToPagedList(pageNumber, 20);

                ViewBag.onePage = onePage;
            }                
            return View();
        }

        [HttpPost]
        public string addUpdateHireType(car_hire_type ht)
        {
            return DBContext.addUpdateHireType(ht);
        }

        [HttpPost]
        public string deleteHireType(int id)
        {
            return DBContext.deleteHireType(id);
        }
    }
}