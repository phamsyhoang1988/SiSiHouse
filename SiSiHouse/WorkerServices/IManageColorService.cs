using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.WorkerServices
{
    public interface IManageColorService
    {
        IList<Color> GetColorList(ColorCondition condition, DataTablesModel tableSetting, out int totalData);

        Color GetColorInfo(int colorID);

        bool UpdateColorInfo(Color data);
    }
}
