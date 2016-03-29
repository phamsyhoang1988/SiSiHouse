using SiSiHouse.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiSiHouse.Models.Conditions
{
    public class CollectionCondition
    {
        public int? CATEGORY_TYPE { get; set; }

        public string CATEGORY_NAME { get; set; }

        public string SEARCH_VALUE { get; set; }
    }
}