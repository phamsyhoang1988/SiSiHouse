using SiSiHouse.Common;
using SiSiHouse.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace SiSiHouse.ViewModels.ManageMoney
{
    public class UpdateMoneyModel
    {
        public Money MoneyInfo { get; set; }

        [DisplayName("Xóa")]
        public bool DELETE_FLAG
        {
            get { return (MoneyInfo.DELETE_FLAG == Constant.DeleteFlag.DELETE); }
            set { MoneyInfo.DELETE_FLAG = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public UpdateMoneyModel()
        {
            MoneyInfo = new Money();
        }
    }
}