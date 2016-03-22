using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.WorkerServices
{
    public interface IManageBrandService
    {
        IList<Brand> GetBrandList(BrandCondition condition, DataTablesModel tableSetting, out int totalData);

        Brand GetBrandInfo(int brandID);

        bool UpdateBrandInfo(Brand data);
    }
}
