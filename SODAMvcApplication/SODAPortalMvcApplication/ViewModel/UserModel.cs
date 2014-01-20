using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SODAPortalMvcApplication.ViewModel
{
    public class UserModel
    {
        [Required]
        public string FirtName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        public string Contact { get; set; }
        public string Company { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        //[Display(Name = "Confirm new password")]
        //[Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string CompanyUrl { get; set; }

        public string Country { get; set; }
    }

    public class changePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(16)]
        //[RegularExpression(@"^(?=[^\d_].*?\d)\w(\w|[!@#$%]){7,20}",ErrorMessage="Your password does not fit the requirements. Please try again")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z]).{7,20}$", ErrorMessage = "Your password does not fit the requirements. Please try again")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}