using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Image = iTextSharp.text.Image;
using Org.BouncyCastle.Pkcs;
using System.Collections;
using iTextSharp.text.xml.xmp;
using Org.BouncyCastle.Crypto;
using System.Windows.Forms;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Ocsp;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Tls;
using iTextSharp.text.error_messages;
using System.Net.Mail;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Net;
using System.Data;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using org.bouncycastle.jce.provider;

namespace Important
{
    public partial class RecipientSign : System.Web.UI.Page
    {
        string details;
        private Org.BouncyCastle.X509.X509Certificate[] chain;
        private List<X509Certificate> certificates = new List<X509Certificate>();
        public AsymmetricKeyParameter akp;
        //private MetaData metadata;
        private static readonly int CER_STATUS_NOT_VERIFIED = 25;
        //public Signature SignatureData { set; get; }
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        SqlConnection con;
        Byte[] pdf;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            btn.Enabled = true;
            btndownload.Visible = false;

            StringBuilder ControlsArray = new StringBuilder();
            if (!IsPostBack)
            {
                try
                {
                    string DocumentNumber = string.Empty;
                    lblRecipientName.Text = Session["LoginUserName"].ToString();
                    lblRecipientEmail.Text = Session["LoginUserEmail"].ToString();
                    string strcon = ConfigurationManager.ConnectionStrings["DBConnect"].ConnectionString;
                    con = new SqlConnection(strcon);
                    con.Open();
                    using (var cmnd = new SqlCommand("select appr.Document,appr.DocumentNumber,appr.Approvers,appr.ApprovedAndSignedDocument from ApproverDetails appr where appr.Approvers like('%" + Session["LoginUserId"] + "%');", con))
                    {
                        cmnd.Connection = con;
                        using (SqlDataReader reader = cmnd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DocumentNumber = reader["DocumentNumber"].ToString();
                                Session["DocumentNumber"] = reader["DocumentNumber"].ToString();
                                Session["Approvers"] = reader["Approvers"].ToString();
                                if (reader["ApprovedAndSignedDocument"].ToString().Length == 0)
                                {
                                    String file = Convert.ToBase64String((byte[])reader["Document"]);
                                    pdfBase64.Value = file;
                                    pdf = Convert.FromBase64String(file);
                                }
                                else
                                {
                                    String file = Convert.ToBase64String((byte[])reader["ApprovedAndSignedDocument"]);
                                    pdfBase64.Value = file;
                                    pdf = Convert.FromBase64String(file);
                                }
                            }
                        }

                    }

                    using (var cmd = new SqlCommand("APPROVER_STATUS", con))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@LoginUserName", Session["LoginUserName"].ToString());
                        using (var cmdSign = new SqlDataAdapter(cmd))
                        {
                            System.Data.DataTable dt = new System.Data.DataTable();
                            cmdSign.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["Message"].ToString().Trim().Length != 0)
                                {
                                    lblMessage.Visible = true;
                                    lblMessage.Text = row["Message"].ToString().Trim();
                                    btn.Enabled = false;
                                    btndownload.Enabled = false;
                                    if (row["Status"].ToString() == "3")
                                    {
                                        btndownload.Visible = true;
                                        btndownload.Enabled = true;
                                    }

                                }
                                else
                                {
                                    //Map the Controls for Signing
                                    using (var cmnd = new SqlCommand("select * from UserSignatureControls where DocumentNumber='" + DocumentNumber + "' and UserId=" + Session["LoginUserId"] + ";", con))
                                    {
                                        cmnd.Connection = con;
                                        using (var controls = new SqlDataAdapter(cmnd))
                                        {
                                            System.Data.DataTable datatable = new System.Data.DataTable();
                                            controls.Fill(datatable);


                                            foreach (DataRow rows in datatable.Rows)
                                            {
                                                ControlsArray.Append(rows["ControlName"].ToString().Replace('"', ' ').Trim());
                                                ControlsArray.Append(",");
                                                ControlsArray.Append(rows["PdfPositionX"].ToString());
                                                ControlsArray.Append(",");
                                                ControlsArray.Append(rows["PdfPositionY"].ToString());
                                                ControlsArray.Append(",");
                                                ControlsArray.Append(rows["ScreenX"].ToString());
                                                ControlsArray.Append(",");
                                                ControlsArray.Append(rows["ScreenY"].ToString());
                                                ControlsArray.Append(":");
                                            }

                                            Session["Cntrls"] = ControlsArray;
                                        }

                                    }
                                }
                            }
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", string.Format("SessionValues('{0}');", lblRecipientName.Text + "*" + ControlsArray), true);

                        }

                    }



                }
                catch (Exception ex)
                {

                }
            }

        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            try
            {
                String Mainpdf = Server.MapPath("\\SignOrder\\Approver.pdf");
                using (FileStream stream = System.IO.File.Create(Mainpdf))
                {
                    pdf = Convert.FromBase64String(pdfBase64.Value);
                    stream.Write(pdf, 0, pdf.Length);
                }
                String ControlsOnpdf = Server.MapPath("\\SignOrder\\ApproverControls.pdf");
                using (FileStream stream = System.IO.File.Create(ControlsOnpdf))
                {
                    pdf = Convert.FromBase64String(pdfBase64.Value);
                    stream.Write(pdf, 0, pdf.Length);
                }
                String pathin = Server.MapPath("\\SignOrder\\Approver.pdf");
                String pathout = Server.MapPath("\\SignOrder\\ApproverControls.pdf");
                string SignatureUpload = Server.MapPath("\\Images\\signupload.png");
                var GivenControls = Session["Cntrls"].ToString();
                using (PdfReader reader = new PdfReader(pathin))
                {

                    using (FileStream os = new FileStream(pathout, FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (PdfStamper stamper = new PdfStamper(reader, os))
                        {

                            var finalString = GivenControls.Replace('"', ' ').Trim();
                            var length = finalString.Split(':').Length;
                            //float value = float.MaxValue;
                            for (var i = 0; i < length - 1; i++)
                            {
                                for (var j = 3; j < finalString.Split(':')[i].Split(',').Length;)
                                {


                                    var PdfXPosition = Convert.ToSingle(finalString.Split(':')[i].Split(',')[1].Trim().Replace('"', ' ').Trim()) - .00220f;
                                    var PdfYPosition = Convert.ToSingle(finalString.Split(':')[i].Split(',')[2].Trim().Replace('"', ' ').Trim()) - 33.00000f;

                                    var Tag = finalString.Split(':')[i].Split(',')[0].Trim();
                                    if (Tag == "Name")
                                    {
                                        Image img = Image.GetInstance(SignatureUpload);
                                        float w = img.ScaledWidth;
                                        float h = img.ScaledHeight;
                                        PdfContentByte PDFDataA = stamper.GetOverContent(1);
                                        BaseFont baseFontA = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                                        PDFDataA.BeginText();
                                        PDFDataA.SetColorFill(BaseColor.RED);
                                        PDFDataA.SetFontAndSize(baseFontA, 11);
                                        //331.92242833052273f
                                        //513.2357142857143f
                                        PDFDataA.ShowTextAligned(PdfContentByte.ALIGN_LEFT, lblRecipientName.Text, PdfXPosition, PdfYPosition, 0);
                                        PDFDataA.EndText();
                                    }
                                    if (Tag == "Date")
                                    {
                                        PdfContentByte cb = stamper.GetOverContent(1);

                                        // DateTime now = DateTime.Now;
                                        // cb.ShowText(now.Date.ToString());
                                        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                                        cb.BeginText();
                                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, indianTime.ToString(), PdfXPosition, PdfYPosition, 0);
                                        cb.SetFontAndSize(iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, false), 9f);
                                        cb.MoveText(PdfXPosition, PdfYPosition);
                                        cb.SetColorFill(BaseColor.RED);

                                        cb.EndText();
                                    }
                                    break;
                                }


                            }
                            stamper.Close();
                            reader.Close();
                            os.Close();
                        }
                        os.Close();
                    }

                    reader.Close();

                }
                DigitalSignatureMapping(GivenControls);

            }
            catch (Exception ex)
            {

            }
        }
        public void DigitalSignatureMapping(string GivenControls)
        {
            try
            {
                string SrcFile = Server.MapPath("\\SignOrder\\ApproverControls.pdf");
                String ControlsOnpdf = Server.MapPath("\\SignOrder\\ApproverSigned.pdf");

                using (FileStream stream = System.IO.File.Create(ControlsOnpdf))
                {
                    pdf = Convert.FromBase64String(pdfBase64.Value);
                    stream.Write(pdf, 0, pdf.Length);

                }
                string DestFile = Server.MapPath("\\SignOrder\\ApproverSigned.pdf");
                // string SrcCertificate = Server.MapPath("\\Files\\" + fileCertificate.PostedFile.FileName);
                string SrcCertificate = Server.MapPath("\\Files\\PDF.pfx");
                string publicKeyStreamFile = Server.MapPath("\\Files\\PDF.cer");
                //BouncyCastleProvider provider = new BouncyCastleProvider();
               // Security.addProvider(provider);
                var pk = akp;
                lblMessage.Visible = false;
                lblMessage.Text = "";
                // var Status = "1";
                /////Password given for .pfx
                var Status = CheckCertificate(SrcCertificate, "roopatijo@123");

                if (Status == "2")
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = ".Pfx file Missing.Please Include.";
                    return;
                }
                if (Status == "1")
                {

                    PdfReader reader = new PdfReader(SrcFile);

                    Byte[] bytes = Convert.FromBase64String(hdnSignImage.Value.Substring(hdnSignImage.Value.LastIndexOf(',') + 1));

                    /////Saving the ImageBase64 as PNG
                    using (System.Drawing.Image images = System.Drawing.Image.FromStream(new MemoryStream(bytes)))
                    {
                        images.Save(Server.MapPath("\\Images\\SamSign_Digital.png"), ImageFormat.Png);  //  Png Save
                    }

                    string SignatureImagePath = Server.MapPath("\\Images\\SamSign_Digital.png");
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(SignatureImagePath);
                    image.SetAbsolutePosition(0, 0);
                    float w1 = image.ScaledWidth;
                    float h1 = image.ScaledHeight;

                    /////Get all Signature Tag Controls
                    var finalString = GivenControls.Split(':').ToList().Where(i => i.Contains("Signature")).ToList();
                    var len = finalString.Count;
                    Guid guid = Guid.NewGuid();
                    bool mode=false;
                    string ApproversCount = ApproverCount();                    
                    if (ApproversCount == "1") mode = true;
                    using (FileStream os = new FileStream(DestFile, FileMode.OpenOrCreate))
                    {
                        try
                        {   
                            using (PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0', null, mode))
                            {


                                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                                PdfSignature pdfSignature = new PdfSignature(PdfName.ADOBE_PPKLITE, PdfName.ADBE_PKCS7_DETACHED);
                                pdfSignature.Reason = "Approval Sign Order Workflow";
                                pdfSignature.Location = "India";
                                pdfSignature.Date = new PdfDate(Convert.ToDateTime("19-06-2022 12:53:28 AM"));
                                

                                PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                                if (mode == true)
                                {
                                    AcroFields acroFields = reader.AcroFields;
                                    List<String> names = acroFields.GetSignatureNames();
                                    foreach (string name in names)
                                    {
                                        if (name.Contains("first"))
                                        {
                                            acroFields.RemoveField(name);
                                        }
                                    }
                                }
                                
                                    for (var j = 0; j < len; j++)
                                    {


                                        //Get Pdf Positions
                                        var PdfXPosition = Convert.ToSingle(finalString[j].Split(',')[1].Trim().Replace('"', ' ').Trim()) - .00220f;
                                        var PdfYPosition = Convert.ToSingle(finalString[j].Split(',')[2].Trim().Replace('"', ' ').Trim()) - 33.00000f;
                                        Image Sign = Image.GetInstance(SignatureImagePath);
                                        Sign.SetAbsolutePosition(PdfXPosition, PdfYPosition);
                                        Sign.Alignment = Element.ALIGN_LEFT;
                                        stamper.GetOverContent(1).AddImage(Sign);
                                        appearance.Image = Sign;
                                        appearance.SignDate = Convert.ToDateTime(indianTime);                                     
                                        
                                        //For Invisible Signature make Height and Width to zero
                                       if(mode==false)
                                            appearance.SetVisibleSignature(new Rectangle(0, 0, 0, 0), 1, "first" + guid);
                                       else
                                        appearance.SetVisibleSignature(new Rectangle(0, 0, 0, 0), 1, "Sign" + guid);



                                }
                                


                               

                                IExternalSignature externalSignature = new PrivateKeySignature(akp, DigestAlgorithms.SHA512);
                                MakeSignatures.SignDetachedd(appearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);
                                //if(mode==true)Verify(DestFile);
                                DBApproverSignedFileSaving(DestFile);
                                
                            }
                            lblMessage.Text = "Document successfully Mailed and Uploaded to Folder (SignOrder/ApproverSigned.pdf).";
                            lblMessage.Visible = true;
                            btn.Enabled = false;
                            btndownload.Enabled = true;
                            btndownload.Visible = true;

                            //Show the Saved Signed File
                            string strcon = ConfigurationManager.ConnectionStrings["DBConnect"].ConnectionString;
                            con = new SqlConnection(strcon);
                            con.Open();
                            using (var cmnd = new SqlCommand("select appr.ApprovedAndSignedDocument from ApproverDetails appr where appr.Approvers like('%" + Session["LoginUserId"] + "%') and DocumentNumber=" + "'" + Session["DocumentNumber"].ToString() + "'", con))
                            {
                                cmnd.Connection = con;
                                using (SqlDataReader sqlreader = cmnd.ExecuteReader())
                                {
                                    if (sqlreader.Read())
                                    {
                                        if (sqlreader["ApprovedAndSignedDocument"].ToString().Length != 0)
                                        {
                                            String file = Convert.ToBase64String((byte[])sqlreader["ApprovedAndSignedDocument"]);
                                            pdfBase64.Value = file;
                                        }

                                    }
                                }
                            }




                        }
                        catch (Exception ex)
                        {
                            lblMessage.Text = ex.Message;
                            lblMessage.Visible = true;
                        }
                       
                        os.Close();
                        reader.Close();
                    }
                  

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;//"Error Occured in Digital Signature Function.";
                lblMessage.Visible = true;
            }
        }

        public void addAnnotation(String src, String dest, FileStream os)
        {

            PdfReader reader = new PdfReader(src);
            PdfStamper stamper = new PdfStamper(reader, os, '\0', true);
            PdfAnnotation comment = PdfAnnotation.CreateText(stamper.Writer,
            new Rectangle(200, 800, 250, 820), "Signed!", "Approver signed the document", true, "Comment");
            stamper.AddAnnotation(comment, 1);
            stamper.Close();

        }



          public void Verify(string DestFile)
          {
            string VerificationFile = Server.MapPath("\\SignOrder\\AppendModeVerificationFile.txt");
            using (FileStream os = new FileStream(VerificationFile, FileMode.OpenOrCreate))
            {
                PdfReader destinationreader = new PdfReader(DestFile);
                AcroFields acroFields = destinationreader.AcroFields;
                List<String> names = acroFields.GetSignatureNames();
                bool Valid = false;
                string NameoFSignatureValid = string.Empty;
                details = string.Empty;
                X509Certificate[] certs;
                PdfPKCS7 pdfPkcs7;
                StreamWriter writer = new StreamWriter(os);
                foreach (string name in names)
                {
                   
                    writer.WriteLine("Signature name: " + name);
                    writer.WriteLine("PDF Document revision: " + acroFields.GetRevision(name) + " of " + acroFields.TotalRevisions);

                    pdfPkcs7 = acroFields.VerifySignature(name);
                    var revisionName = acroFields.GetRevision(name);

                    // pdfPkcs7= VerifySignature(acroFields, name);
                    Valid = pdfPkcs7.Verify();
                    writer.WriteLine("Revision modified: " + !pdfPkcs7.Verify());
                   
                        //if (name.Contains("first"))
                        //{
                        //    acroFields.RemoveField(name);
                        //}
                    
                    //Object fails[] = pdfPkcs7.verifyCertificates(pkc, ks, null, cal);
                    NameoFSignatureValid = name;
                    var cal = pdfPkcs7.SignDate;
                    certs = pdfPkcs7.SignCertificateChain;
                    var pkc = pdfPkcs7.Certificates;
                    var reason = pdfPkcs7.Reason;
                    Org.BouncyCastle.X509.X509Certificate signingCertificate = pdfPkcs7.SigningCertificate;
                    var issuerDN = signingCertificate.IssuerDN;
                    var subjectDN = signingCertificate.SubjectDN;

                    //Writing to Log file
                    details = "Verified Status :" + Valid + "," + "Date:" + pdfPkcs7.SignDate.Date + "," + "Reason:" + reason + "," + "Issuer:" + issuerDN + "," + " Subject" + subjectDN;

                    if (!pdfPkcs7.Verify())
                    {
                        var Msg = "Not Verified.";
                        Valid = false;
                    }

                }
                writer.Flush();
                destinationreader.Close();
                os.Close();
            }
          }
        public string ApproverCount()
        {
            string strcon = ConfigurationManager.ConnectionStrings["DBConnect"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            con.Open();
            string ApproversCount;
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            using (var commamd = new SqlDataAdapter())
            {
                using (var Selectcommand = new SqlCommand("Select ApproversCount from ApproverDetails where DocumentNumber=" + "'" + Session["DocumentNumber"].ToString() + "'"))
                {
                    Selectcommand.Connection = con;
                    commamd.SelectCommand = Selectcommand;
                    ApproversCount = (string)Selectcommand.ExecuteScalar();

                }
            }
            return ApproversCount;
        }
        public string CheckCertificate(string CertificatePath, string password)
        {
            string Status = string.Empty;
            string keyAlias = null;
            try
            {
                //string alias = null;
                Pkcs12Store pk12;

                pk12 = new Pkcs12Store(new FileStream(CertificatePath, FileMode.Open, FileAccess.Read), password.ToCharArray());
                //  IEnumerator i = pk12.Aliases;
                foreach (string name in pk12.Aliases)
                {
                    if (pk12.IsKeyEntry(name))
                    {
                        keyAlias = name;
                        break;
                    }
                }
                //To get the private key
                akp = pk12.GetKey(keyAlias).Key;
                X509CertificateEntry[] ce = pk12.GetCertificateChain(keyAlias);
                chain = new Org.BouncyCastle.X509.X509Certificate[ce.Length];
                for (int k = 0; k < ce.Length; ++k)
                    chain[k] = ce[k].Certificate;
                Status = "1";
            }
            catch (Exception ex)
            {
                Status = "2";
            }
            return Status;
        }

        public string SendMail(string ApproverCount, string SignedLoginUser)
        {

            var Status = "1";
            string to = "roopa.prabha@claysys.in"; //lbEmailAddress.Text;//HR To address    
            string from = "noreply@myappforms.net";//lblRecipientEmail.Text;//"roopa.claysys@gmail.com"; //From address    
            using (System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(from, to))
            {

                string mailbody = GetEmailHtmlBody(ApproverCount, SignedLoginUser);
                message.Subject = "Signed Document";
                message.Body = mailbody;
                message.CC.Add(new MailAddress("roopa.prabha@claysys.in"));//lblRecipientEmail.Text;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Attachments.Add(new Attachment(Server.MapPath("\\SignOrder\\ApproverSigned.pdf")));
                SmtpClient client = new SmtpClient("smtp.1and1.com", 587);
                System.Net.NetworkCredential basicCredential1 = new
                System.Net.NetworkCredential("noreply@myappforms.net", "N063plyMy1PP");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = basicCredential1;
                try
                {
                    client.Send(message);
                }

                catch (Exception ex)
                {
                    Status = "0";
                    throw ex;
                }
            }
            return Status;
        }

        public string GetEmailHtmlBody(string ApproverCount, string SignedLoginUser)
        {
            StringBuilder HtmlTextContent = null;
            try
            {


                HtmlTextContent = new StringBuilder();
                if (ApproverCount == "1")
                    HtmlTextContent.Append("<html><body><div style='background-color:#eaeaea;padding:2%;font-family:Helvetica,Arial,Sans Serif'><table cellspacing='0' cellpadding='0' align='center' width='100%'><tbody><tr><td></td><td><table style='border-collapse:collapse;background-color:#ffffff;max-width:640px'><tbody><tr><td style='padding:10px 24px;font-weight:bold;'><h3>ESignature</h3></td></tr><tr><td style='padding: 0px 24px 30px 24px'><table align='center' style='background-color:#1e4ca1;color:#ffffff' width='100%'><tbody><tr><td align='center' style='tyle='padding - top:24px; font - size:16px; font - family:Helvetica,Arial,Sans Serif; border: none; text - align:center; color:#ffffff'>All the Approvers have Signed the Document.</td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></div></body></html>");
                else if (ApproverCount == "2")
                    HtmlTextContent.Append("<html><body><div style='background-color:#eaeaea;padding:2%;font-family:Helvetica,Arial,Sans Serif'><table cellspacing='0' cellpadding='0' align='center' width='100%'><tbody><tr><td></td><td><table style='border-collapse:collapse;background-color:#ffffff;max-width:640px'><tbody><tr><td style='padding:10px 24px;font-weight:bold;'><h3>ESignature</h3></td></tr><tr><td style='padding: 0px 24px 30px 24px'><table align='center' style='background-color:#1e4ca1;color:#ffffff' width='100%'><tbody><tr><td align='center' style='tyle='padding - top:24px; font - size:16px; font - family:Helvetica,Arial,Sans Serif; border: none; text - align:center; color:#ffffff'>First Approver(" + SignedLoginUser + ") Signed the Document.</td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></div></body></html>");


            }
            catch (Exception ex)
            {
                //Logging.Error("Error while sending the results mail. Error {0}", ex);
                //throw;
            }
            return HtmlTextContent.ToString();
        }

        public void DBApproverSignedFileSaving(string DestFile)
        {
            string strcon = ConfigurationManager.ConnectionStrings["DBConnect"].ConnectionString;
            SqlConnection con = new SqlConnection(strcon);
            con.Open();
            Byte[] bytes = File.ReadAllBytes(DestFile);
            string ApproversCount;
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            using (var commamd = new SqlDataAdapter())
            {
                using (var Selectcommand = new SqlCommand("Select ApproversCount from ApproverDetails where DocumentNumber=" + "'" + Session["DocumentNumber"].ToString() + "'"))
                {
                    Selectcommand.Connection = con;
                    commamd.SelectCommand = Selectcommand;
                    ApproversCount = (string)Selectcommand.ExecuteScalar();
                    string NextApprover;
                    if (Session["Approvers"].ToString().Contains(','))
                    {
                        NextApprover = Session["Approvers"].ToString().Split(',')[1];
                    }
                    else
                    {
                        NextApprover = Session["Approvers"].ToString();
                    }
                    string Query = string.Empty;
                    if (ApproversCount == "2")
                    {
                        Query = "Update ApproverDetails set ApproversCount='1',ApprovedAndSignedDocument=@ApprovedAndSignedDocument, NextApprover=" + NextApprover + ",LastApprovedDate=" + "'" + indianTime.ToString() + "'" + ",ApprovedBy=" + Session["LoginUserId"].ToString() + " where DocumentNumber=" + "'" + Session["DocumentNumber"].ToString() + "'";
                    }
                    if (ApproversCount == "1")
                    {
                        Query = "Update ApproverDetails set ApproversCount='0',ApprovedAndSignedDocument=@ApprovedAndSignedDocument, NextApprover=" + NextApprover + ",LastApprovedDate=" + "'" + indianTime.ToString() + "'" + ",ApprovedBy=" + Session["LoginUserId"].ToString() + " where DocumentNumber=" + "'" + Session["DocumentNumber"].ToString() + "'";

                    }

                    using (var updatecommand = new SqlCommand(Query))
                    {
                        updatecommand.Connection = con;
                        updatecommand.Parameters.AddWithValue("@ApprovedAndSignedDocument", bytes);
                        updatecommand.ExecuteNonQuery();
                    }
                }
            }
            SendMail(ApproversCount, Session["LoginUserName"].ToString());
        }

        protected void btndownload_Click(object sender, EventArgs e)
        {
            string strcon = ConfigurationManager.ConnectionStrings["DBConnect"].ConnectionString;
            con = new SqlConnection(strcon);
            con.Open();
            using (var cmnd = new SqlCommand("select appr.ApprovedAndSignedDocument from ApproverDetails appr where appr.Approvers like('%" + Session["LoginUserId"] + "%') and DocumentNumber=" + "'" + Session["DocumentNumber"].ToString() + "'", con))
            {
                cmnd.Connection = con;
                using (SqlDataReader sqlreader = cmnd.ExecuteReader())
                {
                    if (sqlreader.Read())
                    {
                        if (sqlreader["ApprovedAndSignedDocument"].ToString().Length != 0)
                        {
                            String file = Convert.ToBase64String((byte[])sqlreader["ApprovedAndSignedDocument"]);
                            pdfBase64.Value = file;
                            File.WriteAllBytes(Server.MapPath("\\SignOrder\\ApproverSigned.pdf"), (byte[])sqlreader["ApprovedAndSignedDocument"]);
                            Response.ContentType = "Application/pdf";
                            Response.AppendHeader("Content-Disposition", "attachment; filename=ApproverSigned.pdf");
                            Response.TransmitFile(Server.MapPath("\\SignOrder\\ApproverSigned.pdf"));
                            Response.End();
                        }

                    }
                }
            }



            //Response.ContentType = "Application/pdf";
            //Response.AppendHeader("Content-Disposition", "attachment; filename=ApproverSigned.pdf");
            //Response.TransmitFile(Server.MapPath("\\SignOrder\\ApproverSigned.pdf"));
            //Response.End();
        }
    }
}