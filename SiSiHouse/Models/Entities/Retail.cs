using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SiSiHouse.Resources;

namespace SiSiHouse.Models.Entities
{
    public class Retail
    {
        public long RETAIL_CODE { get; set; }

        public long PRODUCT_ID { get; set; }

        public long PRODUCT_DETAIL_ID { get; set; }

        public int COLOR_ID { get; set; }

        public string SIZE { get; set; }

        public int QUANTITY { get; set; }

        public decimal TOTAL_PRICE { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public long CREATED_USER_ID { get; set; }

        public DateTime? MODIFIED_DATE { get; set; }

        public long MODIFIED_USER_ID { get; set; }

        public string CREATED_USER { get; set; }

        public string MODIFIED_USER { get; set; }
    }
}