using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiSiHouse.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            var view = View();

            if (Request.Browser.IsMobileDevice)
            {
                view.MasterName = "~/Views/Shared/MobileLayout.cshtml"; 
            }

            return view;
        }
    }
}
