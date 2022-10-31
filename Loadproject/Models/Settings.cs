using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Loadproject.Models
{
    public class Roles
    {
        public string QueryType { get; set; }
        public string Id { get; set; }
        public string Role_name { get; set; }
        public string Status { get; set; }
    }

    public class Brand
    {
        public string QueryType { get; set; }
        public string Id { get; set; }
        public string Brand_name { get; set; }
        public string Status { get; set; }
    }

    public class Designation
    {
        public string QueryType { get; set; }
        public string Id { get; set; }
        public string Designation_name { get; set; }
        public string Status { get; set; }
    }

    public class Department
    {
        public string QueryType { get; set; }
        public string Id { get; set; }
        public string Dept_name { get; set; }
        public string Status { get; set; }
    }
}