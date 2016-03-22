using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.Models.Repositories
{
    public interface IManageBrandRepository
    {
        IList<Brand> GetBrandList(BrandCondition condition, DataTablesModel tableSetting, out int totalData);

        Brand GetBrandInfo(int brandID);

        int UpdateBrandInfo(Brand data);
    }
}
