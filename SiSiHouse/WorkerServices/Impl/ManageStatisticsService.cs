using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Models.Repositories;
using SiSiHouse.Models.Repositories.Impl;
using SiSiHouse.ViewModels;
using System.Collections.Generic;
using System.Transactions;

namespace SiSiHouse.WorkerServices.Impl
{
    public class ManageStatisticsService : IManageStatisticsService
    {
        private readonly IManageStatisticsRepository _repository;

        public ManageStatisticsService()
            : this(new ManageStatisticsRepository())
        {
        }

        public ManageStatisticsService(ManageStatisticsRepository repository)
        {
            _repository = repository;
        }

        public IList<Statistics> GetSalesStatistics(StatisticsCondition condition, DataTablesModel tableSetting)
        {
            return _repository.GetSalesStatistics(condition, tableSetting);
        }

        public IList<Product> GetSalesStatisticsDetail(StatisticsCondition condition, DataTablesModel tableSetting, out int totalData)
        {
            return _repository.GetSalesStatisticsDetail(condition, tableSetting, out totalData);
        }

        public IList<Product> GetAllSalesStatisticsDetail(StatisticsCondition condition)
        {
            return _repository.GetAllSalesStatisticsDetail(condition);
        }

        public bool DoAction(BillCondition model, long UpdateUserID)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = _repository.DoAction(model, UpdateUserID);

                if (res)
                    transaction.Complete();
            }

            return res;
        }
    }
}
