using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using SiSiHouse.Resources;

namespace SiSiHouse.ViewModels.ManageUser
{
    public class LoginViewModel
    {
        [DisplayName("Tài khoản")] 
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string ACCOUNT { get; set; }

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string PASSWORD { get; set; }
    }
}