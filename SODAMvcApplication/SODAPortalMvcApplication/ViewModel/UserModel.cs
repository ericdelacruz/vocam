using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAPortalMvcApplication.ViewModel
{
    public class UserModel
    {
        string FirtName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Contact { get; set; }
        string Company { get; set; }
        string Password { get; set; }
        string ConfirmPassword { get; set; }
    }
}