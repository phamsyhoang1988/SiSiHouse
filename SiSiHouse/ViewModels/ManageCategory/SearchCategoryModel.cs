﻿using SiSiHouse.Models.Conditions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace SiSiHouse.ViewModels.ManageCategory
{
    public class SearchCategoryModel
    {
        public CategoryCondition Condition { get; set; }

        public SearchCategoryModel()
        {
            Condition = new CategoryCondition();
        }
    }
}