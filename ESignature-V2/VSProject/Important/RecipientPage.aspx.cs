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

namespace Important
{
    public partial class RecipientPage : System.Web.UI.Page
    {
        string details;
        private Org.BouncyCastle.X509.X509Certificate[] chain;
        private List<X509Certificate> certificates = new List<X509Certificate>();
        public AsymmetricKeyParameter akp;
        private MetaData metadata;
        private static readonly int CER_STATUS_NOT_VERIFIED = 25;
        public Signature SignatureData { set; get; }
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            if (!IsPostBack)
            {
                try
                {
                    Byte[] bytes = File.ReadAllBytes(Server.MapPath("\\Files\\SignDocument.pdf"));
                    String file = Convert.ToBase64String(bytes);
                    pdfBase64.Value = file;
                    var Controlslist = Session["ControlPosition"] as List<PdfControlAndRecipientsDetails>;
                    var Recipientlist = Session["RecipientsList"] as List<RecipientsList>;
                    StringBuilder ControlsArray = new StringBuilder();
                     if (Controlslist != null && Request.QueryString["Color"] != null)
                    {
                        if (Controlslist.Count > 0)
                        {

                            for (int i = 0; i < Controlslist.Count; i++)
                            {
                                //Red color for Recipient: First Recipients
                                if (Controlslist[i].Color.Replace('"', ' ').Trim() == Request.QueryString["Color"] )
                                {
                                    ControlsArray.Append(Controlslist[i].ControlName.Replace('"', ' ').Trim());
                                    ControlsArray.Append(",");
                                    ControlsArray.Append(Controlslist[i].PositionX.Replace('"', ' ').Trim());
                                    ControlsArray.Append(",");
                                    ControlsArray.Append(Controlslist[i].PositionY.Replace('"', ' ').Trim());
                                    ControlsArray.Append(",");
                                    ControlsArray.Append(Controlslist[i].X.Replace('"', ' ').Trim());
                                    ControlsArray.Append(",");
                                    ControlsArray.Append(Controlslist[i].Y.Replace('"', ' ').Trim());
                                    ControlsArray.Append(":");
                                }

                            }
                            Session["ControlsArray"] = ControlsArray;

                            //Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "SessionValues("+Controlslist+","+Recipientlist+")", true);
                            //Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", string.Format("SessionValues('{0}');", ControlsArray), true);

                        }

                        for (int i = 0; i < Recipientlist.Count; i++)
                        {
                            if (Recipientlist[i].Color == Request.QueryString["Color"])
                            {
                                lblRecipientName.Text = Recipientlist[i].Name;
                                lblRecipientEmail.Text = Recipientlist[i].Email;
                            }
                        }
                        
                      Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", string.Format("SessionValues('{0}');", lblRecipientName.Text+"*"+ControlsArray), true);

                    }
                    else
                    {
                        Response.Redirect("AddReceipents.aspx");
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        protected void btn_Click(object sender, EventArgs e)
        {
            try
            {
                //Final Save and Mail

                String pathin = Server.MapPath("\\Files\\SignDocument.pdf");             
                String pathout = Server.MapPath("\\Files\\SignDocument_Controls.pdf");
                string SignatureUpload = Server.MapPath("\\Images\\signupload.png");
                var GivenControls = Session["ControlsArray"].ToString();
                PdfReader reader = new PdfReader(pathin);

                using (FileStream os = new FileStream(pathout, FileMode.OpenOrCreate))
                {
                    PdfStamper stamper = new PdfStamper(reader, os);
                    
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

                    //Digital Signature Mapping
                    pdfBase64.Value = "";
                    Byte[] pdfout;
                    if (hdnSignImage.Value.Length != 0)
                    {
                        DigitalSignatureMapping(GivenControls);
                        //pdfout = File.ReadAllBytes(Server.MapPath("\\Files\\SignDocument_Signed.pdf"));


                    }
                    else
                    {
                        pdfout = File.ReadAllBytes(Server.MapPath("\\Files\\SignDocument_Controls.pdf"));
                    }

                    //String fileout = Convert.ToBase64String(pdfout);
                    //pdfBase64.Value = fileout;
                
              



                reader.Close();
                

            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();

            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                
            }


        }
        private bool preClosed = false;
        public bool isPreClosed()
        {
            return preClosed;
        }
        public void DigitalSignatureMapping(string GivenControls)
        {
            try
            {
                string SrcFile = Server.MapPath("\\Files\\SignDocument_Controls.pdf");
                string DestFile = Server.MapPath("\\Files\\SignDocument_Signed.pdf");
                // string SrcCertificate = Server.MapPath("\\Files\\" + fileCertificate.PostedFile.FileName);
                string SrcCertificate = Server.MapPath("\\Files\\PDF.pfx");
                string CertificatePAssword = txtCertificatePassword.Text;
                string publicKeyStreamFile = Server.MapPath("\\Files\\PDF.cer");
                var pk = akp;
                lblMessage.Visible = false;
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
                    //FileStream os = new FileStream(DestFile, FileMode.Create);
                    // PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
                    //PdfReader Forloopreader;
                    byte[] bytes = Convert.FromBase64String(hdnSignImage.Value.Substring(hdnSignImage.Value.LastIndexOf(',') + 1));

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
                    Guid obj = Guid.NewGuid();
                    using (FileStream os = new FileStream(DestFile, FileMode.OpenOrCreate))
                    {
                        try
                        {
                            using (PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0', null, true))
                            {

                                PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                                for (var j = 0; j < len; j++)
                                {
                                    //Get Pdf Positions
                                    var PdfXPosition = Convert.ToSingle(finalString[j].Split(',')[1].Trim().Replace('"', ' ').Trim()) - .00220f;
                                    var PdfYPosition = Convert.ToSingle(finalString[j].Split(',')[2].Trim().Replace('"', ' ').Trim()) - 33.00000f;
                                    //Forloopreader = new PdfReader(SrcFile);

                                    Image Sign = Image.GetInstance(SignatureImagePath);
                                    Sign.SetAbsolutePosition(PdfXPosition, PdfYPosition);
                                    Sign.Alignment = Element.ALIGN_LEFT;
                                    stamper.GetOverContent(1).AddImage(Sign);


                                    // stamper.GetOverContent(2).AddImage(Sign);


                                    // Creating the appearance
                                    //appearance = stamper.SignatureAppearance;

                                    // appearance.SignDate = System.DateTime.Now;
                                    //appearance.Acro6Layers = true;
                                    //appearance.Image = image;
                                    // appearance.Reason = "Digitally Sign";
                                    appearance.CertificationLevel = PdfSignatureAppearance.CERTIFIED_NO_CHANGES_ALLOWED;
                                    //appearance.c(false);
                                    //For Invisible Signature make Height and Width to zero
                                    appearance.SetVisibleSignature(new Rectangle(0, 0, 0, 0), 1, "Sign" + obj);
                                    //appearance.SetVisibleSignature(new Rectangle(PdfXPosition, PdfYPosition, PdfXPosition + w1, PdfYPosition + h1), 2, "Sign1" + obj);

                                    if (appearance.IsPreClosed())
                                    {

                                    }


                                }

                                IExternalSignature externalSignature = new PrivateKeySignature(akp, DigestAlgorithms.SHA512);
                                MakeSignatures.SignDetachedd(appearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);
                                // MakeSignature.SignDetached(appearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                        //stamper.Writer.FreeReader(Forloopreader);
                        //Forloopreader.Close();
                        // stamper.Close();
                        os.Close();
                        reader.Close();
                    }
                    //reader.Close();


                    ////For Verify/ Validate all Digital Signatures
                    /*  using (PdfReader destinationreader = new PdfReader((DestFile)))
                      {
                          AcroFields acroFields = destinationreader.AcroFields;
                          List<String> names = acroFields.GetSignatureNames();
                          bool Valid = false;
                          string LogFilePath = Server.MapPath("\\Files\\Log.txt");
                          StreamWriter writer = new StreamWriter(LogFilePath);
                          string NameoFSignatureValid = string.Empty;
                          details = string.Empty;
                          X509Certificate[] certs;
                          PdfPKCS7 pdfPkcs7;
                          foreach (string name in names)
                          {
                              pdfPkcs7 = acroFields.VerifySignature(name);
                              Valid = pdfPkcs7.Verify();
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
                                  return;
                              }

                          }
                          //To Validate
                          if (Valid)
                          {

                              string MailStatus = SendMail();
                              if (MailStatus == "1")
                              {
                                  lblMessage.Text = "Document successfully Mailed and Uploaded to Folder (Files/SignDocument_Signed.pdf).";
                                  lblMessage.Visible = true;
                              }
                          }
                          destinationreader.Close();
                      }
                     */
                    string MailStatus = SendMail();
                    if (MailStatus == "1")
                    {
                        lblMessage.Text = "Document successfully Mailed and Uploaded to Folder (Files/SignDocument_Signed.pdf).";
                        lblMessage.Visible = true;
                    }
                    reader.Close();
                    
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;//"Error Occured in Digital Signature Function.";
                lblMessage.Visible = true;
            }
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
        private MetaData AddMetaData(PdfStamper stamper)
        {
            //Adding Meta Datas
            MetaData MyMD = new MetaData();
            MyMD.Author = "Sam.joseph@claysys.in";
            MyMD.Title = "Digital Signature Document";
            MyMD.Subject = "eSignature";
            MyMD.Creator = "Claysys";
            MyMD.Producer = "sam.josph";
            return MyMD;
            // metadata = MyMD;
            //stamper.MoreInfo = metadata.getMetaData();
            // stamper.XmpMetadata = this.metadata.getStreamedMetaData();
        }
        public class MetaData
        {
            public Hashtable info = new Hashtable();

            public Hashtable Info
            {
                get { return info; }
                set { info = value; }
            }

            public string Author
            {
                get { return (string)info["Author"]; }
                set { info.Add("Author", value); }
            }
            public string Title
            {
                get { return (string)info["Title"]; }
                set { info.Add("Title", value); }
            }
            public string Subject
            {
                get { return (string)info["Subject"]; }
                set { info.Add("Subject", value); }
            }
            public string Keywords
            {
                get { return (string)info["Keywords"]; }
                set { info.Add("Keywords", value); }
            }
            public string Producer
            {
                get { return (string)info["Producer"]; }
                set { info.Add("Producer", value); }
            }

            public string Creator
            {
                get { return (string)info["Creator"]; }
                set { info.Add("Creator", value); }
            }

            public Hashtable getMetaData()
            {
                return this.info;
            }
            public byte[] getStreamedMetaData()
            {
                MemoryStream os = new System.IO.MemoryStream();
                XmpWriter xmp = new XmpWriter(os, (System.Collections.Generic.IDictionary<string, string>)info);
                xmp.Close();
                return os.ToArray();
            }

        }
        public string SendMail()
        {

            var Status="1"; 
            string to = "roopa.prabha@claysys.in"; //lbEmailAddress.Text;//HR To address    
            string from = lblRecipientEmail.Text;//"roopa.claysys@gmail.com"; //From address    
            using (System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(from, to))
            {

                string mailbody = GetEmailHtmlBody();
                message.Subject = "Signed Document";
                message.Body = mailbody;
                message.CC.Add(new MailAddress("roopa.prabha@claysys.in"));//lblRecipientEmail.Text;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Attachments.Add(new Attachment(Server.MapPath("\\Files\\SignDocument_Signed.pdf")));
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
                System.Net.NetworkCredential basicCredential1 = new
                System.Net.NetworkCredential("roopa.claysys@gmail.com", "roopatijo@123");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = basicCredential1;
                try
                {
                    client.Send(message);


                    RecipientListBindPage();
                }

                catch (Exception ex)
                {
                    Status = "0";
                    throw ex;
                }
            }
            return Status;
        }

        public string GetEmailHtmlBody()
        {
            StringBuilder HtmlTextContent = null;
            try
            {


                HtmlTextContent = new StringBuilder();
                HtmlTextContent.Append("<html><body><div style='background-color:#eaeaea;padding:2%;font-family:Helvetica,Arial,Sans Serif'><table cellspacing='0' cellpadding='0' align='center' width='100%'><tbody><tr><td></td><td><table style='border-collapse:collapse;background-color:#ffffff;max-width:640px'><tbody><tr><td style='padding:10px 24px;font-weight:bold;'><h3>ESign</h3></td></tr><tr><td style='padding: 0px 24px 30px 24px'><table align='center' style='background-color:#1e4ca1;color:#ffffff' width='100%'><tbody><tr><td align='center' style='tyle='padding - top:24px; font - size:16px; font - family:Helvetica,Arial,Sans Serif; border: none; text - align:center; color:#ffffff'>Your Document has beed Completed.</td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></div></body></html>");


            }
            catch (Exception ex)
            {
                //Logging.Error("Error while sending the results mail. Error {0}", ex);
                //throw;
            }
            return HtmlTextContent.ToString();
        }
        private void RecipientListBindPage()
        {
            try
            {

                string Id = "0";
                System.Data.DataTable dt = (System.Data.DataTable)Session["RecipientesList"];
                if (Request.QueryString["Color"] == "Red") Id = "1";
                else if (Request.QueryString["Color"] == "Blue") Id = "2";
                List<RecipientsList> list = new List<RecipientsList>();
                list = (from DataRow row in dt.Rows

                        select new RecipientsList()
                        {
                            Name = row["Name"].ToString(),
                            Email = row["Email"].ToString(),
                            Id = row["Id"].ToString()
                        }).Where(x => x.Id != Id).ToList();
                System.Data.DataTable dtTable;
                dtTable = new System.Data.DataTable();
                dtTable.Columns.Add("Id", typeof(string));
                dtTable.Columns.Add("Name", typeof(string));
                dtTable.Columns.Add("Email", typeof(string));
                foreach (var item in list)
                {
                    dtTable.Rows.Add(item.Id, item.Name, item.Email);
                }

                // var Recipientlist =  Session["RecipientesList"] as List<RecipientsList>;
                // Recipientlist.RemoveAll(x => x.Color == Request.QueryString["Color"]);
                Session["RecipientesList"] = dtTable;
                Response.Redirect("RecipientRedirectPage.aspx");
            }
           
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();

            }
            catch (Exception ex)
            {

            }
        }

    }
}   