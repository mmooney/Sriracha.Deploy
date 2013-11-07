using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Sriracha.Deploy.Data.Account
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string CookieName { get; set; }
        public string CookieValue { get; set; }
        public string CookiPath { get; set; }
        public string CookieDomain { get; set; }

        public string ErrorMessage { get; set; }
    }
}
