using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiSiHouse.Common;
using SiSiHouse.Resources;
using SiSiHouse.WorkerServices;
using SiSiHouse.WorkerServices.Impl;
using System.Web.Security;
using SiSiHouse.Models.Conditions;
using SiSiHouse.ViewModels;
using SiSiHouse.ViewModels.ManageUser;
using SiSiHouse.Models.Entities;

namespace SiSiHouse.Controllers
{
    public class SiSiController : ControllerBase
    {
        #region Constructor

        private readonly IUserService mainService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SiSiController()
            : this(new UserService())
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service"></param>
        public SiSiController(IUserService service)
        {
            this.mainService = service;
        }

        #endregion

        #region Action Result

        [AllowAnonymous]
        public ActionResult GetIn()
        {
            if (GetLoginUser() != null && (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "ManageProduct");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = this.mainService.Login(model.ACCOUNT, SafePassword.GetSha256(model.PASSWORD));

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.ACCOUNT, false);

                    LoginUser loginUser = new LoginUser()
                    {
                        USER_ID = user.USER_ID,
                        ACCOUNT = user.ACCOUNT,
                        FULL_NAME = user.FULL_NAME,
                        ROLE_ID = user.ROLE_ID
                    };

                    SetLoginUser(loginUser);

                    return RedirectToAction("Index", "ManageProduct");
                }
                else
                {
                    ModelState.AddModelError("", "Vui lòng kiểm tra lại tài khoản đăng nhập.");
                }
            }

            return View(model);
        }

        public ActionResult GetOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Family()
        {
            var model = new UserCondition();

            return this.View("List", model);
        }

        [HttpPost]
        public ActionResult Update(long id = 0)
        {
            var model = new UpdateUserModel();

            if (id > 0)
            {
                model.UserInfo = this.mainService.GetUserInfo(id);
            }

            return this.View("Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateData(UpdateUserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.UserInfo.ACCOUNT)
                        && this.mainService.CheckAccountExist(model.UserInfo.USER_ID, model.UserInfo.ACCOUNT.Trim()))
                    {
                        ModelState.AddModelError("", Resources.Messages.E007);
                    }
                    else
                    {
                        var loginUser = GetLoginUser();

                        model.UserInfo.MODIFIED_DATE = DateTime.Now;
                        model.UserInfo.MODIFIED_USER_ID = loginUser.USER_ID;

                        string tmpPassword = SafePassword.GetSha256(model.UserInfo.PASSWORD);

                        model.UserInfo.PASSWORD = (string.IsNullOrEmpty(model.UserInfo.PASSWORD) || model.UserInfo.PASSWORD == model.UserInfo.OLD_PASSWORD || model.UserInfo.OLD_PASSWORD == tmpPassword) ? model.UserInfo.OLD_PASSWORD : tmpPassword;

                        if (this.mainService.UpdateUserInfo(model.UserInfo))
                        {
                            return RedirectToAction("Family", "SiSi");
                        }
                        else
                        {
                            ModelState.AddModelError("", Resources.Messages.E001);
                        }
                    }
                }

                return View("Update", model);
            }
            catch (Exception ex)
            {
                logger.Error(Messages.E001, ex);
                return this.RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Personal()
        {
            var currentUser = GetLoginUser();
            var model = new UpdateUserModel();

            model.UserInfo = this.mainService.GetUserInfo(currentUser.USER_ID);

            return this.View("Personal", model);
        }

        #endregion

        #region Ajax Action
        public ActionResult Search(DataTablesModel model, UserCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                int totalItem = 0;
                var userList = this.mainService.GetUserList(condition, model, out totalItem);
                IList<object> dataList = new List<object>();
                int rowIndex = 0;

                foreach (var data in userList)
                {
                    rowIndex++;
                    dataList.Add(new object[] {
                        data.USER_ID
                        , rowIndex
                        , HttpUtility.HtmlEncode(data.FULL_NAME)
                        , data.ROLE_ID == Constant.Role.USER ? Constant.Role.Items[0] : Constant.Role.Items[1]
                        , HttpUtility.HtmlEncode(data.MOBILE)
                        , HttpUtility.HtmlEncode(data.EMAIL)
                        , HttpUtility.HtmlEncode(data.PRIVATE_PAGE)
                        , data.MODIFIED_DATE.Value.ToString("yyyy/MM/dd HH:mm:ss")
                        , HttpUtility.HtmlEncode(data.MODIFIED_USER)
                        , data.DELETE_FLAG
                    });
                }

                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = totalItem,
                        iTotalDisplayRecords = totalItem,
                        aaData = dataList
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }

            return new EmptyResult();
        }

        #endregion
    }
}
