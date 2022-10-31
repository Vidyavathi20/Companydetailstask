using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Loadproject.Models
{
    public class Employee
    {
        public string QueryType { get; set; }
        public string EmpCode { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string BandGrade { get; set; }
        public string DateOfBirth { get; set; }
        public string DateOfJoin { get; set; }
        public string PreExp { get; set; }
        public string TitanExp { get; set; }
        public string TotalExp { get; set; }
        public string ExpCat { get; set; }
        public string Report { get; set; }
        public string Designation { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string Email_id { get; set; }
        public string Photo { get; set; }
        public string PrePhoto { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public string Mobile_no { get; set; }

        public string Qualification { get; set; }

    }

    public class R_manager
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string ReportID { get; set; }
        public string Role_name { get; set; }
        public string EmpCode { get; set; }
        public string Photo { get; set; }
        public string Mobile_no { get; set; }

        public string Qulification { get; set; }
    }
}