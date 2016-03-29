using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.Models.Repositories
{
    public interface IManageStatisticsRepository
    {
        IList<Statistics> GetSalesStatistics(StatisticsCondition condition, DataTablesModel tableSetting);

        IList<Product> GetSalesStatisticsDetail(StatisticsCondition condition, DataTablesModel tableSetting, out int totalData);

        IList<Product> GetAllSalesStatisticsDetail(StatisticsCondition condition);

        bool DoAction(BillCondition model, long updateUserID);
    }
}
