using SiSiHouse.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiSiHouse.Models.Conditions
{
    public class StatisticsCondition
    {
        [DisplayName("Mã sản phẩm")]
        public string PRODUCT_CODE { get; set; }

        [DisplayName("Tên sản phẩm")]
        public string PRODUCT_NAME { get; set; }

        [DisplayName("Giới tính")]
        public string SEX { get; set; }

        [DisplayName("Thương hiệu")]
        public string BRAND_ID { get; set; }

        [DisplayName("Phân loại")]
        public string CATEGORY_ID { get; set; }

        [DisplayName("Thống kê theo năm")]
        public int TARGET_YEAR { get; set; }

        public int TARGET_MONTH { get; set; }

        [DisplayName("Ngày tạo hóa đơn")]
        public DateTime? TARGET_DATE { get; set; }
    }
}