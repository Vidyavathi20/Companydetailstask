using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Loadproject.Models
{
    public class Login
    {
        public string id { get; set; }
        public string Admin { get; set; }
        public string Password { get; set; }        
        public string QueryType { get; set; }
        public string Oldpassword { get; set; }

    }


    public class Emp_dashboard
    {
        public string male_count { get; set; }
        public string female_count { get; set; }
        public string on_count { get; set; }
        public string off_count { get; set; }
        public string financial_year { get; set; }
        public string Department { get; set; }
    }

    public class over_all
    {
        public string title_name { get; set; }
        public string total_count { get; set; }
        public string male_count { get; set; }
        public string female_count { get; set; }
    }
}