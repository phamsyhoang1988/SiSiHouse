using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Resources;
using SiSiHouse.ViewModels;
using SiSiHouse.ViewModels.ManageCategory;
using SiSiHouse.WorkerServices;
using SiSiHouse.WorkerServices.Impl;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SiSiHouse.Controllers
{
    public class ManageCategoryController : ControllerBase
    {
        #region Constructor

        private readonly IManageCategoryService mainService;
        private readonly ICommonService commonService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageCategoryController()
            : this(new ManageCategoryService(), new CommonService())
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service"></param>
        public ManageCategoryController(IManageCategoryService service, ICommonService cmService)
        {
            this.mainService = service;
            this.commonService = cmService;
        }

        #endregion

        #region Action Result

        public ActionResult Index()
        {
            var model = new SearchCategoryModel();

            return this.View("List", model);
        }

        public ActionResult Update(int id = 0)
        {
            var model = new UpdateCategoryModel();

            if (id > 0)
            {
                model.CategoryInfo = this.mainService.GetCategoryInfo(id);
            }

            return this.View("Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateData(UpdateCategoryModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();

                    model.CategoryInfo.MODIFIED_DATE = DateTime.Now;
                    model.CategoryInfo.MODIFIED_USER_ID = loginUser.USER_ID;

                    if (this.mainService.UpdateCategoryInfo(model.CategoryInfo))
                    {
                        return RedirectToAction("Index", "ManageCategory");
                    }
                    else
                    {
                        ModelState.AddModelError("", Resources.Messages.E001);
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

        #endregion

        #region Ajax Action
        public ActionResult Search(DataTablesModel model, CategoryCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                int totalItem = 0;
                var categoryList = this.mainService.GetCategoryList(condition, model, out totalItem);
                IList<object> dataList = new List<object>();
                int rowIndex = 0;

                foreach (var data in categoryList)
                {
                    rowIndex++;
                    dataList.Add(new object[] {
                        data.CATEGORY_ID
                        , rowIndex
                        , HttpUtility.HtmlEncode(data.CATEGORY_NAME)
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
