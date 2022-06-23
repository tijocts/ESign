using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;

namespace Important
{
    public partial class UserSign2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //  var list = (List<int>)Session["ControlPosition"];
            try
            {
                var Controlslist = Session["ControlPosition"] as List<PdfControlAndRecipientsDetails>;
                var Recipientlist = Session["RecipientsList"] as List<RecipientsList>;

                String pathin = Server.MapPath("\\Files\\UploadedFile1.pdf");
                String pathout = Server.MapPath("\\Files\\UploadedFile2.pdf");
                string SignatureImagePath = Server.MapPath("\\Images\\signsmall.png");
                string SignatureUpload = Server.MapPath("\\Images\\signupload.png");
                FileStream os = new FileStream(pathout, FileMode.Create);
                PdfReader reader = new PdfReader(pathin);
                PdfStamper stamper=new PdfStamper(reader, os);
                PdfReader redr;
                float PositionX;
                float PositionY;
                var length = Controlslist.Count;
                var count = 0;
                foreach (var item in Controlslist)
                {
                    redr = new PdfReader(pathin);
                    //stamper = new PdfStamper(redr, os);
                   
                    
                    //PdfCopy copy = new PdfCopy(doc, os);
                    if (count < length)
                    {                        
                       // doc.Open();
                        PositionX = float.Parse(item.PositionX, CultureInfo.InvariantCulture.NumberFormat);
                        PositionY = float.Parse(item.PositionY, CultureInfo.InvariantCulture.NumberFormat);
                        if (item.ControlName.Trim().Replace("\"", "").Trim() == "Signature")
                        {
                            //Create Button-----------------------------------------------------------------------

                            Document doc = new Document();
                            //doc.NewPage();
                            PdfWriter writer = PdfWriter.GetInstance(doc, os); 
                            Image img = Image.GetInstance(SignatureUpload);
                            float w = img.ScaledWidth;
                            float h = img.ScaledHeight;
                            Rectangle rect = new Rectangle(PositionX+'f', PositionY+'f', PositionX + 'f' + w, PositionY + 'f' + h);
                            PushbuttonField button = new PushbuttonField(
                              stamper.Writer, rect, "Upload"
                            );
                           
                            button.BorderColor = BaseColor.LIGHT_GRAY;
                            button.TextColor = BaseColor.RED;
                            button.FontSize = 9;
                            button.Text = "Signature";
                            button.Image = Image.GetInstance(SignatureUpload);
                            PdfFormField field = button.Field;
                            field.Action = PdfAction.JavaScript("app.alert(\"Upload Signature !\");", writer);                            
                            stamper.AddAnnotation(field, 1);
                           

                        }
                        if (item.ControlName.Trim().Replace("\"", "").Trim() == "Name")
                        {
                            //Create TextBox-----------------------------------------------------------------------
                            //redr = new PdfReader(pathin);
                            //stamper = new PdfStamper(redr, os);
                            var bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                            var tf = new TextField(stamper.Writer, new Rectangle(PositionX + 'f' - 9f, PositionY + 'f' - 9f, PositionX + 'f' + 50f, PositionY + 'f' + 9f), "txt")
                            {

                                BorderColor = GrayColor.GRAYBLACK,
                                FontSize = 9,
                                Text = item.Color.Trim().Replace("\"", "").Trim() == "Red" ? "Sam Joseph" : "David",
                                Font = bf


                            };

                            stamper.AddAnnotation(tf.GetTextField(), 1);
                          

                        }
                        if (item.ControlName.Trim().Replace("\"", "").Trim() == "Date")
                        {
                            //Create Label-------------------------------------------------------------------
                           // redr = new PdfReader(pathin);
                            //stamper = new PdfStamper(redr, os);
                            DateTime now = DateTime.Now;
                            PdfContentByte cb = stamper.GetOverContent(1);
                            cb.BeginText();
                            cb.SetFontAndSize(iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, false), 9f);
                            cb.MoveText(PositionX + 'f', PositionY + 'f');
                            cb.ShowText(now.Date.ToString());
                            cb.EndText();                           
                          
                        }
                        count++;
                        redr.Close();
                       
                    }
                  
                    stamper.Close();
                

                }
                
              
               // Pdfembed.Src = "Files/UploadedFile2.pdf#toolbar=0&navpanes=0&scrollbar=0";
            }
            catch(Exception ex)
            {

            }

                  /*  return;
                    try
                    {
                       

                        //Push Buttons
                        

                        Document doc1 = new Document();
                        PdfWriter writer1 = PdfWriter.GetInstance(doc1, os);
                        Image img1 = Image.GetInstance(SignatureUpload);
                        float w1 = img1.ScaledWidth;
                        float h1 = img1.ScaledHeight;
                        Rectangle rect1 = new Rectangle(431.25632377740305f, 504.2142857142857f, 431.25632377740305f + w1, 504.2142857142857f + h1);
                        PushbuttonField button1 = new PushbuttonField(
                          stamper.Writer, rect1, "Upload"
                        );
                        // button.BackgroundColor = BaseColor.RED;
                        button1.BorderColor = BaseColor.LIGHT_GRAY;
                        button1.TextColor = BaseColor.RED;
                        button1.FontSize = 9;
                        button1.Text = "Signature";
                        button1.Image = Image.GetInstance(SignatureUpload);
                        PdfFormField field1 = button1.Field;
                        field1.Action = PdfAction.JavaScript("app.alert(\"Upload Signature !\");", writer1);
                        stamper.AddAnnotation(field1, 1);

                        PdfContentByte cb = stamper.GetOverContent(1);
                        cb.BeginText();
                        cb.SetFontAndSize(iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, false), 9f);
                        cb.MoveText(365.0337268128162f, 785.8833333333333f);
                        cb.ShowText("Sam Joseph");
                        cb.EndText();

                        //Textbox
                        var bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                        var tf = new TextField(stamper.Writer, new Rectangle(178.40640809443505f - 9f, 498.2f - 9f, 178.40640809443505f + 50f, 498.2f + 9f), "image")
                        {

                            BorderColor = GrayColor.GRAYBLACK,
                            FontSize = 9,
                            // BorderWidth=0.5f,
                            Text = "Sam Joseph",
                            Font = bf


                        };

                       
                        stamper.Close();
                        
                    }
                    catch (Exception ex)
                    {

                    }*/
                }

            }

        }