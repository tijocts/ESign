using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Important
{
    public partial class UploadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
      

        protected void btnUpload_UploadFile(object sender, EventArgs e)
        {
            try
            {
                string fileName = Path.GetFileName(FileUpload.PostedFile.FileName);
                FileUpload.PostedFile.SaveAs(Server.MapPath("~/app_data/")+"UploadedFile.pdf");
              //  FileUpload.PostedFile.SaveAs(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "UploadedFile.pdf"));
                Response.Redirect("AddReceipents.aspx");
            }catch(Exception ex)
            {
                
            }
        }
    }
}