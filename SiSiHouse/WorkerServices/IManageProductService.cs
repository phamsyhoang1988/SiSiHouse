using SiSiHouse.Common;
using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.WorkerServices
{
    public interface IManageProductService
    {
        IList<Product> GetProductList(ProductCondition condition, DataTablesModel table, out int totalData);

        Product GetProductInfo(long productID);

        IList<ProductDetail> GetProductDetailList(long productID);

        IList<ProductDetail> GetProductQuantityList(long productID);

        bool UpdateProductInfo(Product data, IList<ProductDetail> dataDetail, IList<Artwork> dataArtwork, out long newProductID);

        bool UpdateRetail(Product product, IList<Retail> retailList, bool isEdit);

        bool DeleteProduct(long productID);
    }
}
