//using iTextSharp.text;
//using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Windows.Forms;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
//using Control = System.Web.UI.Control;
//using iText.Kernel.Pdf;
/*using iText.Kernel.Pdf.Annot;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Font;
using iText.Forms;
using iText.Forms.Fields;*/
using iTextSharp.text.pdf;
using iTextSharp.text;
using Rectangle = iTextSharp.text.Rectangle;
using Newtonsoft.Json;
using Image = iTextSharp.text.Image;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

using System.Diagnostics;
using System.Text;

using iTextSharp.text.pdf.parser;
using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;
using Path = System.IO.Path;
using Application = Microsoft.Office.Interop.Word.Application;
using Document = Microsoft.Office.Interop.Word.Document;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Net.Mail;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
//using DataTable=;

namespace Important
{
    public partial class AddReceipents : System.Web.UI.Page
    {

        List<Receipents> ReceipentList = new List<Receipents>();
        Receipents AddedReceipents = new Receipents();
        System.Data.DataTable dtTable;
        Byte[] bytes;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{

            //Creating a DataTable.

            dtTable = new System.Data.DataTable();
            dtTable.Columns.Add("Id", typeof(string));
            dtTable.Columns.Add("Name", typeof(string));
            dtTable.Columns.Add("Email", typeof(string));

            ListViewBind();

             bytes = File.ReadAllBytes(Server.MapPath("\\SignOrder\\SignDocument.pdf"));
            String file = Convert.ToBase64String(bytes);
            pdfBase64.Value = file;
            txtName.Focus();

            //}

        }

        public void ListViewBind()
        {

            //List<Receipents> data = new List<Receipents>
            //{
            //    new Receipents(){ID="1",Name="Sam Joseph",Email="Sam.Joseph@Claysys.net"},
            //    new Receipents(){ID="2",Name="David S",Email="David.S@Claysys.net" }

            //};
            lstViewReceipents.DataSource = null;
            lstViewReceipents.DataBind();

        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                if (Session["RecipientesList"] == null)
                {
                    dtTable.Rows.Add(new Object[]{
                   // Guid.NewGuid(),
                   1,
                    txtName.Text,
                    txtEmail.Text
                    });
                    Session["RecipientesList"] = dtTable;
                }
                else
                {
                    System.Data.DataTable NewDatatable = (System.Data.DataTable)Session["RecipientesList"];
                    NewDatatable.Rows.Add(new Object[]{
                   // Guid.NewGuid(),
                   lstViewReceipents.Items.Count+1,
                    txtName.Text,
                    txtEmail.Text
                    });
                    Session["RecipientesList"] = NewDatatable;

                }

                lstViewReceipents.DataSource = Session["RecipientesList"];
                lstViewReceipents.DataBind();
                txtName.Text = String.Empty;
                txtEmail.Text = String.Empty;

            }
            catch (Exception ex)
            {

            }

        }

        public class Positions
        {
            public float X { get; set; }
            public float Y { get; set; }
        }


        protected void lnk_Click(object sender, EventArgs e)
        {
            try
            {
                //string deserializedArgs = Request["__EVENTARGUMENT"];
                string ControlsList = ((Request.Form["__EVENTARGUMENT"].Split(']'))[0].Split('['))[1];
                string Recipients = (Request.Form["__EVENTARGUMENT"].Split('['))[1].Split(']')[1];



                //Getting the control details that is dragged to Pdf for the respective recipients
                List<string> Controllist = new List<string>();
                List<PdfControlAndRecipientsDetails> ControlPosition = new List<PdfControlAndRecipientsDetails>();

                Controllist = ControlsList.Split('}').ToList();

                for (int i = 0; i < ControlsList.Split('}').ToList().Count; i++)
                {
                    PdfControlAndRecipientsDetails items;
                    string Controls = ControlsList.Replace('{', ' ').Replace('}', ' ').ToString().TrimStart().Trim();
                    for (int k = 0; k < Controls.Split(',').Length;)
                    {
                        items = new PdfControlAndRecipientsDetails();
                        items.ControlName = Controls.Split(',')[k].Split(':')[1];
                        items.PositionX = Controls.Split(',')[k + 1].Split(':')[1];
                        items.PositionY = Controls.Split(',')[k + 2].Split(':')[1];
                        items.Color = Controls.Split(',')[k + 3].Split(':')[1];
                        items.X = Controls.Split(',')[k + 4].Split(':')[1];
                        items.Y = Controls.Split(',')[k + 5].Split(':')[1];
                        k = k + 5;
                        k++;
                        //break;
                        ControlPosition.Add(items);
                    }
                    break;
                }
                Session["ControlPosition"] = ControlPosition;

                //Getting the Recipients Details                 
                List<RecipientsList> List = new List<RecipientsList>();
                RecipientsList ReciptsAdded;
                for (int t = 0; t < Recipients.Split(':')[1].Replace('}', ' ').Trim().Split(',').Length;)
                {
                    ReciptsAdded = new RecipientsList();
                    ReciptsAdded.Id = Recipients.Split(':')[1].Replace('}', ' ').Trim().Split(',')[t];
                    ReciptsAdded.Name = Recipients.Split(':')[1].Replace('}', ' ').Trim().Split(',')[t + 1];
                    ReciptsAdded.Email = Recipients.Split(':')[1].Replace('}', ' ').Trim().Split(',')[t + 2];
                    ReciptsAdded.Color = Recipients.Split(':')[1].Replace('}', ' ').Trim().Split(',')[t + 3];
                    t = t + 3;
                    t++;
                    List.Add(ReciptsAdded);

                }

                Session["RecipientsList"] = List;

                for (int i = 0; i < List.Count; i++)
                {

                    dtTable.Rows.Add(List[i].Id, List[i].Name, List[i].Email);
                }
                System.Data.DataTable distinctTable = dtTable.DefaultView.ToTable( /*distinct*/ true);
                DBSaving(distinctTable, bytes, ControlPosition);
                lblMessage.Text = "Document has been send to Recipients for Signing.";
                lblMessage.Visible = true;




            }
            catch (Exception ex)
            {
                if (ex.Message != "Thread was being aborted.")
                {
                    lblMessage.Text = ex.Message;
                    lblMessage.Visible = true;
                }
                else
                {
                    Response.End();
                }
            }
        }

        private void DBSaving(System.Data.DataTable datatable,Byte[] bytes, List<PdfControlAndRecipientsDetails> ControlPosition)
        {
            string strcon = ConfigurationManager.ConnectionStrings["DBConnect"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            // con.Open();

            if (datatable.Rows.Count > 0)
            {
                int result = 0;
                string Approvers = string.Empty;
                int ApproverCount=0;
                var FirstApprover = string.Empty;
                //Insertion to LoginDetails Table
                foreach (DataRow row in datatable.Rows)
                {

                    using (var cmd = new SqlDataAdapter())
                    {
                        using (var Selectcommand = new SqlCommand("Select count(*) from LoginDetails"))
                        {
                            Selectcommand.Connection = con;
                            cmd.InsertCommand = Selectcommand;
                            if (result == 0) con.Open();
                            result = (int)(Selectcommand.ExecuteScalar());
                            result = result + 1;
                            using (var insertCommand = new SqlCommand("insert into LoginDetails(UserId,UserName,UserEmail,UserPassword)values('" + result + "','" + row["Name"] + "','" + row["Email"] + "','pass')"))
                            {
                                if (Approvers.Length == 0) { Approvers = result.ToString(); }
                                else
                                    Approvers = Approvers +","+ result.ToString();
                                insertCommand.Connection = con;
                                cmd.InsertCommand = insertCommand;
                                //con.Open();
                                cmd.InsertCommand.ExecuteNonQuery();
                            }
                        }
                    }                    

                }
                if (Approvers.Contains(',')) { ApproverCount = Approvers.Split(',').Length; FirstApprover = Approvers.Split(',')[0]; }
                else { ApproverCount = Approvers.Length; FirstApprover = Approvers; }

                //Insertion to Approver Table
                var DocumentNumber = "Document-" + Guid.NewGuid();
                using (var commamd = new SqlDataAdapter())
                {
                    int ApproverId = 0;
                    using (var Selectcommand = new SqlCommand("Select count(*) from ApproverDetails"))
                    {
                        Selectcommand.Connection = con;
                        commamd.InsertCommand = Selectcommand;
                       // if (ApproverId == 0) con.Open();
                        ApproverId = (int)(Selectcommand.ExecuteScalar());
                        ApproverId = ApproverId + 1;
                        using (var insertCommand = new SqlCommand("insert into ApproverDetails(ApproverId,Approvers,Document,ApproversCount,NextApprover,DocumentNumber)values(@ApproverId,@Approvers,@bytes,@ApproverCount,@FirstApprover,@DocumentNumber)"))
                        {
                            insertCommand.Connection = con;
                            insertCommand.Parameters.AddWithValue("@ApproverId", ApproverId);
                            insertCommand.Parameters.AddWithValue("@Approvers", Approvers);
                            insertCommand.Parameters.AddWithValue("@bytes", bytes);
                            insertCommand.Parameters.AddWithValue("@ApproverCount", ApproverCount);
                            insertCommand.Parameters.AddWithValue("@FirstApprover", FirstApprover);
                            insertCommand.Parameters.AddWithValue("@DocumentNumber", DocumentNumber);                            
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
                //Insertion to User SignatureControls Mapping Table

                using (var cmdSign   = new SqlDataAdapter())
                {
                    using (var Selectcommand = new SqlCommand("Select count(*) from UserSignatureControls"))
                    {
                        Selectcommand.Connection = con;
                        cmdSign.InsertCommand = Selectcommand;
                        if (result == 0) con.Open();
                        result = (int)(Selectcommand.ExecuteScalar());
                        
                        foreach(var data in ControlPosition)
                        {
                            result = result + 1;
                            using (var insertCommand = new SqlCommand("insert into UserSignatureControls(ControlId,UserId,ControlName,PdfPositionX,PdfPositionY,ScreenX,ScreenY,DocumentNumber)values(@ControlId,@UserId,@ControlName,@PdfPositionX,@PdfPositionY,@ScreenX,@ScreenY,@DocumentNumber)"))
                            {
                                insertCommand.Connection = con;
                                insertCommand.Parameters.AddWithValue("@ControlId", result);                               
                                if (data.Color.Replace('"', ' ').Trim() == "Red")
                                {
                                    if (Approvers.Contains(',')) { FirstApprover = Approvers.Split(',')[0]; }
                                    else { FirstApprover = Approvers; }
                                    insertCommand.Parameters.AddWithValue("@UserId", FirstApprover);
                                }
                                if (data.Color.Replace('"', ' ').Trim() == "Blue")
                                {
                                    if (Approvers.Contains(',')) { FirstApprover = Approvers.Split(',')[1]; }
                                    else { FirstApprover = Approvers; }
                                    insertCommand.Parameters.AddWithValue("@UserId", FirstApprover);
                                }                                
                                insertCommand.Parameters.AddWithValue("@ControlName",data.ControlName );
                                insertCommand.Parameters.AddWithValue("@PdfPositionX",data.PositionX );
                                insertCommand.Parameters.AddWithValue("@PdfPositionY", data.PositionY);
                                insertCommand.Parameters.AddWithValue("@ScreenX",data.X );
                                insertCommand.Parameters.AddWithValue("@ScreenY", data.Y);
                                insertCommand.Parameters.AddWithValue("@DocumentNumber", DocumentNumber);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
                ReminderMailToApprovers(con, DocumentNumber);
             }

        }
        private void ReminderMailToApprovers(SqlConnection con,string DocumentNumber)
        {
            
                using (var cmd = new SqlCommand("APPROVING_ORDER", con))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentNumber", DocumentNumber);
                    using (var cmdSign = new SqlDataAdapter(cmd))
                    {
                        System.Data.DataTable dt = new System.Data.DataTable();
                        cmdSign.Fill(dt);
                    SendMailReminder(dt);
                    } 
                    
                }             
            
        }
        public void SendMailReminder(System.Data.DataTable dt)
        {

           
            string to = "roopa.prabha@claysys.in"; //lbEmailAddress.Text;//HR To address    
            string from = "noreply@myappforms.net"; //From address    
            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(from, to))
            {

                string mailbody = GetEmailHtmlBody(dt);
                mail.Subject = "Reminder:Document to get Signed.";
                mail.Body = mailbody;
                mail.CC.Add(new MailAddress("roopa.prabha@claysys.in"));//lblRecipientEmail.Text;
                mail.BodyEncoding = Encoding.UTF8;
                SmtpClient client = new SmtpClient("smtp.1and1.com", 587); //Gmail smtp
                client.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicCredential1 = new
                System.Net.NetworkCredential("noreply@myappforms.net", "N063plyMy1PP");
                mail.IsBodyHtml = true;
                client.EnableSsl = true;                
                client.Credentials = basicCredential1;
                try
                {
                    client.Send(mail);
                }

                catch (Exception ex)
                {
                  
                    throw ex;
                }
            }
            
        }

        public string GetEmailHtmlBody(System.Data.DataTable dt)
        {
            StringBuilder HtmlTextContent = null;
            try
            {


                HtmlTextContent = new StringBuilder();
                HtmlTextContent.Append("<html><body><div style='background-color:#eaeaea;padding:2%;font-family:Helvetica,Arial,Sans Serif'><table cellspacing='0' cellpadding='0' align='center' width='100%'><tbody><tr><td></td><td>" +
                    "<table style='border-collapse:collapse;background-color:#ffffff;max-width:640px'><tbody><tr><td style='padding:10px 24px;font-weight:bold;'><h3>ESignature</h3></td></tr><tr><td style='padding: 0px 24px 30px 24px'>" +
                    "<table align='center' style='color:#000000' width='100%'>" +
                    "<tbody><tr><td align='center' style='tyle='padding - top:24px; font - size:16px; font - family:Helvetica,Arial,Sans Serif;font-weight:bold; border: none; text - align:left; color:#ffffff'>" +
                    "PDF-Document has send for Signing.</td></tr>");
                HtmlTextContent.Append("<tr><td style='align:center'><table border='1' style='align:center'><tr style='background-color:#87ceeb;'><td style='font-weight:bolder;'>Approvers Name</td><td style='font-weight:bolder;'>Order</td></tr>");
                foreach (DataRow data in dt.Rows)
                {
                    HtmlTextContent.Append("<tr><td >"+data["Approvers"] +"</td><td>"+data["Order"]+"</td></tr>");
                }
                HtmlTextContent.Append("</table></td></tr></tbody></table></td></tr></tbody></table></div></body></html>");


            }
            catch (Exception ex)
            {
                //Logging.Error("Error while sending the results mail. Error {0}", ex);
                //throw;
            }
            return HtmlTextContent.ToString();
        }

        protected void LogOut_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
    }



}


