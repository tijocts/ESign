<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RecipientPage.aspx.cs" Inherits="Important.RecipientPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset="UTF-8">
        <title>Digital Signature -POC</title>
        <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/css/bootstrap.min.css'>
        <link rel="stylesheet" href="css/style.css">

        <link href="https://code.jquery.com/ui/1.11.4/themes/ui-darkness/jquery-ui.css" rel="stylesheet" />
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
        <script src="https://code.jquery.com/ui/1.11.4/jquery-ui.min.js"></script>

        <link rel="stylesheet" href="//code.jquery.com/ui/1.13.1/themes/base/jquery-ui.css">
        <link rel="stylesheet" href="/resources/demos/style.css">
        <script src="https://code.jquery.com/jquery-3.6.0.js"></script>
        <script src="https://code.jquery.com/ui/1.13.1/jquery-ui.js"></script>

        <script src="js/tabs.js"></script>

        <style>
            .Bold {
                font-weight: bolder;
            }
            /* The Modal (background) */
            .modal {
                display: none; /* Hidden by default */
                position: fixed; /* Stay in place */
                z-index: 1; /* Sit on top */
                left: 0;
                top: 0;
                width: 100%; /* Full width */
                height: 100%; /* Full height */
                overflow: auto; /* Enable scroll if needed */
                background-color: rgb(0,0,0); /* Fallback color */
                background-color: rgba(0,0,0,0.4); /* Black w/ opacity */
            }

            /* Modal Content/Box */
            .modal-content {
                background-color: #fefefe;
                margin: 15% auto; /* 15% from the top and centered */
                padding: 20px;
                border: 1px solid #888;
                width: 30%; /* Could be more or less, depending on screen size */
            }

            /* The Close Button */
            .close {
                color: #aaa;
                float: right;
                font-size: 28px;
                font-weight: bold;
            }

                .close:hover,
                .close:focus {
                    color: black;
                    text-decoration: none;
                    cursor: pointer;
                }


            [data-tab-info] {
                display: none;
            }

            .active[data-tab-info] {
                display: block;
            }

            .tab-content {
                font-family: sans-serif;
                /*font-weight: bold;*/
                color: rgb(82, 75, 75);
            }

            .tabs {
                color: rgb(255, 255, 255);
                display: flex;
                margin: 0;
                text-align: center;
            }

                .tabs span {
                    background: rgb(28, 145, 38);
                    padding: 10px;
                    border: 1px solid rgb(255, 255, 255);
                }

                    .tabs span:hover {
                        background: rgb(29, 185, 112);
                        cursor: pointer;
                        color: black;
                    }
        </style>
        <script language="javascript" type="text/javascript">
            //localStorage.clear();
            $(document).ready(function () {
                localStorage.theImage = '';
                var img = new Image();
                img.src = localStorage.theImage;
                $('.imagearea').html(img);
                var fileupload = document.getElementById("logo");
                fileupload.onchange = function () {
                    var fileInput = $(this)[0];
                    var file = fileInput.files[0];
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        // Create a new image.
                        var img = new Image();
                        img.src = reader.result;
                        img.width = "20px";
                        img.height = "20px";
                        localStorage.theImage = reader.result; //stores the image to localStorage
                        $(".imagearea").html(img);
                        document.getElementById('tabs').style.display = 'none';

                    }
                    reader.readAsDataURL(file);
                    SignaturePopup(GlobalSavePdfX, GlobalSavePdfY, GlobalSavex, GlobalSavey);

                };
                //Test();

            });


            function SessionValues(controllistarray) {
                //debugger;       
                AdminControlsPlacedList = controllistarray;
                PositionControls(AdminControlsPlacedList);
                //RecipientsAddedList = Recipients;
                return false;
            }

            $(".close").click = function () {
                modal.style.display = "none";
            };





        </script>

    </head>
    <body>
         <div style="text-align:center;color:red;">
            <asp:Label ID="lblMessage" runat="server"></asp:Label></div>
        <br /><br />
        <panel style="background-color: cornflowerblue;">
            <fieldset>
                <legend>
                    <h5 class="Bold">Recipient Page</h5>
                </legend>
                <table style="width: 80%;border:1px;">
                    <tr>
                        <td>
                            <asp:Label ID="lblName" runat="server" Text="Name:"></asp:Label>
                            <asp:Label ID="lblRecipientName" runat="server" Text="" CssClass="Bold"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblEmail" runat="server" Text="Email:"></asp:Label>
                            <asp:Label ID="lblRecipientEmail" runat="server" Text="" CssClass="Bold"></asp:Label>
                        </td>
                        <td style="display: none;">
                            <asp:Button ID="btnAdd" runat="server" Text="Upload Signature" />&nbsp;&nbsp;
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/samjoseph.png" />

                        </td>
                        <td>
                            <asp:Label ID="lblMailto" runat="server" Text="Send as Mail To:"></asp:Label>
                            <asp:Label ID="lbEmailAddress" runat="server" Text="hr.team@claysys.net" CssClass="Bold"></asp:Label>
                        </td>
                    </tr>
                    <tr style="display:none;">
                        <td>
                            <fieldset>
                                <legend>
                                    <h5>Certificate</h5>
                                </legend>
                                <table width="10%">
                                    <tr>
                                        <td>
                                            <asp:FileUpload ID="fileCertificate" runat="server" />

                                        </td>
                                        <td>&nbsp;</td>
                                        <td>Password:</td>
                                        <td>
                                            <asp:TextBox ID="txtCertificatePassword" runat="server" TextMode="Password"></asp:TextBox></td>
                                    </tr>
                                </table>

                            </fieldset>
                        </td>
                    </tr>

                </table>
            </fieldset>
        </panel>


       


        <div class="modal" id="tabs" style="display: none;">
            <!-- Modal content -->
            <label style="font-weight: bold; font: 12px;">Signature</label>
            <div class="modal-content">
                <span class="close">&times;</span>
                <div class="tabs">
                    <span data-tab-value="#tab_1">Upload</span>
                    <span data-tab-value="#tab_2">Template</span>
                    <span data-tab-value="#tab_3">Draw</span>
                </div>
                <div class="tab-content">
                    <div class="tabs__tab active" id="tab_1">
                        <input class="classhere" type="file" name="logo" id="logo" />
                        <div class="imagearea" id="Sign"></div>

                    </div>
                    <div class="tabs__tab" id="tab_2">
                        <p></p>

                    </div>
                    <div class="tabs__tab" id="tab_3">
                        <p></p>

                    </div>
                </div>
            </div>

        </div>



        <div style="text-align: right;">
            <asp:Button ID="btn" runat="server" Text="Send" ForeColor="Green" OnClick="btn_Click" OnClientClick="javascript:Validate();return true;" />
        </div>


        <div class="container">
            <div class="row">
                <div class="col-md-12" style="padding: 10px; display: none;">
                    <button class="btn btn-primary btn-block" onclick="showCoordinates()">Click here for Coordinates</button>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12" id="pdfManager">
                    <div class="row" id="selectorContainer">
                        <div class="col-fixed-240" id="parametriContainer" style="display: none">
                        </div>
                        <div class="col-fixed-605">
                            <div id="pageContainer" class="pdfViewer singlePageView dropzone nopadding"
                                style="background-color: transparent">
                                <canvas id="the-canvas" style="border: 1px  solid black"></canvas>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <input id="parameters" type="hidden" value='[{"idParametro":1,"descrizione":"Signature","valore":"X","nota":null}]' />

        <!-- Below the pdf base 64 Representation -->
        <input id="pdfBase64" type="hidden" runat="server" />
        <input type="hidden" id="hdnSelectedReceipentValue" runat="server" />
        <input type="hidden" id="hdnSelectedSignButtonValue" runat="server" />
        <input type="hidden" id="hdnSignImage" runat="server" />
        <script src='https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js'></script>
        <script src='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/js/bootstrap.min.js'></script>
        <script src='https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.0.943/pdf.min.js'></script>
        <script src='https://cdnjs.cloudflare.com/ajax/libs/interact.js/1.2.9/interact.min.js'></script>
        <script src='https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.0.943/pdf.worker.min.js'></script>
        <script src="js/WorkFlowPage1.js"></script>
        <link href="css/tabs.css" rel="stylesheet" />


    </body>
    </html>
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />




</asp:Content>
