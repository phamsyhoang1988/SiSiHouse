using SiSiHouse.Common;
using SiSiHouse.Models.Entities;
using System.ComponentModel;

namespace SiSiHouse.ViewModels.ManageUser
{
    public class UpdateUserModel
    {
        public User UserInfo { get; set; }

        [DisplayName("Xóa")]
        public bool DELETE_FLAG
        {
            get { return (UserInfo.DELETE_FLAG == Constant.DeleteFlag.DELETE); }
            set { UserInfo.DELETE_FLAG = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public UpdateUserModel()
        {
            UserInfo = new User();
        }
    }
}