using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.WorkerServices
{
    public interface IManageMoneyService
    {
        IList<Money> GetMoneyList(MoneyCondition condition, DataTablesModel tableSetting, out int totalData);

        Money GetMoneyInfo(int moneyID);

        bool UpdateMoneyInfo(Money data);
    }
}
