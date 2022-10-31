using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Loadproject.Controllers
{
    public class SettingsController : Controller
    {
        // GET: Settings
        private string connectionstring = ConfigurationManager.ConnectionStrings["con"].ToString();

        //Roles Details
        public ActionResult Roles()
        {
            if (Session["UserName"] != null)
            {
                IEnumerable<Models.Roles> data = Roles_details();
                return View(data);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        private IEnumerable<Models.Roles> Roles_details()
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "all_roles";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Models.Roles
                    {
                        Id = Convert.ToString(row["Id"]),
                        Role_name = Convert.ToString(row["Role_name"]),
                        Status = Convert.ToString(row["Status"]),
                    };
                }
            }
        }

        [HttpPost]
        public ActionResult Insert_role(Models.Roles S)
        {
            if (Session["UserName"] != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionstring))
                    {
                        string ID = "";
                        if (S.QueryType == "Insert")
                        {
                            ID = "0";
                        }
                        else
                        {
                            ID = S.Id;
                        }
                        string response = string.Empty;
                        con.Open();
                        SqlCommand cmd = new SqlCommand("sp_Roles", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = S.QueryType;
                        cmd.Parameters.Add("@Id", SqlDbType.VarChar, 50).Value = ID;
                        cmd.Parameters.Add("@Role_name", SqlDbType.VarChar, 50).Value = S.Role_name;
                        cmd.Parameters.Add("@Created_by", SqlDbType.VarChar, 50).Value = Session["UserName"];
                        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = 'Y';
                        SqlParameter SQLReturn = new SqlParameter("@SQLReturn", SqlDbType.NVarChar, 50);
                        SQLReturn.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(SQLReturn);
                        cmd.ExecuteNonQuery();
                        response = SQLReturn.Value.ToString().Trim();

                        return RedirectToAction("Roles", "Settings", new { ac = response });
                    }
                }


                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Add Role Details ";
                    return View("Roles");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Edit_role(string id)
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
                        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "edit_roles";
                        cmd.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = id;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                        var result = new List<Models.Roles>();
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            result.Add(item: new Models.Roles
                            {
                                Id = Convert.ToString(row["Id"]),
                                Role_name = Convert.ToString(row["Role_name"]),
                            });
                        }

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Failed Edit Details ";
                    return View("Roles");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Delete_role(string ID)
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
                        cmd1.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Delete_Roles";
                        cmd1.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = ID;
                        cmd1.ExecuteNonQuery();
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                }


                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Delete Role";
                    return View("Roles");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }


        //Brand Details
        public ActionResult Brand()
        {
            if (Session["UserName"] != null)
            {
                IEnumerable<Models.Brand> data = Brand_details();
                return View(data);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


        private IEnumerable<Models.Brand> Brand_details()
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

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Models.Brand
                    {
                        Id = Convert.ToString(row["Id"]),
                        Brand_name = Convert.ToString(row["Brand_name"]),
                        Status = Convert.ToString(row["Status"]),
                    };
                }
            }
        }

        [HttpPost]
        public ActionResult Insert_brand(Models.Brand S)
        {
            if (Session["UserName"] != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionstring))
                    {
                        string ID = "";
                        if (S.QueryType == "Insert")
                        {
                            ID = "0";
                        }
                        else
                        {
                            ID = S.Id;
                        }
                        string response = string.Empty;
                        con.Open();
                        SqlCommand cmd = new SqlCommand("sp_Brands", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = S.QueryType;
                        cmd.Parameters.Add("@Id", SqlDbType.VarChar, 50).Value = ID;
                        cmd.Parameters.Add("@Brand_name", SqlDbType.VarChar, 50).Value = S.Brand_name;
                        cmd.Parameters.Add("@Created_by", SqlDbType.VarChar, 50).Value = Session["UserName"];
                        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = 'Y';
                        SqlParameter SQLReturn = new SqlParameter("@SQLReturn", SqlDbType.NVarChar, 50);
                        SQLReturn.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(SQLReturn);
                        cmd.ExecuteNonQuery();
                        response = SQLReturn.Value.ToString().Trim();

                        return RedirectToAction("Brand", "Settings", new { ac = response });
                    }
                }


                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Add Brand Details ";
                    return View("Brand");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }



        public ActionResult Edit_brand(string id)
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
                        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "edit_brands";
                        cmd.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = id;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                        var result = new List<Models.Brand>();
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            result.Add(item: new Models.Brand
                            {
                                Id = Convert.ToString(row["Id"]),
                                Brand_name = Convert.ToString(row["Brand_name"]),
                            });
                        }

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Failed Edit Details ";
                    return View("Brand");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Delete_brand(string ID)
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
                        cmd1.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Delete_Brand";
                        cmd1.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = ID;
                        cmd1.ExecuteNonQuery();
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                }


                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Delete Role";
                    return View("Brand");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }


        //Designation Details
        public ActionResult Designation()
        {
            if (Session["UserName"] != null)
            {
                IEnumerable<Models.Designation> data = Designation_details();
                return View(data);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


        private IEnumerable<Models.Designation> Designation_details()
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

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Models.Designation
                    {
                        Id = Convert.ToString(row["Id"]),
                        Designation_name = Convert.ToString(row["Designation_name"]),
                        Status = Convert.ToString(row["Status"]),
                    };
                }
            }
        }


        [HttpPost]
        public ActionResult Insert_designation(Models.Designation S)
        {
            if (Session["UserName"] != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionstring))
                    {
                        string ID = "";
                        if (S.QueryType == "Insert")
                        {
                            ID = "0";
                        }
                        else
                        {
                            ID = S.Id;
                        }
                        string response = string.Empty;
                        con.Open();
                        SqlCommand cmd = new SqlCommand("sp_designations", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = S.QueryType;
                        cmd.Parameters.Add("@Id", SqlDbType.VarChar, 50).Value = ID;
                        cmd.Parameters.Add("@Designation_name", SqlDbType.VarChar, 50).Value = S.Designation_name;
                        cmd.Parameters.Add("@Created_by", SqlDbType.VarChar, 50).Value = Session["UserName"];
                        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = 'Y';
                        SqlParameter SQLReturn = new SqlParameter("@SQLReturn", SqlDbType.NVarChar, 50);
                        SQLReturn.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(SQLReturn);
                        cmd.ExecuteNonQuery();
                        response = SQLReturn.Value.ToString().Trim();

                        return RedirectToAction("Designation", "Settings", new { ac = response });
                    }
                }


                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Add Designation Details ";
                    return View("Designation");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Edit_designation(string id)
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
                        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "edit_designation";
                        cmd.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = id;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                        var result = new List<Models.Designation>();
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            result.Add(item: new Models.Designation
                            {
                                Id = Convert.ToString(row["Id"]),
                                Designation_name = Convert.ToString(row["Designation_name"]),
                            });
                        }

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Failed Edit Details ";
                    return View("Designation");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Delete_designation(string ID)
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
                        cmd1.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Delete_Designation";
                        cmd1.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = ID;
                        cmd1.ExecuteNonQuery();
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                }


                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Delete Designation";
                    return View("Designation");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        //Department Details
        public ActionResult Department()
        {
            if (Session["UserName"] != null)
            {
                IEnumerable<Models.Department> data = Dept_details();
                return View(data);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        private IEnumerable<Models.Department> Dept_details()
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("SP_GetSettings_data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "all_dept";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    yield return new Models.Department
                    {
                        Id = Convert.ToString(row["Id"]),
                        Dept_name = Convert.ToString(row["Dept_name"]),
                        Status = Convert.ToString(row["Status"]),
                    };
                }
            }
        }

        [HttpPost]
        public ActionResult Insert_dept(Models.Department S)
        {
            if (Session["UserName"] != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionstring))
                    {
                        string ID = "";
                        if (S.QueryType == "Insert")
                        {
                            ID = "0";
                        }
                        else
                        {
                            ID = S.Id;
                        }
                        string response = string.Empty;
                        con.Open();
                        SqlCommand cmd = new SqlCommand("sp_department", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = S.QueryType;
                        cmd.Parameters.Add("@Id", SqlDbType.VarChar, 50).Value = ID;
                        cmd.Parameters.Add("@Dept_name", SqlDbType.VarChar, 50).Value = S.Dept_name;
                        cmd.Parameters.Add("@Created_by", SqlDbType.VarChar, 50).Value = Session["UserName"];
                        cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = 'Y';
                        SqlParameter SQLReturn = new SqlParameter("@SQLReturn", SqlDbType.NVarChar, 50);
                        SQLReturn.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(SQLReturn);
                        cmd.ExecuteNonQuery();
                        response = SQLReturn.Value.ToString().Trim();

                        return RedirectToAction("Department", "Settings", new { ac = response });
                    }
                }


                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Add Department Details ";
                    return View("Department");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Edit_dept(string id)
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
                        cmd.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "edit_dept";
                        cmd.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = id;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                        var result = new List<Models.Department>();
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            result.Add(item: new Models.Department
                            {
                                Id = Convert.ToString(row["Id"]),
                                Dept_name = Convert.ToString(row["Dept_name"]),
                            });
                        }

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Failed Edit Details ";
                    return View("Department");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Delete_dept(string ID)
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
                        cmd1.Parameters.Add("@QueryType", SqlDbType.VarChar, 50).Value = "Delete_dept";
                        cmd1.Parameters.Add("@Parameter", SqlDbType.VarChar, 50).Value = ID;
                        cmd1.ExecuteNonQuery();
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                }


                catch (Exception ex)
                {
                    ViewBag.Message = "Failed to Delete Department";
                    return View("Department");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
    }
}