﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SiSiHouse.Resources;

namespace SiSiHouse.Models.Entities
{
    public class Brand
    {
        public int peta_rn { get; set; }

        public int BRAND_ID { get; set; }

        [DisplayName("Tên thương hiệu")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string BRAND_NAME { get; set; }

        [DisplayName("Mô tả")]
        public string DESCRIPTION { get; set; }

        public string DELETE_FLAG { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? CREATED_DATE { get; set; }

        public long CREATED_USER_ID { get; set; }

        [DisplayName("Ngày cập nhật")]
        public DateTime? MODIFIED_DATE { get; set; }

        public long MODIFIED_USER_ID { get; set; }

        [DisplayName("Người tạo")]
        public string CREATED_USER { get; set; }

        [DisplayName("Người cập nhật")]
        public string MODIFIED_USER { get; set; }

        public bool CHECK { get; set; }
    }
}