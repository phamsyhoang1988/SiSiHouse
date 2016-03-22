using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Models.Repositories;
using SiSiHouse.Models.Repositories.Impl;
using SiSiHouse.ViewModels;
using System.Collections.Generic;
using System.Transactions;

namespace SiSiHouse.WorkerServices.Impl
{
    public class ManageMoneyService : IManageMoneyService
    {
        private readonly IManageMoneyRepository _repository;

        public ManageMoneyService()
            : this(new ManageMoneyRepository())
        {
        }

        public ManageMoneyService(ManageMoneyRepository repository)
        {
            _repository = repository;
        }

        public IList<Money> GetMoneyList(MoneyCondition condition, DataTablesModel tableSetting, out int totalData)
        {
            return _repository.GetMoneyList(condition, tableSetting, out totalData);
        }

        public Money GetMoneyInfo(int moneyID)
        {
            return _repository.GetMoneyInfo(moneyID);
        }

        public bool UpdateMoneyInfo(Money data)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = (_repository.UpdateMoneyInfo(data) == 1);

                if (res)
                    transaction.Complete();
            }

            return res;
        }
    }
}
