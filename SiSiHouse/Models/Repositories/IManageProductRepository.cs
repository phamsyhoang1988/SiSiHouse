﻿using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.Models.Repositories
{
    public interface IManageProductRepository
    {
        IList<Product> GetProductList(ProductCondition condition, DataTablesModel table, out int totalData);

        Product GetProductInfo(long productID);

        IList<ProductDetail> GetProductDetailList(long productID);

        IList<ProductDetail> GetProductQuantityList(long productID);

        bool UpdateProductInfo(Product data, IList<ProductDetail> dataDetail, IList<Picture> dataPicture, out long newProductID);

        bool UpdateRetail(Product product, IList<Retail> retailList, bool isEdit);

        bool DeleteProduct(long productID, long userID);

        int CountProduct(CollectionCondition condition);

        IList<Product> GetCollection(CollectionCondition condition, DataTablesModel table);
    }
}
