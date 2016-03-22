using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SiSiHouse.Resources;

namespace SiSiHouse.Models.Entities
{
    public class User
    {
        public int peta_rn { get; set; }

        public long USER_ID { get; set; }

        [DisplayName("Tài khoản")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string ACCOUNT { get; set; }

        [DisplayName("Mật khẩu")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string PASSWORD { get; set; }

        [DisplayName("Họ tên")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string FULL_NAME { get; set; }

        [DisplayName("Quyền")]
        public int ROLE_ID { get; set; }

        [DisplayName("Số ĐT")]
        public string MOBILE { get; set; }

        [DisplayName("Email")]
        public string EMAIL { get; set; }

        [DisplayName("Trang cá nhân")]
        public string PRIVATE_PAGE { get; set; }

        [DisplayName("Địa chỉ")]
        public string ADDRESS { get; set; }

        public string DELETE_FLAG { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? CREATED_DATE { get; set; }

        public long CREATED_USER_ID { get; set; }

        [DisplayName("Ngày cập nhật")]
        public DateTime? MODIFIED_DATE { get; set; }

        public long MODIFIED_USER_ID { get; set; }

        [DisplayName("Người tạo")]
        public string CREATED_USER { get; set; }

        [DisplayName("Người cập nhật")]
        public string MODIFIED_USER { get; set; }

        public string OLD_PASSWORD { get; set; }
    }
}