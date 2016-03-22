using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SiSiHouse.Resources;

namespace SiSiHouse.ViewModels.PMS01001
{
    public class SetNewPasswordViewModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        [DisplayName("新パスワード")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(32, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        [RegularExpression(@"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string NewPassword { get; set; }

        [DisplayName("新パスワード(確認用)")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E002")]
        [StringLength(32, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E008")]
        [RegularExpression(@"^[a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "E003")]
        public string NewPasswordConfirm { get; set; }

        public string PasswordLockTarget { get; set; }
    }
}