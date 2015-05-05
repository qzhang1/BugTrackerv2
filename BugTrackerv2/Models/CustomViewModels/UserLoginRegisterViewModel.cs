using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerv2.Models.CustomViewModels
{
    public class UserLoginRegisterViewModel
    {
        public LoginViewModel login { get; set; }
        public RegisterViewModel register { get; set; }
        public string message { get; set; }
    }
}