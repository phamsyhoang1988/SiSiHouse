using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiSiHouse.Controllers
{
    public class CommonController :  ControllerBase
    {
        //
        // GET: /Common/

        [AllowAnonymous]
        public ActionResult AuthentTimeout()
        {
            Session.Clear();

            if (Request.IsAjaxRequest())
            {
                this.Response.StatusCode = 419;
                return new EmptyResult();
            }
            else
            {
                return RedirectToAction("GetIn", "SiSi");
            }
        }

    }
}
