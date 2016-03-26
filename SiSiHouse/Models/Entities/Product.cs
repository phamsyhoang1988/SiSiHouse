using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SiSiHouse.Resources;

namespace SiSiHouse.Models.Entities
{
    public class Product
    {
        public int peta_rn { get; set; }

        public long PRODUCT_ID { get; set; }

        [DisplayName("Mã sản phẩm")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string PRODUCT_CODE { get; set; }

        [DisplayName("Tên sản phẩm")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string PRODUCT_NAME { get; set; }

        [DisplayName("Thương hiệu")]
        public int BRAND_ID { get; set; }

        [DisplayName("Phân loại")]
        public int CATEGORY_ID { get; set; }

        [DisplayName("Trạng thái")]
        public int? STATUS_ID { get; set; }

        [DisplayName("Giới tính")]
        public int SEX { get; set; }

        [DisplayName("Giá nhập vào")]
        public decimal? IMPORT_PRICE { get; set; }

        [DisplayName("Nguồn hàng")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E004")]
        public int? MONEY_TYPE_ID { get; set; }

        public decimal? EXCHANGE_RATE { get; set; }

        public decimal? WEIGHT_POSTAGE { get; set; }

        public int? WAGE { get; set; }

        [DisplayName("Trọng lượng")]
        public decimal? WEIGHT { get; set; }

        [DisplayName("Ngày nhập kho")]
        public DateTime? IMPORT_DATE { get; set; }

        [DisplayName("Thực giá")]
        public decimal? REAL_PRICE { get; set; }

        [DisplayName("Giá bán ra")]
        public decimal? SALE_PRICE { get; set; }

        [DisplayName("Giá khuyến mãi")]
        public decimal? SALE_OFF_PRICE { get; set; }

        [DisplayName("Link gốc")]
        public string ROOT_LINK { get; set; }

        [DisplayName("Thành phần")]
        public string COMPOSITION { get; set; }

        [DisplayName("Mô tả")]
        public string DESCRIPTION { get; set; }

        public string CLIP_PATH { get; set; }

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

        public string BRAND_NAME { get; set; }

        public string CATEGORY_NAME { get; set; }

        public string MONEY_SIGN { get; set; }

        public string PICTURE { get; set; }

        public string PICTURE_1 { get; set; }

        public string PICTURE_2 { get; set; }

        public string COLOR_NAME { get; set; }

        public string SIZE { get; set; }

        public int QUANTITY { get; set; }

        public decimal SALES { get; set; }
    }
}