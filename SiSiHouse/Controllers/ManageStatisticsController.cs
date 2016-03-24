using SiSiHouse.Common;
using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Resources;
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
    public class ManageStatisticsController : ControllerBase
    {
        #region Constructor

        private readonly IManageStatisticsService mainService;

        private readonly ICommonService commonService;


        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageStatisticsController()
            : this(new ManageStatisticsService(), new CommonService())
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service"></param>
        public ManageStatisticsController(IManageStatisticsService service, ICommonService common)
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

            model.Condition.TARGET_YEAR = DateTime.Now.Year;

            return this.View("List", model);
        }

        public ActionResult Detail(int targetYear = 0, int targetMonth = 0)
        {
            if (targetYear <= 0 || targetMonth <= 0)
            {
                return RedirectToAction("Index", "Error");
            }

            var model = new SearchStatisticsModel
            {
                BrandSelectList = this.commonService.GetBrandList(),
                CategorySelectList = this.commonService.GetCategoryList()
            };

            model.Condition.TARGET_YEAR = targetYear;
            model.Condition.TARGET_MONTH = targetMonth;

            return this.View("Detail", model);
        }

        public ActionResult ExportRevenue(int targetYear = 0, int targetMonth = 0)
        {
            try
            {
                ExportExcelModel exportModel = new ExportExcelModel
                { 
                    TARGET_YEAR = targetYear,
                    TARGET_MONTH = targetMonth
                };

                StatisticsCondition condition = new StatisticsCondition
                {
                    TARGET_YEAR = targetYear,
                    TARGET_MONTH = targetMonth
                };

                var data = this.mainService.GetAllSalesStatisticsDetail(condition);

                Utility.ExportDataToExcel(this, exportModel, data);
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                logger.Error(Messages.E001, ex);
                return this.RedirectToAction("Index", "Error");
            }
        }

        #endregion

        #region Ajax Action
        public ActionResult Search(DataTablesModel model, StatisticsCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                int totalItem = 0;
                var dataList = this.mainService.GetSalesStatistics(condition, model);
                IList<object> resultList = new List<object>();
                decimal totalQuantity = 0;
                decimal totalCost = 0;
                decimal totalSales = 0;

                foreach (var data in dataList)
                {
                    totalQuantity += data.TOTAL_QUANTITY;
                    totalCost += data.TOTAL_COST;
                    totalSales += data.TOTAL_SALES;

                    resultList.Add(new object[] {
                        data.TARGET_YEAR
                        , data.TARGET_MONTH.ToString("00")
                        , data.TOTAL_QUANTITY.ToString("#,##0")
                        , data.TOTAL_COST.ToString("#,##0")
                        , data.TOTAL_SALES.ToString("#,##0")
                        , data.TOTAL_PROFIT.ToString("#,##0")
                        , data.TOTAL_SALES == 0 ? "0 %" : ((data.TOTAL_PROFIT / data.TOTAL_SALES) * 100).ToString("#,##0.00") + " %"
                    });
                }

                decimal totalProfit = totalSales - totalCost;

                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = totalItem,
                        iTotalDisplayRecords = totalItem,
                        aaData = resultList,
                        totalQuantity = totalQuantity,
                        totalCost = totalCost.ToString("#,##0"),
                        totalSales = totalSales.ToString("#,##0"),
                        totalProfit = totalProfit.ToString("#,##0"),
                        totalProfitRate = totalSales == 0 ? "0 %" : ((totalProfit / totalSales) * 100).ToString("#,##0.00") + " %"
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }

            return new EmptyResult();
        }

        public ActionResult SearchDetail(DataTablesModel model, StatisticsCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                int totalItem = 0;
                var dataList = this.mainService.GetSalesStatisticsDetail(condition, model, out totalItem);
                IList<object> resultList = new List<object>();
                decimal totalQuantity = 0;
                decimal totalCost = 0;
                decimal totalSales = 0;

                foreach (var data in dataList)
                {
                    var cost = data.QUANTITY * Utility.InitialDecimal(data.REAL_PRICE);
                    var profit = data.SALES - cost;

                    totalQuantity += data.QUANTITY;
                    totalCost += cost;
                    totalSales += data.SALES;

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
                        , cost.ToString("#,##0")
                        , profit.ToString("#,##0")
                        , data.SALES == 0 ? "0 %" : ((profit / data.SALES) * 100).ToString("#,##0.00") + " %"
                        , data.MODIFIED_DATE.Value.ToString("yyyy/MM/dd HH:mm:ss")
                    });
                }

                decimal totalProfit = totalSales - totalCost;

                var result = Json(
                    new
                    {
                        sEcho = model.sEcho,
                        iTotalRecords = totalItem,
                        iTotalDisplayRecords = totalItem,
                        aaData = resultList,
                        totalQuantity = totalQuantity,
                        totalCost = totalCost.ToString("#,##0"),
                        totalSales = totalSales.ToString("#,##0"),
                        totalProfit = totalProfit.ToString("#,##0"),
                        totalProfitRate = totalSales == 0 ? "0 %" : ((totalProfit / totalSales) * 100).ToString("#,##0.00") + " %"
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }

            return new EmptyResult();
        }

        #endregion
    }
}
