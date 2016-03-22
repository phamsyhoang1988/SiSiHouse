using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.WorkerServices
{
    public interface IManageCategoryService
    {
        IList<Category> GetCategoryList(CategoryCondition condition, DataTablesModel tableSetting, out int totalData);

        Category GetCategoryInfo(int categoryID);

        bool UpdateCategoryInfo(Category data);
    }
}
