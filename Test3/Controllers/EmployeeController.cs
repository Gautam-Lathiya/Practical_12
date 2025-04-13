using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test3.Models;

namespace Test3.Controllers
{
    public class EmployeeController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InsertDesignations()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Designation (Designation)
                         VALUES ('Manager'), ('Developer'), ('Analyst')";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Index");
        }

        public ActionResult InsertEmployees()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO emp3 (FirstName, MiddleName, LastName, DOB, MobileNumber, Address, Salary, DesignationId)
            VALUES 
            ('Amit', NULL, 'Shah', '1988-05-14', '9876543210', 'Mumbai', 60000, 1),
            ('Rita', 'K.', 'Mehra', '1999-11-25', '7894561230', 'Pune', 50000, 2),
            ('John', NULL, 'Doe', '1975-03-10', '9988776655', 'Delhi', 45000, 2),
            ('Sara', 'M.', 'Ali', '2001-06-30', '9911223344', 'Chennai', 55000, 3),
            ('Tom', NULL, 'Hardy', '2003-09-10', '9090909090', 'Hyderabad', 70000, 1);";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("Index");
        }

        public ActionResult CountEmpByDesignation()
        {
            List<DesignationEmpCount> list = new List<DesignationEmpCount>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT d.Designation, COUNT(e.Id) AS EmployeeCount
                         FROM emp3 e
                         JOIN Designation d ON e.DesignationId = d.Id
                         GROUP BY d.Designation";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new DesignationEmpCount
                    {
                        Designation = reader["Designation"].ToString(),
                        EmployeeCount = Convert.ToInt32(reader["EmployeeCount"])
                    });
                }

                con.Close();
            }

            return View(list);
        }

        public ActionResult DisplayWithDesignation()
        {
            List<EmployeeWithDesignation> list = new List<EmployeeWithDesignation>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT e.FirstName, e.MiddleName, e.LastName, d.Designation
                         FROM emp3 e
                         JOIN Designation d ON e.DesignationId = d.Id";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new EmployeeWithDesignation
                    {
                        FirstName = reader["FirstName"].ToString(),
                        MiddleName = reader["MiddleName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Designation = reader["Designation"].ToString()
                    });
                }

                con.Close();
            }

            return View(list);
        }

        public ActionResult DisplayFromView()
        {
            List<EmployeeDetailsView> list = new List<EmployeeDetailsView>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM vw_EmployeeDetails";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new EmployeeDetailsView
                    {
                        EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                        FirstName = reader["FirstName"].ToString(),
                        MiddleName = reader["MiddleName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Designation = reader["Designation"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        MobileNumber = reader["MobileNumber"].ToString(),
                        Address = reader["Address"].ToString(),
                        Salary = Convert.ToDecimal(reader["Salary"])
                    });
                }

                con.Close();
            }

            return View(list);
        }

        [HttpGet]
        public ActionResult SP_AddDesignation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SP_AddDesignation(Designation model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertDesignation", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Designation", model.DesignationName);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                ViewBag.Message = "Designation added successfully via stored procedure!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult SP_AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SP_AddEmployee(Employee model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertEmployee", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@MiddleName", (object)model.MiddleName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@DOB", model.DOB);
                    cmd.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    cmd.Parameters.AddWithValue("@Salary", model.Salary);
                    cmd.Parameters.AddWithValue("@DesignationId", model.DesignationId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                ViewBag.Message = "Employee inserted successfully via stored procedure.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult DesignationsWithMultipleEmployees()
        {
            List<string> designations = new List<string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT d.Designation
            FROM Designation d
            JOIN emp3 e ON d.Id = e.DesignationId
            GROUP BY d.Designation
            HAVING COUNT(e.Id) > 1";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    designations.Add(reader["Designation"].ToString());
                }

                con.Close();
            }

            return View(designations);
        }

        public ActionResult SP_GetEmployeesOrderedByDOB()
        {
            List<EmployeeDetailsView> list = new List<EmployeeDetailsView>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllEmployeesOrderedByDOB", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new EmployeeDetailsView
                    {
                        EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                        FirstName = reader["FirstName"].ToString(),
                        MiddleName = reader["MiddleName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Designation = reader["Designation"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        MobileNumber = reader["MobileNumber"].ToString(),
                        Address = reader["Address"].ToString(),
                        Salary = Convert.ToDecimal(reader["Salary"])
                    });
                }

                con.Close();
            }

            return View(list);
        }

        [HttpGet]
        public ActionResult SP_GetEmployeesByDesignation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SP_GetEmployeesByDesignation(int designationId)
        {
            List<EmployeeBasicDetail> employees = new List<EmployeeBasicDetail>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetEmployeesByDesignation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DesignationId", designationId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(new EmployeeBasicDetail
                    {
                        EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                        FirstName = reader["FirstName"].ToString(),
                        MiddleName = reader["MiddleName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        MobileNumber = reader["MobileNumber"].ToString(),
                        Address = reader["Address"].ToString(),
                        Salary = Convert.ToDecimal(reader["Salary"])
                    });
                }

                con.Close();
            }

            return View(employees);
        }

        public ActionResult GetEmployeeWithMaxSalary()
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            WITH RankedSalaries AS (
                SELECT *, DENSE_RANK() OVER (ORDER BY Salary DESC) AS SalaryRank
                FROM emp3
            )
            SELECT *
            FROM RankedSalaries
            WHERE SalaryRank = 1;";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(new Employee
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FirstName = reader["FirstName"].ToString(),
                        MiddleName = reader["MiddleName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        MobileNumber = reader["MobileNumber"].ToString(),
                        Address = reader["Address"].ToString(),
                        Salary = Convert.ToDecimal(reader["Salary"]),
                        DesignationId = Convert.ToInt32(reader["DesignationId"])
                    });
                }

                con.Close();
            }

            return View(employees);
        }


    }
}