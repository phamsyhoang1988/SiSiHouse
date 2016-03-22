using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.Models.Repositories
{
    public interface IManageMoneyRepository
    {
        IList<Money> GetMoneyList(MoneyCondition condition, DataTablesModel tableSetting, out int totalData);

        Money GetMoneyInfo(int moneyID);

        int UpdateMoneyInfo(Money data);
    }
}
