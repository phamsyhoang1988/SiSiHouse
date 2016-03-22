using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace SiSiHouse.ViewModels.ManageStatistics
{
    public class SearchStatisticsModel
    {
        public StatisticsCondition Condition { get; set; }

        [DisplayName("Thương hiệu")]
        public IList<Brand> BrandSelectList { get; set; }

        [DisplayName("Phân loại")]
        public IList<Category> CategorySelectList { get; set; }

        public SearchStatisticsModel()
        {
            Condition = new StatisticsCondition();
        }
    }
}