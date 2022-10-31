using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Net.Mail;

namespace Loadproject.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private string connectionstring = ConfigurationManager.ConnectionStrings["con"].ToString();
        private string connectionstring1 = ConfigurationManager.ConnectionStrings["con1"].ToString();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }       

        [HttpPost]
        public ActionResult Check_login(Models.Login Lo)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Check_login";
                cmd.Parameters.Add("@Parameter", SqlDbType.NVarChar, 150).Value = Lo.Admin;
                cmd.Parameters.Add("@Parameter1", SqlDbType.NVarChar, 150).Value = Lo.Password;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    Session["UserID"] = ds.Tables[0].Rows[0]["Id"].ToString();
                    Session["UserName"] = ds.Tables[0].Rows[0]["Username"].ToString();
                }
                else
                {
                    ViewBag.Message = "Invalid Login Details...!";
                    return View("Index");
                }
                return RedirectToAction("Dashboard", "Login");
            }

        }      

        public ActionResult Forgot()
        {            
            return View();
        }

        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public string RandomPassword(int size = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(2, true));
            builder.Append(RandomNumber(10, 99));
            builder.Append(RandomString(2, true));
            builder.Append(RandomNumber(10, 99));
            return builder.ToString();
        }


        [HttpPost]
        public ActionResult Send_mail(Models.Employee lb)
        {
            string message = "";
            string ran_pass = RandomPassword(6);
            DataTable dt1 = new DataTable();

            using (SqlConnection con1 = new SqlConnection(connectionstring))
            {
                con1.Open();
                SqlCommand cmd1 = new SqlCommand("Select * from tbl_gmail_settings", con1);
                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                sda1.Fill(dt1);
            }

            using (SqlConnection con2 = new SqlConnection(connectionstring))
            {
                con2.Open();
                SqlCommand cmd2 = new SqlCommand("Update tbl_Employee SET Password=@Password WHERE Emailid=@Emailid", con2);
                cmd2.Parameters.AddWithValue("@Emailid", lb.Email_id);
                cmd2.Parameters.AddWithValue("@Password", lb.Password);
                cmd2.ExecuteNonQuery();
            }

            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Emailid,Password from tbl_Employee where Emailid=@Emailid ", con);
                cmd.Parameters.AddWithValue("@Emailid", lb.Email_id);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    string password = dt.Rows[0]["Password"].ToString();
                    message = dt.Rows[0]["Emailid"].ToString();
                    MailMessage mail = new MailMessage();
                    mail.To.Add(message);
                    mail.From = new MailAddress(dt1.Rows[0]["Smtp_user"].ToString());
                    mail.Subject = "Forgot Password Details ";
                    mail.Body = string.Format("<b> Your New Password is :  </b> {0} ", password);
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = dt1.Rows[0]["Smtp_host"].ToString();
                    smtp.Port = Convert.ToInt32(dt1.Rows[0]["Smtp_port"].ToString());
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(dt1.Rows[0]["Smtp_user"].ToString(), dt1.Rows[0]["Smtp_pass"].ToString()); // Enter seders User name and password  
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    ViewBag.Message = "Password is Sent to your email id .Please Login!";
                    return View("Forgot");
                }
                else
                {
                    ViewBag.Message = "Email ID Not Valid...!";
                    return View("Forgot");
                }
            }

        }

        [HttpPost]
        public ActionResult CheckPwd(Models.Login User)
        {
            if (Session["UserName"] != null)
            {
                try
                {
                    string oldpassword = User.Oldpassword;
                    string password = User.Password;
                    string message = string.Empty;
                    string UserID = Session["UserID"].ToString();

                    using (SqlConnection con = new SqlConnection(connectionstring))                   {

                        con.Open();
                        DataSet ds = new DataSet();
                        SqlCommand cmd = new SqlCommand("select Password from tbl_Login where Id=" + UserID, con);
                        SqlDataReader sdr = cmd.ExecuteReader();
                        while (sdr.Read())
                        {
                            Session["pass"] = sdr["Password"].ToString();
                        }
                    }

                    if (oldpassword == Session["pass"].ToString())
                        using (SqlConnection con = new SqlConnection(connectionstring))
                        {
                            {
                                con.Open();
                                SqlCommand cmd = new SqlCommand("update tbl_Login set Password=@Password where Id=" + UserID, con);

                                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 150).Value = password;
                                cmd.ExecuteNonQuery();
                                message = "Password Changed Successfully...!";
                                ViewBag.SMessage = message;
                                return View("Change_Password");
                            }

                        }
                    else
                    {
                        message = "Old Password is Incorret...!";
                        ViewBag.Message = message;
                        return View("Change_Password");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Change Password";
                    return View("Change_Password");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


        public ActionResult Dashboard()
        {
            if (Session["UserName"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


        [HttpGet]
        public ActionResult Change_Password()
        {
            if (Session["UserName"] != null)
            {
                try
                {
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Load...!";
                    return View("Change_Password");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        public ActionResult Logout()
        {
            try
            {
                Session["UserName"] = "";                
                Session["UserID"] = "";
                ViewBag.LMessage = "Logout Successfully...!";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Get_dashboard_Count(Models.Emp_dashboard E)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Get_dashboard_emp_data";
                cmd.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = E.Department;
                cmd.Parameters.Add("@Parameter1", SqlDbType.VarChar, 50).Value = E.financial_year;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = new List<Models.Emp_dashboard>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.Emp_dashboard
                    {
                        male_count = Convert.ToString(row["male_count"]),
                        female_count = Convert.ToString(row["female_count"]),
                        on_count = Convert.ToString(row["on_count"]),
                        off_count = Convert.ToString(row["off_count"]),
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Get_dashboard_deptwise_data(Models.Emp_dashboard E)
        {
            using (SqlConnection con = new SqlConnection(connectionstring1))
            {
                con.Open();
                DataSet ds = new DataSet();

                if(E.Department == "All" && E.financial_year == "-")
                {
                    SqlCommand cmd = new SqlCommand("select a.*,b.Dept_name,c.Name 'Report' from emp_details as a left join tbl_department as b on a.Department=b.Dept_id"
                                                + " left join tb_managerHOD as c on a.team_lead = c.Emp_code  ", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
                else if(E.Department == "All" && E.financial_year != "")
                {
                    SqlCommand cmd = new SqlCommand("select a.*,b.Dept_name,c.Name 'Report' from emp_details as a left join tbl_department as b on a.Department=b.Dept_id"
                                                + " left join tb_managerHOD as c on a.team_lead = c.Emp_code where a.dob LIKE '%'+@Parameter1+'%' OR a.DateOfJoin LIKE '%'+@Parameter1+'%' ", con);
                    cmd.Parameters.Add("@Parameter1", SqlDbType.VarChar, 50).Value = E.financial_year;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("select a.*,b.Dept_name,c.Name 'Report' from emp_details as a left join tbl_department as b on a.Department=b.Dept_id"
                                                + " left join tb_managerHOD as c on a.team_lead = c.Emp_code where a.Department=@Parameter and  a.dob LIKE '%'+@Parameter1+'%' OR a.DateOfJoin LIKE '%'+@Parameter1+'%' ", con);

                    cmd.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = E.Department;
                    cmd.Parameters.Add("@Parameter1", SqlDbType.VarChar, 50).Value = E.financial_year;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
                
                var result = new List<Models.Employee>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.Employee
                    {
                        EmpCode = Convert.ToString(row["Emp_code"]),
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
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }



        public ActionResult Get_overall_role(Models.over_all L)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Get_overall_role";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = new List<Models.over_all>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.over_all
                    {
                        title_name = Convert.ToString(row["title_name"]),
                        total_count = Convert.ToString(row["total_count"]),
                        male_count = Convert.ToString(row["male_count"]),
                        female_count = Convert.ToString(row["female_count"]),
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult Get_overall_brand(Models.over_all L)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Get_overall_brand";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = new List<Models.over_all>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.over_all
                    {
                        title_name = Convert.ToString(row["title_name"]),
                        total_count = Convert.ToString(row["total_count"]),
                        male_count = Convert.ToString(row["male_count"]),
                        female_count = Convert.ToString(row["female_count"]),
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Get_overall_desig(Models.over_all L)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Get_overall_desig";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = new List<Models.over_all>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(item: new Models.over_all
                    {
                        title_name = Convert.ToString(row["title_name"]),
                        total_count = Convert.ToString(row["total_count"]),
                        male_count = Convert.ToString(row["male_count"]),
                        female_count = Convert.ToString(row["female_count"]),
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }
    }

}