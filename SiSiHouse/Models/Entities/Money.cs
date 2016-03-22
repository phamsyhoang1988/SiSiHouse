using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SiSiHouse.Resources;

namespace SiSiHouse.Models.Entities
{
    public class Money
    {
        public int peta_rn { get; set; }

        public int MONEY_ID { get; set; }

        [DisplayName("Loại tiền")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string MONEY_NAME { get; set; }

        [DisplayName("Ký hiệu")]
        public string MONEY_SIGN { get; set; }

        [DisplayName("Ngày áp dụng")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        public DateTime? APPLIED_DATE { get; set; }

        [DisplayName("Tỷ giá")]
        public decimal EXCHANGE_RATE { get; set; }

        [DisplayName("Phí trọng lượng")]
        public decimal WEIGHT_POSTAGE { get; set; }

        [DisplayName("Phí nhập hàng")]
        public int WAGE { get; set; }

        [DisplayName("Mô tả")]
        public string DESCRIPTION { get; set; }

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