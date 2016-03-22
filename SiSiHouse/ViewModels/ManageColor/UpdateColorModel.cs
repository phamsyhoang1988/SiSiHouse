using SiSiHouse.Common;
using SiSiHouse.Models.Entities;
using System.ComponentModel;

namespace SiSiHouse.ViewModels.ManageColor
{
    public class UpdateColorModel
    {
        public Color ColorInfo { get; set; }

        [DisplayName("Xóa")]
        public bool DELETE_FLAG
        {
            get { return (ColorInfo.DELETE_FLAG == Constant.DeleteFlag.DELETE); }
            set { ColorInfo.DELETE_FLAG = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public UpdateColorModel()
        {
            ColorInfo = new Color();
        }
    }
}