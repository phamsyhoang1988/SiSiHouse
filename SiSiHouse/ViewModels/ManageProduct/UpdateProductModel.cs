using SiSiHouse.Common;
using SiSiHouse.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace SiSiHouse.ViewModels.ManageProduct
{
    public class UpdateProductModel
    {
        public Product ProductInfo { get; set; }

        [DisplayName("Xóa")]
        public bool DELETE_FLAG
        {
            get { return (ProductInfo.DELETE_FLAG == Constant.DeleteFlag.DELETE); }
            set { ProductInfo.DELETE_FLAG = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        [DisplayName("Thương hiệu")]
        public IList<SelectListItem> BrandSelectList { get; set; }

        [DisplayName("Phân loại")]
        public IList<SelectListItem> CategorySelectList { get; set; }

        [DisplayName("Tiền tệ")]
        public IList<Money> MoneySelectList { get; set; }

        public IList<Color> ColorSelectList { get; set; }

        public IList<Artwork> ArtworkList { get; set; }

        public IList<ProductDetail> ProductDetailList { get; set; }

        public IList<ProductDetail> ProductQuantityList { get; set; }

        public UpdateProductModel()
        {
            ProductInfo = new Product();
            ProductDetailList = new List<ProductDetail>();
            ProductQuantityList = new List<ProductDetail>();
            ArtworkList = new List<Artwork>();
            BrandSelectList = new List<SelectListItem>();
            CategorySelectList = new List<SelectListItem>();
            MoneySelectList = new List<Money>();
            ColorSelectList = new List<Color>();
        }
    }
}