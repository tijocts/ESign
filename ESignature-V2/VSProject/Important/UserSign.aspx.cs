using Ghostscript.NET;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;
using sys = System.Drawing;

namespace Important
{
    public partial class UserSign : System.Web.UI.Page
    {
        public System.Drawing.Point Location { get; set; }



        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //  var list = (List<int>)Session["ControlPosition"];

                var Controlslist = Session["ControlPosition"] as List<PdfControlAndRecipientsDetails>;
                var Recipientlist = Session["RecipientsList"] as List<RecipientsList>;

                try
                {
                    String pathin = Server.MapPath("\\Files\\UploadedFile1.pdf");
                    String pathout = Server.MapPath("\\Files\\UploadedFile2.pdf");                
                    string SignatureUpload = Server.MapPath("\\Images\\signupload.png");
                    string ImageTransparent = Server.MapPath("\\Images\\transparent-png.png");
                    
                    PdfReader reader = new PdfReader(pathin);
                    FileStream os = new FileStream(pathout, FileMode.Create);
                    PdfStamper stamper = new PdfStamper(reader, os);

                    //First Push Buttons
                    //Document doc = new Document();
                    PdfDocument doc = new PdfDocument();
                    PdfWriter writer = PdfWriter.GetInstance(doc, os);
                    Image img = Image.GetInstance(SignatureUpload);
                    float w = img.ScaledWidth;
                    float h = img.ScaledHeight;
                    Rectangle rect = new Rectangle(425.2360876897134f, 779.8690476190476f, 425.2360876897134f + w, 779.8690476190476f + h);
                    PushbuttonField button = new PushbuttonField(
                      stamper.Writer, rect, "Upload"
                    );
                    // button.BackgroundColor = BaseColor.RED;
                    //button.BorderColor = BaseColor.LIGHT_GRAY;
                    button.TextColor = BaseColor.RED;
                    button.FontSize = 9;
                    button.Text = "Signature";
                    button.Image = Image.GetInstance(SignatureUpload);
                    button.Visibility = PushbuttonField.VISIBLE_BUT_DOES_NOT_PRINT;
                    PdfFormField field = button.Field;
                    stamper.AddAnnotation(field, 1);

                    //Second Push Button
                    AcroFields form = stamper.AcroFields;
                    Document doc1 = new Document();
                    PdfWriter writer1 = PdfWriter.GetInstance(doc1, os);
                    Image img1 = Image.GetInstance(SignatureUpload);
                    float w1 = img1.ScaledWidth;
                    float h1 = img1.ScaledHeight;
                    Rectangle rect1 = new Rectangle(431.25632377740305f, 504.2142857142857f, 431.25632377740305f + w1, 504.2142857142857f + h1);
                    PushbuttonField button1 = new PushbuttonField(
                      stamper.Writer, rect1, "Upload"
                    );                    
                    button1.TextColor = BaseColor.RED;
                    button1.FontSize = 9;
                    button1.Text = "Signature.";
                    button1.Image = Image.GetInstance(SignatureUpload);
                    button1.Visibility = PushbuttonField.VISIBLE_BUT_DOES_NOT_PRINT;
                    PdfFormField field1 = button1.Field;
                    field1.Action = PdfAction.JavaScript("app.alert(\"Upload Signature!! !\");", writer1);
                    stamper.AddAnnotation(field1, 1);


                    //Date
                    PdfContentByte cb = stamper.GetOverContent(1);
                    cb.BeginText();
                    cb.SetFontAndSize(iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, false), 9f);
                    cb.MoveText(365.0337268128162f, 785.8833333333333f);
                    cb.SetColorFill(BaseColor.RED);
                    DateTime now = DateTime.Now;
                    cb.ShowText(now.Date.ToString());
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
                    stamper.AddAnnotation(tf.GetTextField(), 1);

                    //Create a Stamp
                    PdfContentByte PDFData = stamper.GetOverContent(1);
                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                    PDFData.BeginText();
                    PDFData.SetColorFill(CMYKColor.RED);
                    PDFData.SetFontAndSize(baseFont, 9);
                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Signature", 425.2360876897134f, 779.8690476190476f, 0);

                    // PDFData.SetAction=PdfAction
                    PDFData.EndText();
                    //Create a Stamp
                    PdfContentByte PDFData1 = stamper.GetOverContent(1);
                    BaseFont baseFont1 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                    PDFData1.BeginText();
                    PDFData1.SetColorFill(CMYKColor.RED);
                    PDFData1.SetFontAndSize(baseFont, 9);
                    PDFData1.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Signature", 431.25632377740305f, 504.2142857142857f, 0);

                    // PDFData.SetAction=PdfAction
                    PDFData1.EndText();


                    //    // We add a submit button to the existing form
                    /* PushbuttonField buttonA = new PushbuttonField(
                        stamper.Writer, new Rectangle(90, 660, 140, 690), "submit"
                      );
                      buttonA.Text = "POST";
                      buttonA.BackgroundColor = new GrayColor(0.7f);
                      buttonA.Visibility = PushbuttonField.VISIBLE_BUT_DOES_NOT_PRINT;
                      PdfFormField submit = buttonA.Field;
                      submit.Action = PdfAction.CreateSubmitForm(
                       SignatureUpload, null, 0
                      );
                      stamper.AddAnnotation(submit, 1);


                     // We add an extra field that can be used to upload a file
                      TextField file = new TextField(
                            stamper.Writer, new Rectangle(160, 660, 470, 690), "image"
                          );
                          file.Options = TextField.FILE_SELECTION;
                          file.BackgroundColor = new GrayColor(0.9f);
                      PdfFormField upload = file.GetTextField();
                          upload.SetAdditionalActions(PdfName.U,
                            PdfAction.JavaScript(
                              "this.getField('image').browseForFileToSubmit();"
                              + "this.getField('submit').setFocus();",
                              stamper.Writer
                            )
                          );
                          stamper.AddAnnotation(upload, 1);*/

                    /* TextField file1 = new TextField(stamper.Writer, new Rectangle(160, 660, 470, 690), "myfile");
                     file1.Options = TextField.FILE_SELECTION;
                    file1.BackgroundColor= new GrayColor(0.9f);
                    PdfFormField upload1 = file1.GetTextField();
                     upload1.SetAdditionalActions(PdfName.U, PdfAction.JavaScript(
                                 "this.getField('myfile').browseForFileToSubmit();", writer));
                     stamper.AddAnnotation(upload1,1);
                     */
                    //string pdfFilePath = pathout;
                    //byte[] bytes = System.IO.File.ReadAllBytes(pdfFilePath);
                    // DoGet(bytes, os);





                    // os.Close();
                    //reader.Close();   
                    stamper.Close();
                    //Get all the text inside the Pdfs
                    PdfReader ReaderFindText = new PdfReader(Server.MapPath("\\Files\\UploadedFile2.pdf"));

                    System.Text.StringBuilder builder = new System.Text.StringBuilder();
                    for (int i = 1; i <= ReaderFindText.NumberOfPages; i++)
                    {
                        builder.Append(PdfTextExtractor.GetTextFromPage(ReaderFindText, i));
                        builder.Append(' ');
                    }
                    string pdfText= builder.ToString();
                    //string text = PdfTextExtractor.GetTextFromPage(reader, 1);
                   
                   // Pdfembed.Src = "Files/UploadedFile2.pdf#toolbar=0&navpanes=0&scrollbar=0";

                   


                   /* Bitmap objBitmap;
                    Graphics objGraphics;
                    objBitmap = new Bitmap(400, 440);
                    objGraphics = Graphics.FromImage(objBitmap);
                    objGraphics.Clear(Color.Red);
                    Pen p = new Pen(Color.Yellow, 0);
                    System.Drawing.Rectangle rectA = new System.Drawing.Rectangle(566, 1039, 20, 20);
                    objGraphics.DrawEllipse(p, rectA);

                    Brush b1 = new SolidBrush(Color.Red);
                    Brush b2 = new SolidBrush(Color.Green);
                    Brush b3 = new SolidBrush(Color.Blue);
                    objGraphics.FillPie(b1, rectA, 0f, 60f);
                    objGraphics.FillPie(b2, rectA, 60f, 150f);
                    objGraphics.FillPie(b3, rectA, 210f, 150f);
                    objBitmap.Dispose();
                    objGraphics.Dispose();
                    */





                  
                 //  Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "MyFunction()", true);
                    


                }
                catch (Exception ex)
                {   

                }
            }
        }


        public void LoadImage(string InputPDFFile, int PageNumber)
        {
            try
            {
                string outImageName = InputPDFFile;
                outImageName = outImageName + "_" + PageNumber.ToString() + "_.png";


                GhostscriptPngDevice dev = new GhostscriptPngDevice(GhostscriptPngDeviceType.Png256);
                dev.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
                dev.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
                dev.ResolutionXY = new GhostscriptImageDeviceResolution(290, 290);
                dev.InputFiles.Add(InputPDFFile);
                dev.Pdf.FirstPage = PageNumber;
                dev.Pdf.LastPage = PageNumber;
                dev.CustomSwitches.Add("-dDOINTERPOLATE");
                string Imgae = "\\Images\\" + outImageName;
                dev.OutputPath = Server.MapPath("\\Images\\"+ Imgae);
                dev.Process();
            }
            catch(Exception ex)
            {

            }

        }

        protected void DoGet(byte[] pdf, Stream stream)
        {
            // We get a resource from our web app
            PdfReader reader = new PdfReader(pdf);
            // Now we create the PDF
            using (PdfStamper stamper = new PdfStamper(reader, stream))
            {
                // We add a submit button to the existing form
                PushbuttonField button = new PushbuttonField(
                  stamper.Writer, new Rectangle(90, 660, 140, 690), "submit"
                );
                button.Text = "POST";
                button.BackgroundColor = new GrayColor(0.7f);
                button.Visibility = PushbuttonField.VISIBLE_BUT_DOES_NOT_PRINT;
                PdfFormField submit = button.Field;
                submit.Action = PdfAction.CreateSubmitForm(
                  Server.MapPath("\\Images\\"), null, 0
                );
                stamper.AddAnnotation(submit, 1);
                // We add an extra field that can be used to upload a file
                TextField file = new TextField(
                  stamper.Writer, new Rectangle(160, 660, 470, 690), "image"
                );
                file.Options = TextField.FILE_SELECTION;
                file.BackgroundColor = new GrayColor(0.9f);
                PdfFormField upload = file.GetTextField();
                upload.SetAdditionalActions(PdfName.U,
                  PdfAction.JavaScript(
                    "this.getField('image').browseForFileToSubmit();"
                    + "this.getField('submit').setFocus();",
                    stamper.Writer
                  )
                );
                stamper.AddAnnotation(upload, 1);
            }
        }
            public void Manipulate()
        {
            String src = Server.MapPath("\\Files\\UploadedFile1.pdf");
            PdfReader reader = new PdfReader(src);
            int n = reader.NumberOfPages;
            using (MemoryStream ms = new MemoryStream())
            {
                // Create a stamper
                using (PdfStamper stamper = new PdfStamper(reader, ms))
                {
                    // Create pushbutton 1
                    PushbuttonField saveAs = new PushbuttonField(
                      stamper.Writer,
                      new Rectangle(160, 660, 470, 690),
                      "Save"
                    );
                    stamper.FormFlattening = true;
                    saveAs.BorderColor = BaseColor.BLACK;
                    saveAs.Text = "Save";
                    saveAs.TextColor = BaseColor.RED;
                    saveAs.Layout = PushbuttonField.LAYOUT_LABEL_ONLY;
                    saveAs.Rotation = 90;
                    PdfAnnotation saveAsButton = saveAs.Field;
                    saveAsButton.Action = PdfAction.JavaScript(
                      "app.execMenuItem('SaveAs')", stamper.Writer
                    );
                    // Create pushbutton 2
                    PushbuttonField mail = new PushbuttonField(
                      stamper.Writer,
                      new Rectangle(736, 10, 816, 30),
                      "Mail"
                    );
                    mail.BorderColor = BaseColor.BLACK;
                    mail.Text = "Mail";
                    mail.TextColor = BaseColor.RED;
                    mail.Layout = PushbuttonField.LAYOUT_LABEL_ONLY;
                    mail.Rotation = 90;
                    PdfAnnotation mailButton = mail.Field;
                    mailButton.Action = PdfAction.JavaScript(
                      "app.execMenuItem('AcroSendMail:SendMail')",
                      stamper.Writer
                    );
                    // Add the annotations to every page of the document
                    for (int page = 1; page <= n; page++)
                    {
                        stamper.AddAnnotation(saveAsButton, page);
                        stamper.AddAnnotation(mailButton, page);
                    }
                    stamper.Close();

                }

            }
        }
        public void ManipulatePdf(byte[] src,string SignatureUpload,PdfReader RESOURCE)
        {
            // Create a reader for the original document
            PdfReader reader = new PdfReader(src);
            // Create a reader for the advertisement resource
            PdfReader ad = new PdfReader(RESOURCE);
            using (MemoryStream ms = new MemoryStream())
            {
                // Create a stamper
                using (PdfStamper stamper = new PdfStamper(reader, ms))
                {
                    // Create the advertisement annotation for the menubar
                    Rectangle rect = new Rectangle(90, 660, 140, 690);
                    PushbuttonField button = new PushbuttonField(
                      stamper.Writer, rect, "click"
                    );
                    button.BackgroundColor = BaseColor.RED;
                    button.BorderColor = BaseColor.RED;
                    button.FontSize = 10;
                    button.Text = "Close this advertisement";
                    button.Image = Image.GetInstance(SignatureUpload);
                    button.Layout = PushbuttonField.LAYOUT_LABEL_LEFT_ICON_RIGHT;
                    button.IconHorizontalAdjustment = 1;
                    PdfFormField menubar = button.Field;
                    String js = "var f1 = getField('click'); f1.display = display.hidden;"
                      + "var f2 = getField('advertisement'); f2.display = display.hidden;"
                    ;
                    menubar.Action = PdfAction.JavaScript(js, stamper.Writer);
                    // Add the annotation
                    stamper.AddAnnotation(menubar, 1);
                    // Create the advertisement annotation for the content
                    rect = new Rectangle(290, 660, 140, 690);
                    button = new PushbuttonField(
                      stamper.Writer, rect, "advertisement"
                    );
                    button.BackgroundColor = BaseColor.WHITE;
                    button.BorderColor = BaseColor.RED;
                    button.Text = "Buy the book iText in Action 2nd edition";
                    button.Template = stamper.GetImportedPage(ad, 1);
                    button.Layout = PushbuttonField.LAYOUT_ICON_TOP_LABEL_BOTTOM;
                    PdfFormField advertisement = button.Field;
                    advertisement.Action = new PdfAction(
                      "http://www.1t3xt.com/docs/book.php"
                    );
                    // Add the annotation
                    stamper.AddAnnotation(advertisement, 1);
                    stamper.Close();
                }
               
               // return ms.ToArray();
            }
        }
        public static PdfAnnotation createPopup(PdfWriter writer, Rectangle rect, String contents, Boolean open)
        {
            PdfAnnotation annot = new PdfAnnotation(writer, rect);
            annot.Put(PdfName.SUBTYPE, PdfName.POPUP);
            if (contents != null)
                annot.Put(PdfName.CONTENTS, new PdfString(contents, PdfObject.TEXT_UNICODE));
            if (open)
                annot.Put(PdfName.OPEN, PdfBoolean.PDFTRUE);
            return annot;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {

            String pathin = Server.MapPath("\\Files\\UploadedFile1.pdf");
            String pathout = Server.MapPath("\\Files\\UploadedFile2.pdf");
            string SignatureImagePath = Server.MapPath("\\Images\\signsmall.png");
            string SignatureUpload = Server.MapPath("\\Images\\signupload.png");
            PdfReader reader = new PdfReader(pathin);
            FileStream os = new FileStream(pathout, FileMode.Create);
            PdfStamper stamper = new PdfStamper(reader, os);
            //First Push Buttons
           /* Document doc = new Document();
            //  doc.Open();
            PdfWriter writer = PdfWriter.GetInstance(doc, os);
            Image img = Image.GetInstance(SignatureUpload);
            float w = img.ScaledWidth;
            float h = img.ScaledHeight;
            Rectangle rect = new Rectangle(425.2360876897134f, 779.8690476190476f, 425.2360876897134f + w, 779.8690476190476f + h);
            PushbuttonField button = new PushbuttonField(
              stamper.Writer, rect, "Upload"
            );
            // button.BackgroundColor = BaseColor.RED;
            //button.BorderColor = BaseColor.LIGHT_GRAY;
            button.TextColor = BaseColor.RED;
            button.FontSize = 9;
            button.Text = "Signature";
            button.Image = Image.GetInstance(SignatureUpload);
            button.Visibility = PushbuttonField.VISIBLE_BUT_DOES_NOT_PRINT;
            PdfFormField field = button.Field;
            field.Action = PdfAction.JavaScript("app.alert(\"Upload Signature !\");", writer);
            stamper.AddAnnotation(field, 1);

            //Second Push Button
            AcroFields form = stamper.AcroFields;
            Document doc1 = new Document();
            PdfWriter writer1 = PdfWriter.GetInstance(doc1, os);
            Image img1 = Image.GetInstance(SignatureUpload);
            float w1 = img1.ScaledWidth;
            float h1 = img1.ScaledHeight;
            Rectangle rect1 = new Rectangle(431.25632377740305f, 504.2142857142857f, 431.25632377740305f + w1, 504.2142857142857f + h1);
            PushbuttonField button1 = new PushbuttonField(
              stamper.Writer, rect1, "Upload"
            );
            //button1.BorderColor = BaseColor.LIGHT_GRAY;
            button1.TextColor = BaseColor.RED;
            button1.FontSize = 9;
            button1.Text = "Signature.";
            button1.Image = Image.GetInstance(SignatureUpload);
            button1.Visibility = PushbuttonField.VISIBLE_BUT_DOES_NOT_PRINT;
            PdfFormField field1 = button1.Field;
            // button1.Click += new EventHandler(button1_Click);
            field1.Action = PdfAction.JavaScript("app.alert(\"Upload Signature!! !\");", writer1);
            //field1.Action = PdfAction.JavaScript("eApp.Page1.FormPurpose_PolicyNumber.rawValue = 'hello'", writer1);

            stamper.AddAnnotation(field1, 1);*/

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
            stamper.AddAnnotation(tf.GetTextField(), 1);

            //Date
            PdfContentByte cb = stamper.GetOverContent(1);
            cb.BeginText();
            cb.SetFontAndSize(iTextSharp.text.pdf.BaseFont.CreateFont(iTextSharp.text.pdf.BaseFont.HELVETICA, iTextSharp.text.pdf.BaseFont.CP1250, false), 9f);
            cb.MoveText(365.0337268128162f, 785.8833333333333f);
            cb.SetColorFill(BaseColor.RED);
            DateTime now = DateTime.Now;
            cb.ShowText(now.Date.ToString());
            cb.EndText();

            foreach (var item in chklist.Items)
            {
                //Signature Mapping
                if (chklist.Items[0].Selected == true)
                {

                    Image imgA = Image.GetInstance(SignatureImagePath);
                    imgA.SetAbsolutePosition(425.2360876897134f, 779.8690476190476f);
                    stamper.GetOverContent(1).AddImage(imgA);
                }
                if (chklist.Items[1].Selected == true)
                {

                    Image imgB = Image.GetInstance(SignatureImagePath);
                    imgB.SetAbsolutePosition(431.25632377740305f, 504.2142857142857f);
                    stamper.GetOverContent(1).AddImage(imgB);
                }
                break;

            }



            stamper.Close();
            //Pdfembed.Src = "Files/UploadedFile2.pdf#toolbar=0&navpanes=0&scrollbar=0";
        }
    }
}

        