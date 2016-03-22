using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace SiSiHouse.ViewModels.ManageProduct
{
    public class SearchProductModel
    {
        public ProductCondition Condition { get; set; }

        [DisplayName("Thương hiệu")]
        public IList<Brand> BrandSelectList { get; set; }

        [DisplayName("Phân loại")]
        public IList<Category> CategorySelectList { get; set; }

        public string CallBack { get; set; }

        public SearchProductModel()
        {
            Condition = new ProductCondition();
        }
    }
}