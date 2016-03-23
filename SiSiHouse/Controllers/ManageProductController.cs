using SiSiHouse.Common;
using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Resources;
using SiSiHouse.ViewModels;
using SiSiHouse.ViewModels.ManageProduct;
//using SiSiHouse.ViewModels.ManageProduct;
using SiSiHouse.WorkerServices;
using SiSiHouse.WorkerServices.Impl;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Configuration;
using System.IO;

namespace SiSiHouse.Controllers
{
    public class ManageProductController : ControllerBase
    {
        #region Constructor

        private readonly IManageProductService mainService;

        private readonly ICommonService commonService;


        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageProductController()
            : this(new ManageProductService(), new CommonService())
        {
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="service"></param>
        public ManageProductController(IManageProductService service, ICommonService common)
        {
            this.mainService = service;
            this.commonService = common;
        }

        #endregion

        #region Action Result

        public ActionResult Index()
        {
            var model = new SearchProductModel
            {
                BrandSelectList = this.commonService.GetBrandList(),
                CategorySelectList = this.commonService.GetCategoryList()
            };

            return this.View("List", model);
        }

        public ActionResult Update(long id = 0)
        {
            var model = new UpdateProductModel
            {
                BrandSelectList = this.GetBrandSelectList(),
                CategorySelectList = this.GetCategorySelectList(),
                MoneySelectList = this.commonService.GetMoneyList(),
                ColorSelectList = this.commonService.GetColorList(),
            };

            if (id > 0)
            {
                model.PictureList = this.commonService.GetPictureList(id);

                model.ProductInfo = this.mainService.GetProductInfo(id);
                model.ProductDetailList = this.mainService.GetProductDetailList(id);
                model.ProductQuantityList = this.mainService.GetProductQuantityList(id);
            }

            return this.View("Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateData(UpdateProductModel model, IEnumerable<HttpPostedFileBase> pictureFiles)
        {
            List<string> newFiles = new List<string>();
            string srcPath = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();

                    model.ProductInfo.MODIFIED_DATE = DateTime.Now;
                    model.ProductInfo.MODIFIED_USER_ID = loginUser.USER_ID;

                    srcPath = Server.MapPath(Path.Combine(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_PICTURE], model.ProductInfo.PRODUCT_ID.ToString()));
                    int index = 0;

                    foreach (var picture in model.PictureList)
                    {
                        if (picture.PICTURE_ID > 0)
                        {
                            index++;
                        } else {
                            break;
                        }
                    }

                    // save file to destination folder
                    foreach (var file in pictureFiles)
                    {
                        if (file != null)
                        {
                            string newFile = UploadFile.SaveFile(file, srcPath);

                            model.PictureList[index].FILE_PATH = newFile;
                            newFiles.Add(newFile);
                        }

                        index++;
                    }

                    long newProductID = 0;

                    if (this.mainService.UpdateProductInfo(model.ProductInfo, model.ProductQuantityList, model.PictureList, out newProductID))
                    {
                        string desPath = Server.MapPath(Path.Combine(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_PICTURE], newProductID.ToString()));

                        // case insert new
                        if (model.ProductInfo.PRODUCT_ID == 0)
                        {
                            // move file from temp folder to new project folder
                            UploadFile.CreateFolder(desPath);

                            foreach (var picture in model.PictureList)
                            {
                                if (!string.IsNullOrEmpty(picture.FILE_PATH))
                                {
                                    UploadFile.MoveFile(Path.Combine(srcPath, picture.FILE_PATH), Path.Combine(desPath, picture.FILE_PATH));
                                }
                            }
                        }
                        else // case update
                        {
                            // delete file changed or deleted
                            foreach (var picture in model.PictureList)
                            {
                                if (picture.PICTURE_ID > 0 && (picture.DELETED.HasValue && picture.DELETED.Value))
                                {
                                    UploadFile.DeleteFile(Path.Combine(desPath, picture.FILE_PATH));
                                }
                            }
                        }

                        JsonResult result = Json(
                            new
                            {
                                statusCode = 201,
                                message = string.Format(Resources.Messages.I002)
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }
                    else
                    {
                        // delete all file uploaded when get error
                        if (newFiles.Count > 0)
                        {
                            foreach (string fileName in newFiles)
                            {
                                UploadFile.DeleteFile(Path.Combine(srcPath, fileName));
                            }
                        }

                        ModelState.AddModelError("", Resources.Messages.E001);
                    }
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                // delete all file uploaded when get error
                if (newFiles.Count > 0)
                {
                    foreach (string fileName in newFiles)
                    {
                        UploadFile.DeleteFile(Path.Combine(srcPath, fileName));
                    }
                }

                logger.Error(Messages.E001, ex);
                ModelState.AddModelError("", Resources.Messages.E001);
                return new EmptyResult();
            }
        }

        public ActionResult Retail(long id = 0)
        {
            if (id > 0)
            {
                var model = new UpdateRetailModel();

                if (id > 0)
                {
                    model.ProductInfo = this.mainService.GetProductInfo(id);
                    model.ProductInfo.PICTURE = this.GetPicturePath(id, model.ProductInfo.PICTURE);
                    model.ColorSelectListByProduct = this.GetColorSelectListByProduct(id);
                }

                return this.PartialView("Retail", model);
            }

            return new EmptyResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRetail(UpdateRetailModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginUser = GetLoginUser();

                    model.ProductInfo.MODIFIED_DATE = DateTime.Now;
                    model.ProductInfo.MODIFIED_USER_ID = loginUser.USER_ID;

                    if (this.mainService.UpdateRetail(model.ProductInfo, model.RetailList, false))
                    {
                        JsonResult result = Json(
                            new
                            {
                                statusCode = 201,
                                message = "Đã tạo hóa đơn bán lẻ thành công."
                            },
                            JsonRequestBehavior.AllowGet);

                        return result;
                    }
                    else
                    {
                        ModelState.AddModelError("", Resources.Messages.E001);
                    }
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                logger.Error(Messages.E001, ex);
                ModelState.AddModelError("", Resources.Messages.E001);
                return new EmptyResult();
            }
        }

        public ActionResult Delete(long id)
        {
            if (Request.IsAjaxRequest())
            {
                try
                {
                    if (this.mainService.DeleteProduct(id, GetLoginUser().USER_ID))
                    {
                        JsonResult result = Json(
                            new
                            {
                                statusCode = 201,
                                message = "Đã xóa sản phẩm thành công."
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

        #region Ajax Action
        public ActionResult Search(DataTablesModel model, ProductCondition condition)
        {
            if (Request.IsAjaxRequest())
            {
                int totalItem = 0;
                var productList = this.mainService.GetProductList(condition, model, out totalItem);
                IList<object> dataList = new List<object>();

                foreach (var data in productList)
                {
                    dataList.Add(new object[] {
                        data.PRODUCT_ID
                        , data.ROOT_LINK
                        , this.GetPicturePath(data.PRODUCT_ID, data.PICTURE)
                        , "<div>Mã: " + data.PRODUCT_CODE + "</div><div>Tên: " + HttpUtility.HtmlEncode(data.PRODUCT_NAME) + "</div>"
                        , "<div>Hãng: " + HttpUtility.HtmlEncode(data.BRAND_NAME) + "</div><div>Loại: " + HttpUtility.HtmlEncode(data.CATEGORY_NAME) + "</div>"
                        , this.BuildProductInStock(data.PRODUCT_ID)
                        , this.GetStatusName(data.STATUS_ID.ToString())
                        , this.BuildSalePrice(data.STATUS_ID.ToString(), data.SALE_PRICE, data.SALE_OFF_PRICE)
                        , Utility.InitialDecimal(data.REAL_PRICE).ToString("#,##0")
                        , "<div>" + data.MODIFIED_DATE.Value.ToString("yyyy/MM/dd") + "</div><div>" + data.MODIFIED_DATE.Value.ToString("HH:mm:ss") + "</div>"
                        , data.DELETE_FLAG
                        , data.STATUS_ID
                        , HttpUtility.HtmlEncode(data.PRODUCT_NAME)
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

        public ActionResult JsonGetSizeByColor(long productID, int colorID)
        {
            if (Request.IsAjaxRequest())
            {
                var sizeList = this.commonService.GetSizeByColor(productID, colorID);

                var result = Json(
                    new
                    {
                        data = sizeList
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }

            return new EmptyResult();
        }

        public ActionResult JsonGetDetailInStock(long productID, int colorID, string size)
        {
            if (Request.IsAjaxRequest())
            {
                var detail = this.commonService.GetDetailInStock(productID, colorID, size);

                var result = Json(
                    new
                    {
                        data = detail
                    },
                    JsonRequestBehavior.AllowGet);

                return result;
            }

            return new EmptyResult();
        }
        #endregion

        #region Private method

        private IList<SelectListItem> GetBrandSelectList()
        {
            return this.commonService.GetBrandList().Select(
                f =>
                new SelectListItem
                {
                    Value = f.BRAND_ID.ToString(),
                    Text = f.BRAND_NAME
                }).ToList();
        }

        private IList<SelectListItem> GetCategorySelectList()
        {
            return this.commonService.GetCategoryList().Select(
                f =>
                new SelectListItem
                {
                    Value = f.CATEGORY_ID.ToString(),
                    Text = f.CATEGORY_NAME
                }).ToList();
        }

        private IList<SelectListItem> GetColorSelectListByProduct(long productID)
        {
            return this.commonService.GetColorListByProduct(productID).Select(
                f =>
                new SelectListItem
                {
                    Value = f.COLOR_ID.ToString(),
                    Text = f.COLOR_NAME
                }).ToList();
        }

        private string BuildSalePrice(string status, decimal? sale, decimal? saleOff)
        {
            string html = "";

            if (Constant.Status.SALE_OFF == status)
            {
                html += "<div class='sale-off'>" + sale.Value.ToString("#,##0") + "</div>";
                html += "<div class='red'>" + saleOff.Value.ToString("#,##0") + "</div>";
            }
            else if (sale.HasValue)
            {
                html += sale.Value.ToString("#,##0");
            }

            return html;
        }

        private string BuildProductInStock(long productID)
        {
            var productDetailList = this.mainService.GetProductQuantityList(productID);
            int totalQuantity = 0;
            string detail = "";

            foreach (var productDetail in productDetailList)
            {
                totalQuantity += productDetail.QUANTITY;
                detail += "Màu " + productDetail.COLOR_NAME + " (" + productDetail.SIZE + "): " + productDetail.QUANTITY.ToString() + "\n";
            }

            string status = totalQuantity > 0 ? "Còn " + totalQuantity.ToString() : "Hết hàng";

            return "<i class='lbl-link' title='" + detail + "'>" + status + "</i>";
        }

        #endregion
    }
}
