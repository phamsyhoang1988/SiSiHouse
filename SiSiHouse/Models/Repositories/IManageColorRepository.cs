using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.Models.Repositories
{
    public interface IManageColorRepository
    {
        IList<Color> GetColorList(ColorCondition condition, DataTablesModel tableSetting, out int totalData);

        Color GetColorInfo(int colorID);

        int UpdateColorInfo(Color data);
    }
}
