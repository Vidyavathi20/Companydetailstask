using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using ClosedXML.Excel;

namespace Loadproject.Controllers
{
    public class EmployeeController : Controller
    {
        private string connectionstring = ConfigurationManager.ConnectionStrings["con"].ToString();
        private string connectionstring1 = ConfigurationManager.ConnectionStrings["con1"].ToString();

        // GET: Employee
        public ActionResult Index()
        {
            if (Session["UserName"] != null)
            {
                IEnumerable<Models.Employee> data = Employee_data();
                return View(data);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        private IEnumerable<Models.Employee> Employee_data()
        {
            using (SqlConnection con = new SqlConnection(connectionstring1))
            {
                con.Open();

                DataSet ds = new DataSet();

                SqlCommand cmd = new SqlCommand("select a.*,b.Dept_name,c.Name 'Report' from emp_details as a left join tbl_department as b on a.Department=b.Dept_id"
                                                + " left join tb_managerHOD as c on a.team_lead = c.Emp_code", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Models.Employee
                    {
                        EmpCode = Convert.ToString(row["EmpCode"]),
                        Name = Convert.ToString(row["Name"]),
                        BandGrade = Convert.ToString(row["Level"]),
                        Gender = Convert.ToString(row["Gender"]),
                        DateOfBirth = Convert.ToString(row["dob"]),
                        DateOfJoin = Convert.ToString(row["DateOfJoin"]),
                        PreExp = Convert.ToString(row["Outside_exp"]),
                        TitanExp = Convert.ToString(row["Titan_exp"]),
                        TotalExp = Convert.ToString(row["Total_exp"]),
                        ExpCat = Convert.ToString(row["ExpCat"]),
                        Report = Convert.ToString(row["Report"]),
                        //Designation = Convert.ToString(row["Designation_name"]),
                        Department = Convert.ToString(row["Dept_name"]),
                        Role = Convert.ToString(row["role"]),
                        Mobile_no = Convert.ToString(row["Mobile_no"]),
                        Qualification = Convert.ToString(row["Qualification"]),
                        Status = Convert.ToString(row["emp_status"]),                      
                    };
                }
            }
        }

        [HttpPost]
        public ActionResult Insert_employee(Models.Employee S, HttpPostedFileBase Photo)
        {
            if (Session["UserName"] != null)
            {
                try
                {
                    string filename = string.Empty;
                    string filePath = string.Empty;

                    using (SqlConnection con = new SqlConnection(connectionstring))
                    {
                        string ID = "";
                        if (S.QueryType == "Insert")
                        {
                            ID = "0";

                            if (Photo != null)
                            {
                                string path = Server.MapPath("~/image/");
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                filePath = path + Path.GetFileName(Photo.FileName);
                                filename = Path.GetFileName(Photo.FileName);
                                string extension = Path.GetExtension(Photo.FileName);
                                Photo.SaveAs(filePath);
                            }
                        }
                        else
                        {
                            ID = S.Id;
                            filename = S.PrePhoto;
                        }

                        string response = string.Empty;
                        con.Open();
                        SqlCommand cmd = new SqlCommand("SP_employee", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = S.QueryType;
                        cmd.Parameters.Add("@Id", SqlDbType.VarChar, 50).Value = S.Id;
                        cmd.Parameters.Add("@EmpCode", SqlDbType.VarChar, 50).Value = S.EmpCode;
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = S.Name;
                        cmd.Parameters.Add("@BrandGrade", SqlDbType.VarChar, 50).Value = S.BandGrade;
                        cmd.Parameters.Add("@DateOfBirth", SqlDbType.VarChar, 50).Value = S.DateOfBirth;
                        cmd.Parameters.Add("@DateOfJoin", SqlDbType.VarChar, 50).Value = S.DateOfJoin;
                        cmd.Parameters.Add("@PreExp", SqlDbType.VarChar, 50).Value = S.PreExp;
                        cmd.Parameters.Add("@TitanExp", SqlDbType.VarChar, 50).Value = S.TitanExp;
                        cmd.Parameters.Add("@TotalExp", SqlDbType.VarChar, 50).Value = S.TotalExp;
                        cmd.Parameters.Add("@ExpCat", SqlDbType.VarChar, 50).Value = S.ExpCat;
                        cmd.Parameters.Add("@Report", SqlDbType.VarChar, 50).Value = S.Report;
                        cmd.Parameters.Add("@Designation", SqlDbType.VarChar, 50).Value = S.Designation;
                        cmd.Parameters.Add("@Role", SqlDbType.VarChar, 50).Value = S.Role;
                        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = S.Status;
                        cmd.Parameters.Add("@Created_by", SqlDbType.VarChar, 50).Value = Session["UserName"];
                        cmd.Parameters.Add("@Photo", SqlDbType.VarChar, 50).Value = filename;
                        cmd.Parameters.Add("@Email_id", SqlDbType.VarChar, 50).Value = S.Email_id;
                        cmd.Parameters.Add("@Department", SqlDbType.VarChar, 50).Value = S.Department;
                        cmd.Parameters.Add("@Gender", SqlDbType.VarChar, 50).Value = S.Gender;
                        cmd.Parameters.Add("@Mobile_No", SqlDbType.VarChar, 50).Value = S.Mobile_no;
                        cmd.Parameters.Add("@Qualification", SqlDbType.VarChar, 50).Value = S.Qualification;                   

                        SqlParameter SQLReturn = new SqlParameter("@SQLReturn", SqlDbType.NVarChar, 50);
                        SQLReturn.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(SQLReturn);
                        cmd.ExecuteNonQuery();
                        response = SQLReturn.Value.ToString().Trim();

                        return RedirectToAction("Dashboard", "Employee", new { ac = response });
                    }


                }


                catch (Exception ex)

                {
                    ViewBag.Message = "Failed to Add Employee Details ";
                    return View("Index");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Edit_emp(string id)
        {
            if (Session["UserName"] != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionstring))
                    {
                        con.Open();
                        DataSet ds = new DataSet();
                        SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Getall_employee";
                        cmd.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = id;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                        var result = new List<Models.Employee>();
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            result.Add(item: new Models.Employee
                            {
                                Id = Convert.ToString(row["Id"]),
                                EmpCode = Convert.ToString(row["EmpCode"]),
                                Name = Convert.ToString(row["Name"]),
                                BandGrade = Convert.ToString(row["BandGrade"]),
                                DateOfBirth = Convert.ToString(row["DateOfBirth"]),
                                DateOfJoin = Convert.ToString(row["DateOfJoin"]),
                                PreExp = Convert.ToString(row["PreExp"]),
                                TitanExp = Convert.ToString(row["TitanExp"]),
                                TotalExp = Convert.ToString(row["TotalExp"]),
                                ExpCat = Convert.ToString(row["ExpCat"]),
                                Report = Convert.ToString(row["Report"]),
                                Designation = Convert.ToString(row["Designation"]),
                                Role = Convert.ToString(row["Role"])
                            });
                        }

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Failed Edit Details ";
                    return View("Index");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Delete_emp(string ID)
        {
            if (Session["UserName"] != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionstring))
                    {
                        con.Open();
                        DataSet dt = new DataSet();
                        SqlCommand cmd1 = new SqlCommand("SP_delete_settings", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Delete_Employee";
                        cmd1.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = ID;
                        cmd1.ExecuteNonQuery();
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                }


                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Delete Employee";
                    return View("Index");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }


        }

        public ActionResult Getrole(Models.Roles s)
        {
            using (SqlConnection con = new SqlConnection(connectionstring1))
            {
                con.Open();
                DataSet ds = new DataSet();

                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "all_roles";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = new List<Models.Roles>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.Roles
                    {
                        Id = Convert.ToString(row["RoleID"]),
                        Role_name = Convert.ToString(row["RoleName"])
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

       public ActionResult Getdeptwise_data(string dept)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Getdeptwise_data";
                cmd.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = dept;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = new List<Models.Employee>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.Employee
                    {
                        EmpCode = Convert.ToString(row["EmpCode"]),
                        Name = Convert.ToString(row["Name"]),
                        BandGrade = Convert.ToString(row["Brand_name"]),
                        Gender = Convert.ToString(row["Gender"]),
                        DateOfBirth = Convert.ToString(row["DateOfBirth"]),
                        DateOfJoin = Convert.ToString(row["DateOfJoin"]),
                        PreExp = Convert.ToString(row["PreExp"]),
                        TitanExp = Convert.ToString(row["TitanExp"]),
                        TotalExp = Convert.ToString(row["TotalExp"]),
                        ExpCat = Convert.ToString(row["ExpCat"]),
                        Report = Convert.ToString(row["Report"]),
                        Designation = Convert.ToString(row["Designation_name"]),
                        Department = Convert.ToString(row["Dept_name"]),
                        Role = Convert.ToString(row["Role_name"]),
                        Status = Convert.ToString(row["Status"]),
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Getdesignation(Models.Designation s)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                DataSet ds = new DataSet();

                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "all_designation";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = new List<Models.Designation>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.Designation
                    {
                        Id = Convert.ToString(row["Id"]),
                        Designation_name = Convert.ToString(row["Designation_name"])
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Getbrand(Models.Roles s)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                DataSet ds = new DataSet();

                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "all_brand";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = new List<Models.Brand>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.Brand
                    {
                        Id = Convert.ToString(row["Id"]),
                        Brand_name = Convert.ToString(row["Brand_name"])
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Getdept(Models.Department s)
        {
            using (SqlConnection con = new SqlConnection(connectionstring1))
            {
                con.Open();
                DataSet ds = new DataSet();

                SqlCommand cmd = new SqlCommand("select * from tbl_dept", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = new List<Models.Department>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.Department
                    {
                        Id = Convert.ToString(row["Dept_id"]),
                        Dept_name = Convert.ToString(row["Dept_code"])
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Organ_structure()
        {
            return View();
        }

        [HttpPost]
        public ActionResult get_org_structure(Models.Department S)
        {
            using (SqlConnection con = new SqlConnection(connectionstring1))
            {
                con.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("select * from emp_details where Department=@para  AND team_lead !='2' order by team_lead asc ", con);
                cmd.Parameters.Add("@para", SqlDbType.VarChar, 50).Value = S.Dept_name;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = new List<Models.R_manager>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.R_manager
                    {
                        Id = Convert.ToString(row["Emp_code"]),
                        FirstName = Convert.ToString(row["Name"]),
                        Designation = Convert.ToString(row["role"]),
                        Email = Convert.ToString(row["email_id"]),
                        ReportID = Convert.ToString(row["team_lead"]),
                        Role_name = Convert.ToString(row["role"]),
                        EmpCode = Convert.ToString(row["Emp_code"]),
                        Photo = Convert.ToString(row["Image"]),
                        Mobile_no = Convert.ToString(row["Mobile_no"])
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        //public ActionResult Get_report_employee()
        //{
        //    using (SqlConnection con = new SqlConnection(connectionstring))
        //    {
        //        con.Open();
        //        DataSet ds = new DataSet();
        //        SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Getall_employee";
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(ds);
        //        var result = new List<Models.R_manager>();
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            result.Add(item: new Models.R_manager
        //            {
        //                Name = Convert.ToString(row["Name"]),
        //                EmpCode = Convert.ToString(row["EmpCode"]),
        //                Report = Convert.ToString(row["Report"]),
        //            });
        //        }
        //        return Json(result, JsonRequestBehavior.AllowGet);

        //    }
        //}
    }
}