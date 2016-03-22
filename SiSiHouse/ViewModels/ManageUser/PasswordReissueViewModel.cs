using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SiSiHouse.Resources;

namespace SiSiHouse.ViewModels.PMS01001
{
    public class PasswordReissueViewModel
    {
        [DisplayName("メールアドレス")] 
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
        ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        public string Email { get; set; }
    }
}