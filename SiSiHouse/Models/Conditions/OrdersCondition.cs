using SiSiHouse.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiSiHouse.Models.Conditions
{
    public class OrdersCondition
    {
        public string ORDERS_TYPE_ID { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E004")]
        public string CUSTOMER_NAME { get; set; }

        public string CUSTOMER_MOBILE { get; set; }

        public DateTime? FROM { get; set; }

        public DateTime? TO { get; set; }

        [DisplayName("Bao gồm dữ liệu đã xóa")]
        public bool DELETE_FLAG { get; set; }
    }
}