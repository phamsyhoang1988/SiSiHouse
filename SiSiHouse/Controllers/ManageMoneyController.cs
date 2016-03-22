using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Resources;
using SiSiHouse.ViewModels;
using SiSiHouse.ViewModels.ManageMoney;
using SiSiHouse.WorkerServices;
using SiSiHouse.WorkerServices.Impl;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SiSiHouse.Controllers
{
    public class ManageMoneyController : ControllerBase
    {
        #region Constructor

        private readonly IManageMoneyService mainService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageMoneyController()
            : this(new ManageMoneyService())
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service"></param>
        public ManageMoneyController(IManageMoneyService service)
        {
            this.mainService = service;
        }

        #endregion

        #region Action Result

        public ActionResult Index()
        {
            var condition = new MoneyCondition();

            return this.View("List", condition);
        }

        public ActionResult Update(int id = 0)
        {
            var model = new UpdateMoneyModel();

            if (id > 0)
            {
                model.MoneyInfo = this.mainService.GetMoneyInfo(id);
            }

            return this.View("Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateData(UpdateMoneyModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();

                    model.MoneyInfo.MODIFIED_DATE = DateTime.Now;
                    model.MoneyInfo.MODIFIED_USER_ID = loginUser.USER_ID;

                    if (this.mainService.UpdateMoneyInfo(model.MoneyInfo))
                    {
                        return RedirectToAction("Index", "ManageMoney");
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
        public ActionResult Search(DataTablesModel model, MoneyCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                int totalItem = 0;
                var moneyList = this.mainService.GetMoneyList(condition, model, out totalItem);
                IList<object> dataList = new List<object>();
                int rowIndex = 0;

                foreach (var data in moneyList)
                {
                    rowIndex++;
                    dataList.Add(new object[] {
                        data.MONEY_ID
                        , rowIndex
                        , HttpUtility.HtmlEncode(data.MONEY_NAME)
                        , HttpUtility.HtmlEncode(data.MONEY_SIGN)
                        , data.APPLIED_DATE.Value.ToString("yyyy/MM/dd")
                        , data.EXCHANGE_RATE.ToString("#,##0.00") + " VNĐ"
                        , data.WEIGHT_POSTAGE.ToString("#,##0") + " VNĐ"
                        , data.WAGE.ToString() + "%"
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
