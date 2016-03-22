using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.WorkerServices
{
    public interface IManageStatisticsService
    {
        IList<Statistics> GetSalesStatistics(StatisticsCondition condition, DataTablesModel tableSetting);

        IList<Product> GetSalesStatisticsDetail(StatisticsCondition condition, DataTablesModel tableSetting, out int totalData);

        IList<Product> GetAllSalesStatisticsDetail(StatisticsCondition condition);
    }
}
