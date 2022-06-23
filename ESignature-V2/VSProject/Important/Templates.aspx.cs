using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Important
{
    public partial class Templates : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        
            protected void Panel2_Load(object sender, EventArgs e)
        {
           
            string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            String assemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

            Process proc = new Process();
            //proc.
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = Server.MapPath("~/app_data/UploadedFile.pdf")
                // assemblyPath+ "\\app_data\\UploadedFile.pdf"
            };
            proc.Start();
           // axAcroPDF.src = ofd.FileName;
        }
    }
}