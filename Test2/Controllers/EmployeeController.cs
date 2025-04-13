using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test2.Models;

namespace Test2.Controllers
{
    public class EmployeeController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            List<Employee> empList = new List<Employee>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM emp2";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    empList.Add(new Employee
                    {
                        Id = Convert.ToInt32(rdr["Id"]),
                        FirstName = rdr["FirstName"].ToString(),
                        MiddleName = rdr["MiddleName"]?.ToString(),
                        LastName = rdr["LastName"].ToString(),
                        DOB = Convert.ToDateTime(rdr["DOB"]),
                        MobileNumber = rdr["MobileNumber"].ToString(),
                        Address = rdr["Address"]?.ToString(),
                        Salary = Convert.ToDecimal(rdr["Salary"])
                    });
                }
            }

            return View(empList);
        }

        // Insert sample records
        public ActionResult InsertRecords()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO emp2 (FirstName, MiddleName, LastName, DOB, MobileNumber, Address, Salary) VALUES 
            ('Amit', NULL, 'Sharma', '1995-12-10', '9876543210', 'Pune', 45000),
            ('Neha', 'Raj', 'Verma', '2001-05-21', '9123456789', 'Mumbai', 50000),
            ('Ravi', NULL, 'Kumar', '1988-07-15', '9001234567', 'Delhi', 60000),
            ('Priya', 'M.', 'Joshi', '1999-02-05', '9012345678', 'Nashik', 55000),
            ('Karan', NULL, 'Mehta', '2002-11-30', '9023456781', 'Ahmedabad', 47000)
        ";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Total Salary
        public ActionResult TotalSalary()
        {
            decimal total = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT SUM(Salary) FROM emp2";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                var res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                {
                    total = (decimal)res;
                }
            }

            TempData["TotalSalary"] = total;
            return RedirectToAction("Index");
        }

        // DOB < 2000
        public ActionResult FilterByDOB()
        {
            var employees = new List<Employee>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM emp2 WHERE DOB < '2000-01-01'";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    employees.Add(new Employee
                    {
                        Id = Convert.ToInt32(rdr["Id"]),
                        FirstName = rdr["FirstName"].ToString(),
                        MiddleName = rdr["MiddleName"]?.ToString(),
                        LastName = rdr["LastName"].ToString(),
                        DOB = Convert.ToDateTime(rdr["DOB"]),
                        MobileNumber = rdr["MobileNumber"].ToString(),
                        Address = rdr["Address"]?.ToString(),
                        Salary = Convert.ToDecimal(rdr["Salary"])
                    });
                }
            }
            return View("Index", employees);
        }

        // Count MiddleName NULL
        public ActionResult CountNullMiddleName()
        {
            int count = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM emp2 WHERE MiddleName IS NULL";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                var res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                {
                    count = (int)res;
                }
            }

            TempData["NullMiddleNameCount"] = count;
            return RedirectToAction("Index");
        }

    }
}