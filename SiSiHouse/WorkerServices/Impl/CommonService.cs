using SiSiHouse.Models.Entities;
using SiSiHouse.Models.Repositories;
using SiSiHouse.Models.Repositories.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiSiHouse.WorkerServices.Impl
{
    public class CommonService: ICommonService
    {
        private ICommonRepository _repository;

        public CommonService()
            : this(new CommonRepository())
        {
        }

        public CommonService(ICommonRepository _repository)
        {
            this._repository = _repository;
        }

        public IList<Brand> GetBrandList()
        {
            return _repository.GetBrandList();
        }

        public IList<Category> GetCategoryList()
        {
            return _repository.GetCategoryList();
        }

        public IList<Money> GetMoneyList()
        {
            return _repository.GetMoneyList();
        }

        public IList<Color> GetColorList()
        {
            return _repository.GetColorList();
        }

        public IList<Color> GetColorListByProduct(long productID)
        {
            return _repository.GetColorListByProduct(productID);
        }

        public IList<ProductDetail> GetSizeByColor(long productID, int colorID)
        {
            return _repository.GetSizeByColor(productID, colorID);
        }

        public ProductDetail GetDetailInStock(long productID, int colorID, string size)
        {
            return _repository.GetDetailInStock(productID, colorID, size);
        }

        public IList<Picture> GetPictureList(long productID)
        {
            return _repository.GetPictureList(productID);
        }
    }
}