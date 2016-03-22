using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Models.Repositories;
using SiSiHouse.Models.Repositories.Impl;
using SiSiHouse.ViewModels;
using System.Collections.Generic;
using System.Transactions;

namespace SiSiHouse.WorkerServices.Impl
{
    public class ManageCategoryService : IManageCategoryService
    {
        private readonly IManageCategoryRepository _repository;

        public ManageCategoryService()
            : this(new ManageCategoryRepository())
        {
        }

        public ManageCategoryService(ManageCategoryRepository repository)
        {
            _repository = repository;
        }

        public IList<Category> GetCategoryList(CategoryCondition condition, DataTablesModel tableSetting, out int totalData)
        {
            return _repository.GetCategoryList(condition, tableSetting, out totalData);
        }

        public Category GetCategoryInfo(int categoryID)
        {
            return _repository.GetCategoryInfo(categoryID);
        }

        public bool UpdateCategoryInfo(Category data)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = (_repository.UpdateCategoryInfo(data) == 1);

                if (res)
                    transaction.Complete();
            }

            return res;
        }
    }
}
