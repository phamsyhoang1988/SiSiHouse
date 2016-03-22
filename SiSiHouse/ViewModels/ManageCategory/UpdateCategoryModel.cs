using SiSiHouse.Common;
using SiSiHouse.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace SiSiHouse.ViewModels.ManageCategory
{
    public class UpdateCategoryModel
    {
        public Category CategoryInfo { get; set; }

        [DisplayName("Phân loại chính")]
        public IList<SelectListItem> ParentCategorySelectList { get; set; }

        [DisplayName("Xóa")]
        public bool DELETE_FLAG
        {
            get { return (CategoryInfo.DELETE_FLAG == Constant.DeleteFlag.DELETE); }
            set { CategoryInfo.DELETE_FLAG = (value ? Constant.DeleteFlag.DELETE : Constant.DeleteFlag.NON_DELETE); }
        }

        public UpdateCategoryModel()
        {
            CategoryInfo = new Category();
        }
    }
}