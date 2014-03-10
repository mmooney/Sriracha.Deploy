using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Models
{
    public class SystemSetupData
    {
        [Required]
        [Display(Name = "Administrator User Name")]
        public string AdministratorUserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Administrator Email Address")]
        public string AdministratorEmailAddress { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Administrator Password")]
        public string AdministratorPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.Web.Mvc.Compare("AdministratorPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string AdministratorConfirmPassword { get; set; }
    }
}