using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test1.Models;

namespace Test1.Controllers
{
    public class EmployeeController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            List<Employee> employeeList = new List<Employee>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM emp1";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    employeeList.Add(new Employee
                    {
                        Id = Convert.ToInt32(rdr["Id"]),
                        FirstName = rdr["FirstName"].ToString(),
                        MiddleName = rdr["MiddleName"].ToString(),
                        LastName = rdr["LastName"].ToString(),
                        DOB = Convert.ToDateTime(rdr["DOB"]),
                        MobileNumber = rdr["MobileNumber"].ToString(),
                        Address = rdr["Address"].ToString()
                    });
                }
            }

            return View(employeeList);
        }


        // 1. Insert one record
        public ActionResult InsertOne()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO emp1 (FirstName, MiddleName, LastName, DOB, MobileNumber, Address) " +
                               "VALUES (@FirstName, @MiddleName, @LastName, @DOB, @MobileNumber, @Address)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FirstName", "John");
                cmd.Parameters.AddWithValue("@MiddleName", "A");
                cmd.Parameters.AddWithValue("@LastName", "Doe");
                cmd.Parameters.AddWithValue("@DOB", DateTime.Parse("1990-01-01"));
                cmd.Parameters.AddWithValue("@MobileNumber", "1234567890");
                cmd.Parameters.AddWithValue("@Address", "Mumbai");

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // 2. Insert multiple test records
        public ActionResult InsertTestData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                for (int i = 1; i <= 3; i++)
                {
                    string query = "INSERT INTO emp1 (FirstName, MiddleName, LastName, DOB, MobileNumber, Address) " +
                                   "VALUES (@FirstName, @MiddleName, @LastName, @DOB, @MobileNumber, @Address)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@FirstName", $"Test{i}");
                    cmd.Parameters.AddWithValue("@MiddleName", $"M{i}");
                    cmd.Parameters.AddWithValue("@LastName", $"L{i}");
                    cmd.Parameters.AddWithValue("@DOB", DateTime.Now.AddYears(-20 - i));
                    cmd.Parameters.AddWithValue("@MobileNumber", $"99999999{i}");
                    cmd.Parameters.AddWithValue("@Address", $"City{i}");
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        // 3. Update FirstName of first record to SQLPerson
        public ActionResult UpdateFirstName()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE emp1 SET FirstName = 'SQLPerson' WHERE Id = 1";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // 4. Update all MiddleNames to "I"
        public ActionResult UpdateMiddleNames()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE emp1 SET MiddleName = 'I'";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // 5. Delete records where Id < 2
        public ActionResult DeleteById()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM emp1 WHERE Id < 2";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // 6. Delete all data
        public ActionResult DeleteAll()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "Truncate table emp1";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}