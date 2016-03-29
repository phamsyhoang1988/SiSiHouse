
namespace SiSiHouse.Models.Conditions
{
    public class BillCondition
    {
        public string RETAIL_CODE { get; set; }

        public long PRODUCT_ID { get; set; }

        public long PRODUCT_DETAIL_ID { get; set; }

        public int QUANTITY { get; set; }

        public int? STATUS_ID { get; set; }

        public bool IS_UNDO { get; set; }
    }
}