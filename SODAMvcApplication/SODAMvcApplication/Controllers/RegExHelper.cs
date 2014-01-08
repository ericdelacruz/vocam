using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SODAMvcApplication.Controllers
{
    public static class RegExHelper
    {
        public static bool IsValidEmail(string strIn)
        {
            return Regex.IsMatch(strIn, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
        }
        public static bool IsValidPhone(string strIn)
        {
           // return Regex.IsMatch(strIn, @"(\([2-9]\d\d\)|[2-9]\d\d) ?[-.,]? ?[2-9]\d\d ?[-.,]? ?\d{4}");

            return Regex.IsMatch(strIn, @"[^a-zA-Z]+$") && strIn.Length >= 7;
        }
        public static bool isValidPostal(string strIn)
        {
            return Regex.IsMatch(strIn, @"[^a-zA-Z]+$");
        }
    }
}