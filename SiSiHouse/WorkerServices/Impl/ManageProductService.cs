using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Models.Repositories;
using SiSiHouse.Models.Repositories.Impl;
using SiSiHouse.ViewModels;
using System.Collections.Generic;
using System.Transactions;

namespace SiSiHouse.WorkerServices.Impl
{
    public class ManageProductService : IManageProductService
    {
        private readonly IManageProductRepository _repository;

        public ManageProductService()
            : this(new ManageProductRepository())
        {
        }

        public ManageProductService(ManageProductRepository repository)
        {
            _repository = repository;
        }


        public IList<Product> GetProductList(ProductCondition condition, DataTablesModel table, out int totalData)
        {
            return _repository.GetProductList(condition, table, out totalData);
        }

        public Product GetProductInfo(long productID)
        {
            return _repository.GetProductInfo(productID);
        }

        public IList<ProductDetail> GetProductDetailList(long productID)
        {
            return _repository.GetProductDetailList(productID);
        }

        public IList<ProductDetail> GetProductQuantityList(long productID)
        {
            return _repository.GetProductQuantityList(productID);
        }

        public bool UpdateProductInfo(Product data
            , IList<ProductDetail> dataDetail
            , IList<Picture> dataPicture
            , out long newProductID)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = _repository.UpdateProductInfo(data, dataDetail, dataPicture, out newProductID);

                if (res)
                    transaction.Complete();
            }

            return res;
        }

        public bool UpdateRetail(Product product, IList<Retail> retailList, bool isEdit)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = _repository.UpdateRetail(product, retailList, isEdit);

                if (res)
                    transaction.Complete();
            }

            return res;
        }

        public bool DeleteProduct(long productID, long userID)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = _repository.DeleteProduct(productID, userID);

                if (res)
                    transaction.Complete();
            }

            return res;
        }
    }
}
