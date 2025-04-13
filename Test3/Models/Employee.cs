using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test3.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public decimal Salary { get; set; }
        public int DesignationId { get; set; }
    }
}