using Org.BouncyCastle.Pkcs;
using System;
using System.Collections;
using System.IO;
using iTextSharp.text.xml.xmp;
using Org.BouncyCastle.Crypto;
using iTextSharp.text.pdf;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Ocsp;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Tls;
//using Microsoft.Extensions.Logging;

//using iText.Signatures;
//using System.Globalization;

namespace Important
{
    //public AsymmetricKeyParameter GetKey();
    public partial class DigitalSignature : System.Web.UI.Page
    {
        //private static Logger LOGGER = LoggerFactory.getLogger(Important.class);
        string details;
        private Org.BouncyCastle.X509.X509Certificate[] chain;
        private List<X509Certificate> certificates = new List<X509Certificate>();
        public AsymmetricKeyParameter akp;
        private MetaData metadata;
        private static readonly int CER_STATUS_NOT_VERIFIED = 25;
        public Signature SignatureData { set; get; }
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
            {
            string Message = string.Empty;
            lblMessage.Text = string.Empty;
            lblMessage.Visible = false;
            MetaData md;
            try
            {
                lblMessage.Visible = false;
                string SrcFile = Server.MapPath("\\Files\\SrcPdfDocument.pdf");//fileInput.FileName;
                string DestFile = Server.MapPath("\\Files\\DestPdfDocument.pdf");//fileOutput.FileName;
                string SrcCertificate = Server.MapPath("\\Files\\PDF.pfx"); //Server.MapPath("\\Files\\" + fileCertificate.PostedFile.FileName);
                string SignatureImagePath = Server.MapPath("\\Images\\samjoseph.png");
                string CertificatePAssword = txtCertificatePassword.Text;
                string publicKeyStreamFile = Server.MapPath("\\Files\\PDF.cer");
                var pk = akp;
                var Status = CheckCertificate(SrcCertificate, "roopatijo@123");//CertificatePAssword);
                if (Status == "0")
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "Please make sure you upload a valid Certificate and Password";
                    return;
                }
                PdfReader reader = new PdfReader(SrcFile);
                FileStream os = new FileStream(DestFile, FileMode.Create);
                PdfStamper stamper = PdfStamper.CreateSignature(reader, os, '\0');
                
                md = AddMetaData(stamper);
                metadata = md;
                //stamper.MoreInfo = metadata.getMetaData();
                //Not making the Pdf editable once signature is placed.
                // reader.RemoveUsageRights();
                //Signature Image Added
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(SignatureImagePath);
                image.SetAbsolutePosition(0, 0);
                float w1 = image.ScaledWidth;
                float h1 = image.ScaledHeight;
                // PdfTemplate template = PdfTemplate.CreateTemplate(stamper.Writer,image.Width, image.Height);
                //template.AddImage(image);
                //stamper.GetOverContent(1).AddTemplate(template, 36, 748); //AddTemplate(template, 150, 200, true); 
                // Creating the appearance
                PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                //appearance.Reason = "This document is for Testing purpose"; 
                // appearance.Location ="Kottayam";
                appearance.SignDate = System.DateTime.Now;
                //appearance.SetCrypto(akp, chain, null, PdfSignatureAppearance.WINCER_SIGNED);
                appearance.SetVisibleSignature(new iTextSharp.text.Rectangle(36f, 748f, 36f+w1, 748f+h1), 1, null);
                //Tag removed using Acro6Layers as true
                appearance.Acro6Layers = true ;
                appearance.Image = image;
                IExternalSignature externalSignature = new PrivateKeySignature(akp, DigestAlgorithms.SHA512);
              //  ITSAClient tsaClient = new TSAClientBouncyCastle(timeStampServer, timeStampUser, timeStampPass);

                //var tsc = new TSAClientBouncyCastle(SignatureData.TlsClient, SignatureData.TsaClient.UserName, SignatureData.TsaClient.Password);

                MakeSignature.SignDetached(appearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);
                stamper.Close();
                reader.Close();

               // FileStream PublicKeyfs = new FileStream(publicKeyStreamFile, FileMode.Create);
                //verifyPdfSignature(DestFile, PublicKeyfs);
                //return;





                PdfReader destinationreader = new PdfReader(DestFile);
                AcroFields acroFields = destinationreader.AcroFields;
                //FileStream PublicKeyfs = new FileStream(publicKeyStreamFile, FileMode.Create);
                //var parser = new X509CertificateParser();
               // var certificate = parser.ReadCertificate(PublicKeyfs);
                List<String> names = acroFields.GetSignatureNames();
                bool Valid=false;
                string LogFilePath = Server.MapPath("\\Files\\Log.txt");
                StreamWriter writer = new StreamWriter(LogFilePath);
                string NameoFSignatureValid = string.Empty;
                details = string.Empty;
                X509Certificate[] certs;
                PdfPKCS7 pdfPkcs7;
                foreach (string name in names)
                {   
                     pdfPkcs7 = acroFields.VerifySignature(name);
                    //
                   // pdfPkcs7= VerifySignature(acroFields, name);
                    Valid= pdfPkcs7.Verify();
                     NameoFSignatureValid = name;
                    var cal = pdfPkcs7.SignDate;
                    certs = pdfPkcs7.SignCertificateChain;
                    var pkc = pdfPkcs7.Certificates;
                    var reason = pdfPkcs7.Reason;
                    Org.BouncyCastle.X509.X509Certificate signingCertificate = pdfPkcs7.SigningCertificate;
                    var issuerDN = signingCertificate.IssuerDN;
                    var subjectDN = signingCertificate.SubjectDN;

                    //Writing to Log file
                     details = "Verified Status :" + Valid + "," + "Date:" + pdfPkcs7.SignDate.Date +","+ "Reason:" + reason +","+ "Issuer:" + issuerDN +","+ " Subject" + subjectDN;

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


                    string info= details+"DestPdfDocument.pdf is Validated successfully";
                    //File.WriteAllText(@"D:\temp\log.txt", info);
                    lblMessage.Text = "Document successfully Uploaded to Folder (Files/DestPdfDocument.pdf).";
                    lblMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error Occured. Please check.";
                lblMessage.Visible = true;
            }


        }

        public void verifyPdfSignature(string pdfFile, Stream publicKeyStream)
        {
            var parser = new X509CertificateParser();
            var certificate = parser.ReadCertificate(publicKeyStream);
            publicKeyStream.Dispose();

            PdfReader reader = new PdfReader(pdfFile);
            AcroFields af = reader.AcroFields;
            var names = af.GetSignatureNames();

            if (names.Count == 0)
            {
                throw new InvalidOperationException("No Signature present in pdf file.");
            }

            foreach (string name in names)
            {
                if (!af.SignatureCoversWholeDocument(name))
                {
                    throw new InvalidOperationException(string.Format("The signature: { 0 } does not covers the whole document.", name));
                }

                PdfPKCS7 pk = af.VerifySignature(name);
                var cal = pk.SignDate;
                var pkc = pk.Certificates;
                Org.BouncyCastle.Tsp.TimeStampToken tts = pk.TimeStampToken;
                Org.BouncyCastle.X509.Store.X509CertStoreSelector selector = new Org.BouncyCastle.X509.Store.X509CertStoreSelector();
                selector.Issuer = tts.SignerID.Issuer;
                selector.SerialNumber = tts.SignerID.SerialNumber;
                // Retrieve the matching certificates from the time stamp certificate collection
                System.Collections.ICollection certs = tts.GetCertificates("COLLECTION").GetMatches(selector);
                // Assuming at most one match, retrieve this matching certificate
                IEnumerator enumCerts = certs.GetEnumerator();
                if (enumCerts.MoveNext())
                {
                    X509Certificate cert = (X509Certificate)enumCerts.Current;

                    // Verify that this is the correct certificate by verifying the time stamp token
                    tts.Validate(cert);

                    // Extracting information from the now verified tsa certificate
                    Console.WriteLine(String.Format("Not before: {0}", cert.NotBefore));
                    Console.WriteLine(String.Format("Not after: {0}", cert.NotAfter));
                }

                if (!pk.Verify())
                {
                    throw new InvalidOperationException("The signature could not be verified.");
                }
                if (!pk.VerifyTimestampImprint())
                {
                    throw new InvalidOperationException("The signature timestamp could not be verified.");
                }

                Object[] fails =(Object[]) CertificateVerification.VerifyCertificates(pkc, new X509Certificate[] { certificate }, null, cal);
                if (fails != null)
                {
                    throw new InvalidOperationException("The file is not signed using the specified key - pair.");
                }
            }
        }



        public PdfPKCS7 VerifySignatureOld(AcroFields fields, String name)
        {
            //Console.WriteLine("Signature covers whole document: " + fields.SignatureCoversWholeDocument(name));
            //Console.WriteLine("Document revision: " + fields.GetRevision(name) + " of " + fields.TotalRevisions);
            PdfPKCS7 pkcs7 = fields.VerifySignature(name);
            //Console.WriteLine("Integrity check OK? " + pkcs7.Verify());
            return pkcs7;
        }

        public PdfPKCS7 VerifySignature(AcroFields fields, String name)
        {
            PdfPKCS7 pkcs7 = VerifySignatureOld(fields, name);
            X509Certificate[] certs = pkcs7.SignCertificateChain;
            DateTime cal = pkcs7.SignDate;
            X509Store store = new X509Store(StoreName.Root);
            store.Open(OpenFlags.ReadOnly);
            foreach (var tmpcert in store.Certificates)
            {
                certificates.Add(DotNetUtilities.FromX509Certificate(tmpcert));
            }
            store.Close();

            store = new X509Store(StoreName.CertificateAuthority);
            store.Open(OpenFlags.ReadOnly);
            foreach (var tmpcert in store.Certificates)
            {
                certificates.Add(DotNetUtilities.FromX509Certificate(tmpcert));
            }
            store.Close();

            IList<iTextSharp.text.pdf.security.VerificationException> errors = CertificateVerification.VerifyCertificates(certs, certificates, null, cal.ToUniversalTime());
            if (errors == null)
                Console.WriteLine("Certificates verified against the KeyStore");
            else
                foreach (object error in errors)
                    details = details + error;
                   
            for (int i = 0; i < certs.Length; ++i)
            {
                X509Certificate cert = certs[i];
                details= details+"Certificate " + i ;
                
            }
            X509Certificate signCert = certs[0];
            X509Certificate issuerCert = (certs.Length > 1 ? certs[1] : null);

            details = details + "Checking validity of the document at the time of signing";
            CheckRevocation(pkcs7, signCert, issuerCert, cal.ToUniversalTime());
            details = details+"Checking validity of the document today";
            CheckRevocation(pkcs7, signCert, issuerCert, DateTime.Now);
            return pkcs7;
        }



        public int CheckRevocation(PdfPKCS7 pkcs7, X509Certificate signCert, X509Certificate issuerCert, DateTime date)
        {
            List<BasicOcspResp> ocsps = new List<BasicOcspResp>();
            if (pkcs7.Ocsp != null)
                ocsps.Add(pkcs7.Ocsp);
            iTextSharp.text.pdf.security.OcspVerifier ocspVerifier = new iTextSharp.text.pdf.security.OcspVerifier(null, ocsps);  
            var verification =
                ocspVerifier.Verify(signCert, issuerCert, date);
            if (verification.Count == 0)
            {
                List<X509Crl> crls = new List<X509Crl>();
                if (pkcs7.CRLs != null)
                    foreach (X509Crl crl in pkcs7.CRLs)
                        crls.Add(crl);
                CrlVerifier crlVerifier = new CrlVerifier(null, crls);
                verification.AddRange(crlVerifier.Verify(signCert, issuerCert, date));
            }
            if (verification.Count == 0)
            {
                Console.WriteLine("No se pudo verificar estado de revocación del certificado por CRL ni OCSP");
                return CER_STATUS_NOT_VERIFIED;
            }
            else
            {
                foreach (VerificationOK v in verification)
                    Console.WriteLine(v);
                return 0;
            }
        }
        public void Verify()
        {
            //Bouncy Castle is a collection of APIs used in cryptography
          // Org.BouncyCastle.Crypto.B BouncyCastleProvider provider = new BouncyCastleProvider();

        }

        /// <summary>
        /// Verifies the signature of a prevously signed PDF document using the specified public key
        /// </summary>
        /// <param name="pdfFile">a Previously signed pdf document</param>
        /// <param name="publicKeyStream">Public key to be used to verify the signature in .cer format</param>
        /// <exception cref="System.InvalidOperationException">Throw System.InvalidOperationException if the document is not signed or the signature could not be verified</exception>



        public  String layer4Text;
        public string Layer4Text
        {
            get
            {
                return layer4Text;
            }
            set
            {
                layer4Text = value;
            }
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
        private MetaData AddMetaData(PdfStamper stamper)
        {
            //Adding Meta Datas
            MetaData MyMD = new MetaData();
            MyMD.Author = "roopa.prabha@claysys.in";
            MyMD.Title = "Digital Signature Document";
            MyMD.Subject = "Test";
            MyMD.Creator = "Claysys";
            MyMD.Producer = "roopa.prabha";
            return MyMD;
            // metadata = MyMD;
            //stamper.MoreInfo = metadata.getMetaData();
            // stamper.XmpMetadata = this.metadata.getStreamedMetaData();
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
                Status = "0";
            }
            return Status;
        }

        protected void btnSavefile_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                // Code to write the stream goes here.
                txtSaveFile.Text = saveFileDialog1.FileName;


            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string DestFile = Server.MapPath("\\Files\\DestPdfDocument.pdf");//fileOutput.FileName;
                PdfReader reader = new PdfReader(DestFile);
                AcroFields acroFields = reader.AcroFields;
                List<String> names = acroFields.GetSignatureNames();

                foreach (string name in names)
                {
                    PdfPKCS7 pdfPkcs7 = acroFields.VerifySignature(name);
                    pdfPkcs7.Verify();
                    //Validate(pdfPkcs7);


                }

            }
            catch (Exception ex)
            {

            }
        }

        public void Validate(PdfPKCS7 pdfpks)
        {
            
           /* KeyStore kall = pdfpks.loadCacertsKeyStore();
            PdfReader reader = new PdfReader("my_signed_doc.pdf");
            AcroFields af = reader.getAcroFields();
            ArrayList names = af.getSignatureNames();
            for (int k = 0; k < names.size(); ++k)
            {
                String name = (String)names.get(k);
                System.out.println("Signature name: " + name);
                System.out.println("Signature covers whole document: " + af.signatureCoversWholeDocument(name));
                PdfPKCS7 pk = af.verifySignature(name);
                Calendar cal = pk.getSignDate();
                Certificate pkc[] = pk.getCertificates();
                System.out.println("Subject: " + PdfPKCS7.getSubjectFields(pk.getSigningCertificate()));
                System.out.println("Document modified: " + !pk.verify());
                Object fails[] = PdfPKCS7.verifyCertificates(pkc, kall, null, cal);
                if (fails == null)
                    System.out.println("Certificates verified against the KeyStore");
                else
                    System.out.println("Certificate failed: " + fails[1]);
            }*/
        }


       /* public PdfPKCS7 verifySignature(String name)
        {
            return verifySignature(name, null);
        }*/
        

    }
}