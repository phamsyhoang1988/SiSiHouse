using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.Models.Repositories
{
    public interface IManageCategoryRepository
    {
        IList<Category> GetCategoryList(CategoryCondition condition, DataTablesModel tableSetting, out int totalData);

        Category GetCategoryInfo(int categoryID);

        int UpdateCategoryInfo(Category data);
    }
}
