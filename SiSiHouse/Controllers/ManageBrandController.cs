using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Resources;
using SiSiHouse.ViewModels;
using SiSiHouse.ViewModels.ManageBrand;
using SiSiHouse.WorkerServices;
using SiSiHouse.WorkerServices.Impl;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SiSiHouse.Controllers
{
    public class ManageBrandController : ControllerBase
    {
        #region Constructor

        private readonly IManageBrandService mainService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageBrandController()
            : this(new ManageBrandService())
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service"></param>
        public ManageBrandController(IManageBrandService service)
        {
            this.mainService = service;
        }

        #endregion

        #region Action Result

        public ActionResult Index()
        {
            var condition = new BrandCondition();

            return this.View("List", condition);
        }

        public ActionResult Update(int id = 0)
        {
            var model = new UpdateBrandModel();

            if (id > 0)
            {
                model.BrandInfo = this.mainService.GetBrandInfo(id);
            }

            return this.View("Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateData(UpdateBrandModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();

                    model.BrandInfo.MODIFIED_DATE = DateTime.Now;
                    model.BrandInfo.MODIFIED_USER_ID = loginUser.USER_ID;

                    if (this.mainService.UpdateBrandInfo(model.BrandInfo))
                    {
                        return RedirectToAction("Index", "ManageBrand");
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
        public ActionResult Search(DataTablesModel model, BrandCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                int totalItem = 0;
                var brandList = this.mainService.GetBrandList(condition, model, out totalItem);
                IList<object> dataList = new List<object>();
                int rowIndex = 0;

                foreach (var data in brandList)
                {
                    rowIndex++;
                    dataList.Add(new object[] {
                        data.BRAND_ID
                        , rowIndex
                        , HttpUtility.HtmlEncode(data.BRAND_NAME)
                        , HttpUtility.HtmlEncode(data.DESCRIPTION)
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
