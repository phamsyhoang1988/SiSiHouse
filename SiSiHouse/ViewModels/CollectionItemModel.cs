using SiSiHouse.Common;
using SiSiHouse.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace SiSiHouse.ViewModels
{
    public class CollectionItemModel
    {
        public Product ProductInfo { get; set; }

        public IList<Picture> PictureList { get; set; }

        public CollectionItemModel()
        {
            ProductInfo = new Product();
            PictureList = new List<Picture>();
        }
    }
}