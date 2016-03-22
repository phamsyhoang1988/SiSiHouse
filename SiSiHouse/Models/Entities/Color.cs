using SiSiHouse.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiSiHouse.Models.Entities
{
    public class Color
    {
        public int COLOR_ID { get; set; }

        [DisplayName("Tên màu")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string COLOR_NAME { get; set; }

        [DisplayName("Mã màu")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(20, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string COLOR_CODE { get; set; }

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
    }
}