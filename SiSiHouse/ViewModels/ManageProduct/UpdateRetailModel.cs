using SiSiHouse.Common;
using SiSiHouse.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace SiSiHouse.ViewModels.ManageProduct
{
    public class UpdateRetailModel
    {
        public Product ProductInfo { get; set; }

        public IList<SelectListItem> ColorSelectListByProduct { get; set; }

        public IList<Retail> RetailList { get; set; }

        public UpdateRetailModel()
        {
            ProductInfo = new Product();
            ColorSelectListByProduct = new List<SelectListItem>();
            RetailList = new List<Retail>();
        }
    }
}