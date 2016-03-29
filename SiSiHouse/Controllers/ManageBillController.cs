using SiSiHouse.Common;
using SiSiHouse.Models.Conditions;
using SiSiHouse.ViewModels;
using SiSiHouse.ViewModels.ManageStatistics;
using SiSiHouse.WorkerServices;
using SiSiHouse.WorkerServices.Impl;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SiSiHouse.Controllers
{
    public class ManageBillController : ControllerBase
    {
        #region Constructor

        private readonly IManageStatisticsService mainService;

        private readonly ICommonService commonService;


        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageBillController()
            : this(new ManageStatisticsService(), new CommonService())
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service"></param>
        public ManageBillController(IManageStatisticsService service, ICommonService common)
        {
            this.mainService = service;
            this.commonService = common;
        }

        #endregion

        #region Action Result
        public ActionResult Index()
        {
            var model = new SearchStatisticsModel
            {
                BrandSelectList = this.commonService.GetBrandList(),
                CategorySelectList = this.commonService.GetCategoryList()
            };

            return this.View("List", model);
        }

        #endregion

        #region Ajax Action
        public ActionResult Search(DataTablesModel model, StatisticsCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                int totalItem = 0;
                var dataList = this.mainService.GetSalesStatisticsDetail(condition, model, out totalItem);
                IList<object> resultList = new List<object>();

                foreach (var data in dataList)
                {
                    resultList.Add(new object[] {
                        data.PRODUCT_ID
                        , Utility.GetPicturePath(data.PRODUCT_ID, data.PICTURE)
                        , data.PRODUCT_CODE
                        , HttpUtility.HtmlEncode(data.PRODUCT_NAME)
                        , HttpUtility.HtmlEncode(data.BRAND_NAME)
                        , HttpUtility.HtmlEncode(data.CATEGORY_NAME)
                        , HttpUtility.HtmlEncode(data.COLOR_NAME)
                        , HttpUtility.HtmlEncode(data.SIZE)
                        , data.QUANTITY
                        , data.SALES.ToString("#,##0")
                        , data.CREATED_DATE.Value.ToString("yyyy/MM/dd HH:mm:ss")
                        , HttpUtility.HtmlEncode(data.CREATED_USER)
                        , data.RETAIL_CODE
                        , data.PRODUCT_DETAIL_ID
                        , data.STATUS_ID
                    });
                }

                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = totalItem,
                        iTotalDisplayRecords = totalItem,
                        aaData = resultList
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }

            return new EmptyResult();
        }

        public ActionResult DoAction(BillCondition model)
        {
            if (Request.IsAjaxRequest())
            {
                try
                {
                    if (this.mainService.DoAction(model, GetLoginUser().USER_ID))
                    {
                        string messageSuccess = "Đã " + (model.IS_UNDO ? "trả lại" : "xóa") + " hóa đơn \"" + model.RETAIL_CODE + "\" thành công.";
                        JsonResult result = Json(
                            new
                            {
                                statusCode = 201,
                                message = messageSuccess
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }
                    else
                    {
                        ModelState.AddModelError("", Resources.Messages.E001);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    ModelState.AddModelError("", Resources.Messages.E001);
                    return new EmptyResult();
                }
            }

            return new EmptyResult();
        }

        #endregion
    }
}
