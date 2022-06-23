
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Important
{
    public partial class _Default : Page
    {
        public SqlConnection con;// = new SqlConnection(strcon);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Visible = false;
                lblMessage.Text = string.Empty;
                txtName.Focus();
            }
        }
        protected void lknGenerateTemplate_Click(object sender, EventArgs e)
        {
            Response.Redirect("UploadFile.aspx");
        }
        protected void lnkLogin_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false; lblMessage.Text = "";
            if (txtName.Text.Trim().Length == 0) { lblMessage.Visible = true; lblMessage.Text = "Please Enter Name."; txtName.Focus(); }

            else if (txtPassword.Text.Trim().Length == 0) { lblMessage.Visible = true; lblMessage.Text = "Please Enter Password.";txtPassword.Focus(); }
            else
            {
                try
                {
                    string strcon = ConfigurationManager.ConnectionStrings["DBConnect"].ConnectionString;
                    con = new SqlConnection(strcon);
                    con.Open();
                    SqlCommand command = new SqlCommand("Select count(*) from LoginDetails where Upper(UserName)=Upper('"+txtName.Text.Trim()+ "') and Upper(UserPassword)=Upper('" + txtPassword.Text.Trim()+"')", con);
                    int result = (int)(command.ExecuteScalar());
                    if (result > 0)
                    {
                        if (txtName.Text.ToUpper().Trim() == "ADMIN")
                        {
                            
                            Response.Redirect("AddReceipents.aspx");
                        }
                        else
                        {
                            SqlCommand cmd = new SqlCommand("Select * from  LoginDetails where Upper(UserName)=Upper('" + txtName.Text.Trim() + "') and Upper(UserPassword)=Upper('" + txtPassword.Text.Trim() + "')", con);

                            using (var cmdSign = new SqlDataAdapter(cmd))
                            {
                                System.Data.DataTable dt = new System.Data.DataTable();
                                cmdSign.Fill(dt);
                                foreach (DataRow row in dt.Rows)
                                {
                                    Session["LoginUserName"] = row["UserName"].ToString().Trim();
                                    Session["LoginUserEmail"] = row["UserEmail"].ToString().Trim();
                                    Session["LoginUserId"] = row["UserId"].ToString().Trim();
                                    Response.Redirect("RecipientSign.aspx");
                                }
                            }
                           
                            //Check for Approval Position
                          /*  using (var cmd = new SqlCommand("APPROVER_STATUS", con))
                            {
                                cmd.Connection = con;
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@LoginUserName", txtName.Text.Trim());
                                using (var cmdSign = new SqlDataAdapter(cmd))
                                {
                                    System.Data.DataTable dt = new System.Data.DataTable();
                                    cmdSign.Fill(dt);
                                    foreach (DataRow row in dt.Rows)
                                    {
                                       if(row["Approval"].ToString().ToUpper().Trim()=="NOT COMPLETED")
                                        {
                                            if (row["Status"].ToString() == "0")
                                            {
                                                lblMessage.Visible = true;
                                                lblMessage.Text = row["Message"].ToString();
                                                break;
                                            }
                                            else
                                            {
                                               // Response.Redirect("RecipientSign.aspx");
                                            }

                                        }
                                        else
                                        {
                                            lblMessage.Visible = true;
                                            lblMessage.Text = "All the Approvers have Signed the Document.";
                                        }



                                       
                                        
                                    }
                                }

                            }
                            */
                        }

                        
                    }
                    else
                        {
                            lblMessage.Visible = true; lblMessage.Text = "Invalid User."; txtName.Focus();
                        }

                }
                catch (Exception ex)
                {
                    lblMessage.Visible = true; lblMessage.Text = "Database connectivity Error.";
                }
            }


        }
    }
}