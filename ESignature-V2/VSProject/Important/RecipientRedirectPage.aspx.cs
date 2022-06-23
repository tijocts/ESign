using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Important
{
    public partial class RecipientRedirectPage : System.Web.UI.Page
    {
        List<Receipents> ReceipentList = new List<Receipents>();
        Receipents AddedReceipents = new Receipents();
        System.Data.DataTable dtTable;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Data.DataTable Datatable = (System.Data.DataTable)Session["RecipientesList"];
                //Creating a DataTable.
                if (Datatable.Rows.Count != 0)
                {
                    dtTable = new System.Data.DataTable();
                    dtTable.Columns.Add("Id", typeof(string));
                    dtTable.Columns.Add("Name", typeof(string));
                    dtTable.Columns.Add("Email", typeof(string));
                    
                    lstViewReceipents.DataSource = Datatable;
                    lstViewReceipents.DataBind();
                    lblMessage.Visible = false;
                }
                else
                {
                    lstViewReceipents.DataSource = null;
                    lstViewReceipents.DataBind();
                    lblMessage.Visible = false;
                }
            }

        }

        protected void btnSign_Click(object sender, EventArgs e)
        {

            try
            {
                //HiddenField lblModuleTitle = (HiddenField)lstViewReceipents.Items(lstViewReceipents.SelectedIndex).FindControl("hdnRecipienId");

                //HiddenField ID = lstViewReceipents.FindControl("hdnRecipienId") as HiddenField;
                var ID = ((System.Web.UI.WebControls.LinkButton)sender).CommandName;
                    if (ID == "1")
                        Response.Redirect("RecipientPage.aspx?Color=" + "Red");

                    //Session["Color"] = "Red";
                    if (ID == "2")
                        Response.Redirect("RecipientPage.aspx?Color=" + "Blue");

                    //Session["Color"] = "Blue";
                

            }
            catch (Exception ex){
                lblMessage.Visible = true;
                lblMessage.Text = "Error Occured.";
            }

        }
    }
}