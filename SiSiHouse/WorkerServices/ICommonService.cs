using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiSiHouse.Models.Entities;
using System.Web.Mvc;

namespace SiSiHouse.WorkerServices
{
    public interface ICommonService
    {
        IList<Brand> GetBrandList();

        IList<Category> GetCategoryList();

        IList<Money> GetMoneyList();

        IList<Color> GetColorList();

        IList<Color> GetColorListByProduct(long productID);

        IList<ProductDetail> GetSizeByColor(long productID, int colorID);

        ProductDetail GetDetailInStock(long productID, int colorID, string size);

        IList<Artwork> GetArtworkList(long productID);
    }
}
