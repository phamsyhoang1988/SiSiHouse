using SiSiHouse.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiSiHouse.Models.Conditions
{
    public class ProductCondition
    {
        [DisplayName("Mã sản phẩm")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E004")]
        public string PRODUCT_CODE { get; set; }

        [DisplayName("Tên sản phẩm")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E004")]
        public string PRODUCT_NAME { get; set; }

        [DisplayName("Thương hiệu")]
        public string BRAND_ID { get; set; }

        [DisplayName("Phân loại")]
        public string CATEGORY_ID { get; set; }

        [DisplayName("Trạng thái")]
        public string STATUS_ID { get; set; }

        [DisplayName("Giới tính")]
        public string SEX { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FROM { get; set; }

        [DataType(DataType.Date)]
        public DateTime? TO { get; set; }

        [DisplayName("Bao gồm dữ liệu đã xóa")]
        public bool DELETE_FLAG { get; set; }

        public bool IS_SELECT_DIALOG { get; set; }
    }
}