using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SiSiHouse.Resources;

namespace SiSiHouse.Models.Entities
{
    public class ProductDetail
    {
        public long PRODUCT_ID { get; set; }

        public long PRODUCT_DETAIL_ID { get; set; }

        public int? COLOR_ID { get; set; }

        [DisplayName("Size")]
        [StringLength(10, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string SIZE { get; set; }

        [DisplayName("Số lượng")]
        public int? QUANTITY { get; set; }

        public bool? DELETED { get; set; }

        public string COLOR_NAME { get; set; }
    }
}