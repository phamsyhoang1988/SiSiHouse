using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SiSiHouse.Resources;

namespace SiSiHouse.Models.Entities
{
    public class Artwork
    {
        public long PRODUCT_ID { get; set; }

        public long ARTWORK_ID { get; set; }

        public string FILE_PATH { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public long CREATED_USER_ID { get; set; }

        public string FILE_PATH_OLD { get; set; }

        public bool? DELETED { get; set; }

        public bool? CHANGED { get; set; }
    }
}