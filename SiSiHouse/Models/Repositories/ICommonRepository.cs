using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;
using System.Text;

namespace SiSiHouse.Models.Repositories
{
    public interface ICommonRepository
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
