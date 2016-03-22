using System.ComponentModel;

namespace SiSiHouse.Models.Conditions
{
    public class UserCondition {

        [DisplayName("Họ tên")]
        public string FULL_NAME { get; set; }

        [DisplayName("Điện thoại")]
        public string MOBILE { get; set; }

        [DisplayName("Email")]
        public string EMAIL { get; set; }

        [DisplayName("Trang cá nhân")]
        public string PRIVATE_PAGE { get; set; }

        [DisplayName("Bao gồm dữ liệu đã xóa")]
        public bool DELETE_FLAG { get; set; }

        public bool IS_SELECT_DIALOG { get; set; }

        public string CallBack { get; set; }
    }
}