using SiSiHouse.Common;
using SiSiHouse.Models.Conditions;
using SiSiHouse.ViewModels;
using SiSiHouse.WorkerServices;
using SiSiHouse.WorkerServices.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiSiHouse.Controllers
{
    public class ShowController : Controller
    {
        #region Constructor

        private readonly IManageProductService mainService;

        private readonly ICommonService commonService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ShowController()
            : this(new ManageProductService(), new CommonService())
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service"></param>
        public ShowController(IManageProductService service, ICommonService common)
        {
            this.mainService = service;
            this.commonService = common;
        }

        #endregion

        #region Action
        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToAction("", "Home");
        }

        [AllowAnonymous]
        public ActionResult Collection(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("", "Home");
            }

            CollectionCondition condition = this.CreateCondition(id.ToLower());

            ViewBag.TotalProduct = this.mainService.CountProduct(condition);
            ViewBag.Title = id;

            return View();
        }

        [AllowAnonymous]
        public ActionResult Item(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("", "Home");
            }

            long productID = Convert.ToInt64(id);

            CollectionItemModel model = new CollectionItemModel
            {
                ProductInfo = this.mainService.GetCollectionItem(productID),
                PictureList = this.commonService.GetPictureList(productID)
            };

            if (Convert.ToInt16(Constant.Status.SALE_OFF) == model.ProductInfo.STATUS_ID)
            {
                model.ProductInfo.DISPLAY_PRICE = model.ProductInfo.SALE_OFF_PRICE.HasValue ? model.ProductInfo.SALE_OFF_PRICE.Value.ToString("#,##0") + Constant.VND : "";
            }
            else if (model.ProductInfo.SALE_OFF_PRICE.HasValue)
            {
                model.ProductInfo.DISPLAY_PRICE = model.ProductInfo.SALE_PRICE.Value.ToString("#,##0") + Constant.VND;
            }

            foreach (var img in model.PictureList)
            {
                img.IMG_SRC = Utility.GetPicturePath(productID, img.FILE_PATH);
            }

            ViewBag.Title = model.ProductInfo.PRODUCT_NAME;

            return View(model);
        }

        #endregion

        #region Ajax Action
        [AllowAnonymous]
        public ActionResult List(string type, int countItem = 0)
        {
            if (string.IsNullOrEmpty(type) || !Request.IsAjaxRequest())
            {
                return new EmptyResult();
            }

            CollectionCondition condition = this.CreateCondition(type.ToLower());
            DataTablesModel table = new DataTablesModel {
                iDisplayStart = countItem,
                iDisplayLength = Constant.DISPLAY_ITEM_PER_PAGE,
                sColumns = "IMPORT_DATE",
                iSortCol_0 = 0,
                sSortDir_0 = "DESC"
            };

            var productList = this.mainService.GetCollection(condition, table);
            IList<object> dataList = new List<object>();

            foreach (var data in productList)
            {
                dataList.Add(new object[] {
                    data.PRODUCT_ID
                    , Utility.GetPicturePath(data.PRODUCT_ID, data.PICTURE_1)
                    , Utility.GetPicturePath(data.PRODUCT_ID, data.PICTURE_2)
                });
            }

            var result = Json(
                     new
                     {
                         data = dataList
                     },
                     JsonRequestBehavior.AllowGet);

            return result;
        }
        #endregion

        #region Private Method
        private CollectionCondition CreateCondition(string key)
        {
            CollectionCondition condition = new CollectionCondition();

            switch (key)
            {
                case Constant.CategoryName.CLOTHES:
                    condition.CATEGORY_TYPE = Constant.CategoryType.CLOTHES;
                    break;
                case Constant.CategoryName.FOOTWEARS:
                    condition.CATEGORY_TYPE = Constant.CategoryType.FOOTWEARS;
                    break;
                case Constant.CategoryName.ACCESSORIES:
                    condition.CATEGORY_TYPE = Constant.CategoryType.ACCESSORIES;
                    break;
                default:
                    condition.CATEGORY_NAME = key;
                    break;
            }

            return condition;
        }

        #endregion
    }
}
