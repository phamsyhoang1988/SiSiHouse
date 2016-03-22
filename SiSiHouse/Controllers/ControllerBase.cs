using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SiSiHouse.Common;
using SiSiHouse.Resources;
using System.Configuration;
using System.IO;

namespace SiSiHouse.Controllers
{
    /// <summary>
    /// BaseController for all the controllers
    /// </summary>
    [Authorize]
    [ValidateInput(false)]
    public abstract class ControllerBase : Controller
    {
        protected static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string SESSION_SITEMAP = "SESSION_SITEMAP";

        /// <summary>
        /// Return the login user object
        /// </summary>
        /// <returns></returns>
        protected LoginUser GetLoginUser()
        {
            LoginUser user = Session[Constant.SESSION_LOGIN_USER] as LoginUser;

            //if (user == null)
            //{
            //    filterContext.Result = new RedirectResult(Url.Action("GetIn", "SiSi"));
            //}

            return user;
        }

        /// <summary>
        /// Set the login user object
        /// </summary>
        /// <param name="user"></param>
        protected void SetLoginUser(LoginUser user)
        {
            Session[Constant.SESSION_LOGIN_USER] = user;
        }

        /// <summary>
        /// This method is called before the acion method
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Request.IsAjaxRequest())
            {
                var routeData = filterContext.RouteData;
                var controller = routeData.Values["controller"].ToString();
                var action = routeData.Values["action"].ToString();

                var loginUser = GetLoginUser();

                if (loginUser == null && controller.Contains("Manage"))
                {
                    filterContext.Result = new RedirectResult(Url.Action("GetIn", "SiSi"));
                    return;
                }

                if (loginUser != null
                    && Constant.Role.USER == loginUser.ROLE_ID
                    && (action == "Family" || Constant.AdminManage.Contains(controller)))
                {
                    filterContext.Result = new RedirectResult(Url.Action("Index", "ManageProduct"));
                    return;
                }

                int pos = Sitemap.FindIndex(item => item.ControllerName == controller);

                if (0 <= pos)
                {
                    Sitemap.RemoveRange(pos, 0);
                }
                else
                {
                    var item = new SitemapItem
                    {
                        ControllerName = controller,
                        RestoreData = null
                    };

                    Sitemap.Insert(0, item);
                }

            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (Request.IsAjaxRequest())
            {
                if (!ModelState.IsValid)
                {
                    filterContext.Result = Json(
                        new
                        {
                            ErrorMessages = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)
                        }
                    );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;

            if (viewResult != null)
            {
                viewResult.ViewBag.LoginUser = GetLoginUser();
                viewResult.ViewBag.WindowName = GetWindowName();
            }

            base.OnResultExecuting(filterContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null)
            {
                var loginUser = GetLoginUser();
                if (loginUser != null)
                {
                    logger.FatalFormat("Logged in user： [{0}]", loginUser.ACCOUNT);
                }
                logger.Fatal(Messages.E999, filterContext.Exception);
            }

            base.OnException(filterContext);
        }

        /// <summary>
        /// Go back to the previous screen
        /// </summary>
        /// <param name="n">steps</param>
        /// <returns>ActionResult</returns>
        protected ActionResult Backward(int n)
        {
            if (0 < n && n < Sitemap.Count)
            {
                var sitemap = Sitemap[n];

                return Backward(sitemap);
            }
            else
            {
                return new EmptyResult();
            }
        }

        protected ActionResult Backward(string controllerName)
        {
            int pos = Sitemap.FindIndex(item => item.ControllerName == controllerName);
            if (0 <= pos)
            {
                var sitemap = Sitemap[pos];

                return Backward(sitemap);
            }
            else
            {
                return new EmptyResult();
            }
        }

        private ActionResult Backward(SitemapItem sitemap)
        {
            if (sitemap != null)
            {
                var factory = ControllerBuilder.Current.GetControllerFactory();
                var controller = factory.CreateController(Request.RequestContext, sitemap.ControllerName);

                string actionName = "Index";
                if (controller is IRestoreStateController)
                {
                    actionName = "Restore";
                }

                factory.ReleaseController(controller);

                return RedirectToAction(actionName, sitemap.ControllerName);
            }
            else
            {
                return new EmptyResult();
            }
        }

        protected object GetRestoreData()
        {
            object data = null;

            int pos = Sitemap.FindIndex(item => item.ControllerName == RouteData.Values["controller"].ToString());
            if (0 <= pos)
            {
                data = Sitemap[pos].RestoreData;
                //Sitemap[pos].RestoreData = null;
            }

            return data;
        }

        protected void SaveRestoreData(object data)
        {
            if (data != null)
            {
                int pos = Sitemap.FindIndex(item => item.ControllerName == RouteData.Values["controller"].ToString());
                if (0 <= pos)
                {
                    Sitemap[pos].RestoreData = data;
                }
            }
        }

        private string GetWindowName()
        {
            var controller = RouteData.Values["controller"].ToString();

            string windowName = Constant.WindowName.MAIN;

            HttpCookie cookie = Request.Cookies[Constant.WindowName.COOKIE_NAME];
            if (cookie != null)
            {
                if (Constant.WindowName.Items.Contains(controller))
                {
                    cookie.Value = Constant.WindowName.Items[controller] as string;
                }

                windowName = cookie.Value;
            }

            return windowName;
        }

        private List<SitemapItem> Sitemap
        {
            get
            {
                var windowName = GetWindowName();
                var sitemaps = Session[SESSION_SITEMAP] as IDictionary<string, List<SitemapItem>>;

                if (sitemaps == null)
                {
                    sitemaps = new Dictionary<string, List<SitemapItem>>();
                    foreach (string name in Constant.WindowName.Items.Values)
                    {
                        sitemaps.Add(name, new List<SitemapItem>());
                    }

                    Session[SESSION_SITEMAP] = sitemaps;
                }

                var sitemap = sitemaps[windowName];

                return sitemap;
            }
        }

        [Serializable]
        private class SitemapItem
        {
            public string ControllerName { get; set; }

            public object RestoreData { get; set; }
        }

        private void PrintSitemap()
        {
            var windowName = GetWindowName();
            var sitemaps = Session[SESSION_SITEMAP] as IDictionary<string, List<SitemapItem>>;

            var sitemap = sitemaps[windowName];

            var sb = new System.Text.StringBuilder();
            sb.Append(string.Format("\nSitemap stack [{0}]\n", windowName));
            foreach (var item in sitemap.ToArray().Reverse())
            {
                sb.AppendFormat("\t{0}\n", item.ControllerName);
            }
            sb.Append("---------");

            logger.Debug(sb.ToString());
        }

        protected string GetArtworkPath(long productID, string fileName)
        {
            string filePath = string.IsNullOrEmpty(fileName) ? "" : Path.Combine(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_ARTWORK], productID.ToString(), fileName);

            return filePath;
        }

        protected string GetStatusName(string statusID)
        {
            string statusName = Constant.Status.Items[3].ToString();

            switch (statusID)
            {
                case Constant.Status.WAITING:
                    statusName = Constant.Status.Items[0].ToString();
                    break;
                case Constant.Status.SELLING:
                    statusName = Constant.Status.Items[1].ToString();
                    break;
                case Constant.Status.SALE_OFF:
                    statusName = Constant.Status.Items[2].ToString();
                    break;
            }

            return statusName;
        }
    }
}
