using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Important
{
    public partial class DragAndDrop : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
           // HttpContext.Current.Session["Receipent"] = "2";

            /*string path = Server.MapPath("\\Files\\aspose.pdf");
            using (var converter = new Aspose.Pdf.Facades.PdfConverter())
            {
                converter.BindPdf(path);

                // initiate conversion
                converter.DoConvert();

                // create TiffSettings & set compression type
                var settings = new Aspose.Pdf.Devices.TiffSettings()
                {
                    Compression = Aspose.Pdf.Devices.CompressionType.CCITT4,
                };
                // save PDF as TIFF
                string pathout = Server.MapPath("\\Files\\");
                converter.SaveAsTIFF(pathout + "output.png", settings);
            }*/

        }

        protected void btn_Click(object sender, EventArgs e)
        {

        }

        //protected void rdoReceipents_SelectedIndexChanged(Object sender, EventArgs e)
        //{//0:First receipent, 1:second receipent
        //     hdnSelectedReceipentValue.Value = "";
        //    if (rdoReceipents.SelectedItem.Text == "0")
        //        hdnSelectedReceipentValue.Value = "0";
        //    else
        //        hdnSelectedReceipentValue.Value = "1";
        //}
    }
}