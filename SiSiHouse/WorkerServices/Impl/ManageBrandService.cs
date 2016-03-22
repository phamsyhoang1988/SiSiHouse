using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Models.Repositories;
using SiSiHouse.Models.Repositories.Impl;
using SiSiHouse.ViewModels;
using System.Collections.Generic;
using System.Transactions;

namespace SiSiHouse.WorkerServices.Impl
{
    public class ManageBrandService : IManageBrandService
    {
        private readonly IManageBrandRepository _repository;

        public ManageBrandService()
            : this(new ManageBrandRepository())
        {
        }

        public ManageBrandService(ManageBrandRepository repository)
        {
            _repository = repository;
        }

        public IList<Brand> GetBrandList(BrandCondition condition, DataTablesModel tableSetting, out int totalData)
        {
            return _repository.GetBrandList(condition, tableSetting, out totalData);
        }

        public Brand GetBrandInfo(int brandID)
        {
            return _repository.GetBrandInfo(brandID);
        }

        public bool UpdateBrandInfo(Brand data)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = (_repository.UpdateBrandInfo(data) == 1);

                if (res)
                    transaction.Complete();
            }

            return res;
        }
    }
}
