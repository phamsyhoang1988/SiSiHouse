using SiSiHouse.Resources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiSiHouse.Models.Conditions
{
    public class BrandCondition
    {
        [DisplayName("Tên thương hiệu")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string BRAND_NAME { get; set; }

        [DisplayName("Bao gồm dữ liệu đã xóa")]
        public bool DELETE_FLAG { get; set; }
    }
}