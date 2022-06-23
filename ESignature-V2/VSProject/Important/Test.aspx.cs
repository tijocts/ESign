using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
//using Syncfusion.Pdf.Graphics;
using Label = ceTe.DynamicPDF.PageElements.Label;
using Page = ceTe.DynamicPDF.Page;

namespace Important
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            FileStream stream = new FileStream(Server.MapPath("~/app_data/roopaps.pdf"), FileMode.Open);

            //Initialize PDF Viewer

           // PdfViewerControl pdfViewerControl1 = new PdfViewerControl();



            /* Document document = new Document();
             // Create a Page and add it to the document
             Page page = new Page();
             document.Pages.Add(page);
             // Add a label to the page 
             page.Elements.Add(new Label("My PDF Document", 0, 0, 512, 40, iTextSharp.text.Font.Helvetica, 30, ceTe.DynamicPDF.TextAlign.Center));
             // Save the PDF document
             document.Draw(Server.MapPath("~/app_data/NewlyCreatedFile.pdf"));*/

            //Load a PDF document.
            System.Net.WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(Server.MapPath("~/app_data/roopaps.pdf"));
            using (PdfLoadedDocument doc = new PdfLoadedDocument(FileBuffer))
            {

                //Get first page from document
                PdfLoadedPage page = doc.Pages[0] as PdfLoadedPage;

                //Create PDF graphics for the page
                PdfGraphics graphics = page.Graphics;

                //Set the standard font.
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);

                //Draw the text.
                graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new PointF(0, 0));

                //Save the document.
                // var s = Server.MapPath("~/app_data/roopaps.pdf");
               // doc.Save();// "output.pdf", HttpContext.Current.ApplicationInstance.Response, HttpReadType.Save);

                //Close the document.
                doc.Close(true);
            }
        }
    }
}