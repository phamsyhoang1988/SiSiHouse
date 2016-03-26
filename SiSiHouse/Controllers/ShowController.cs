using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiSiHouse.Controllers
{
    public class ShowController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToAction("", "Home");
        }

        [AllowAnonymous]
        public ActionResult Collection(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("", "Home");
            }

            ViewBag.Title = id;
            return View();
        }

        [AllowAnonymous]
        public ActionResult List(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new EmptyResult();
            }

            var result = Json(
                     new
                     {
                         data = "co data"
                     },
                     JsonRequestBehavior.AllowGet);

            return result;
        }
    }
}
