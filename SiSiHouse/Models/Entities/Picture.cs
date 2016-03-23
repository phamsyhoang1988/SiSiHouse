using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SiSiHouse.Resources;
using SiSiHouse.Common;

namespace SiSiHouse.Models.Entities
{
    public class Picture
    {
        public long PRODUCT_ID { get; set; }

        public long PICTURE_ID { get; set; }

        public string FILE_PATH { get; set; }

        public string DISPLAY_FLAG { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public long CREATED_USER_ID { get; set; }

        public bool? DELETED { get; set; }

        public bool IS_MAIN
        {
            get { return (this.DISPLAY_FLAG == Constant.DisplayPicture.MAIN); }
            set { this.DISPLAY_FLAG = (value ? Constant.DisplayPicture.MAIN : Constant.DisplayPicture.EXTRA); }
        }
    }
}