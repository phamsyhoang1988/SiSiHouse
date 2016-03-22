using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Resources;
using SiSiHouse.ViewModels;
using SiSiHouse.ViewModels.ManageColor;
using SiSiHouse.WorkerServices;
using SiSiHouse.WorkerServices.Impl;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SiSiHouse.Controllers
{
    public class ManageColorController : ControllerBase
    {
        #region Constructor

        private readonly IManageColorService mainService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageColorController()
            : this(new ManageColorService())
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service"></param>
        public ManageColorController(IManageColorService service)
        {
            this.mainService = service;
        }

        #endregion

        #region Action Result

        public ActionResult Index()
        {
            var condition = new ColorCondition();

            return this.View("List", condition);
        }

        public ActionResult Update(int id = 0)
        {
            var model = new UpdateColorModel();

            if (id > 0)
            {
                model.ColorInfo = this.mainService.GetColorInfo(id);
            }

            return this.View("Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateData(UpdateColorModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();

                    model.ColorInfo.MODIFIED_DATE = DateTime.Now;
                    model.ColorInfo.MODIFIED_USER_ID = loginUser.USER_ID;


                    if (this.mainService.UpdateColorInfo(model.ColorInfo))
                    {
                        return RedirectToAction("Index", "ManageColor");
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
        public ActionResult Search(DataTablesModel model, ColorCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                int totalItem = 0;
                var colorList = this.mainService.GetColorList(condition, model, out totalItem);
                IList<object> dataList = new List<object>();
                int rowIndex = 0;

                foreach (var data in colorList)
                {
                    rowIndex++;
                    dataList.Add(new object[] {
                        data.COLOR_ID
                        , rowIndex
                        , HttpUtility.HtmlEncode(data.COLOR_NAME)
                        , HttpUtility.HtmlEncode(data.COLOR_CODE)
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
