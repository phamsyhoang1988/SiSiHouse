﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SiSiHouse.Resources;

namespace SiSiHouse.ViewModels.PMS01001
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessageResourceName = "W1000", ErrorMessageResourceType = typeof(Messages))]
        [DisplayName("Email")]
        public string Email { get; set; }
    }
}