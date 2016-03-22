using SiSiHouse.Common;
using SiSiHouse.Models.Entities;
using System.ComponentModel;

namespace SiSiHouse.ViewModels.ManageBrand
{
    public class UpdateBrandModel
    {
        public Brand BrandInfo { get; set; }

        [DisplayName("Xóa")]
        public bool DELETE_FLAG
        {
            get { return (BrandInfo.DELETE_FLAG == Constant.DeleteFlag.DELETE); }
            set { BrandInfo.DELETE_FLAG = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public UpdateBrandModel()
        {
            BrandInfo = new Brand();
        }
    }
}