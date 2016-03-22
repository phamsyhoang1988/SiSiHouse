using System.Web.Mvc;
using System.Web.Security;

namespace SiSiHouse.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            ViewBag.Error = true;

            return View("Error");
        }
    }
}
