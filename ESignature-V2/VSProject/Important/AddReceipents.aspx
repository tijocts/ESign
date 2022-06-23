<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="AddReceipents.aspx.cs" Inherits="Important.AddReceipents" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset="UTF-8">
        <title>Digital Signature -POC</title>
        <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/css/bootstrap.min.css'>
        <link rel="stylesheet" href="css/style.css">
    </head>
    <body>
        <script>

            document.getElementById("clickMe").onclick = function () {
               // debugger;
               // alert(window.COordinatevalue);
              //  __doPostBack("<%=lnk.UniqueID%>", "{X:" + pos1 + ",Y:" + pos2 + "}");

            };
            function DBSaving() {
                showCoordinates();
                // alert(pdfbase64Data);
                // downloadPDF(pdfbase64Data);
                //alert(pdfcoordinates);
                //alert(ReceipentsList);
                __doPostBack("<%=lnk.UniqueID%>", "{Coordinates:" + pdfcoordinates + ",Recipients:" + ReceipentsDetails + "}");
            }

            function LogOut() {
                __doPostBack("<%=LogOut.UniqueID%>", '');
            }

        </script>
        <div>
            <panel style="background-color: cornflowerblue;">
                <fieldset>
                    <legend>
                        <h5>Add Recipients</h5>
                    </legend>
                    <table>
                        <tr><td style="text-align:center;"><asp:Label ID="lblMessage" ForeColor="Red" runat="server"  Visible="false"></asp:Label></td></tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblName" runat="server" Text="Name:"></asp:Label>
                                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                            </td>
                            <td>
                            <td>
                                <asp:Label ID="lblEmail" runat="server" Text="Email:"></asp:Label>
                                <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" /></td>
                        </tr>
                        <tr>
                            <td>
                        </tr>
                    </table>
                </fieldset>
            </panel>

            <br />


            <%--style="background: url(Images/signatureupload.png);width:120px;height:120px;"><h9>Sign</h9></button>--%>
            <asp:ListView ID="lstViewReceipents" runat="server">

                <LayoutTemplate>
                    <table width="700px" border="1">
                        <tr>
                            <td>
                                <b></b></td>
                            <td style="text-align: left"><b>Name</b></td>
                            <td style="text-align: left"><b>Email</b></td>
                        </tr>
                        <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
                    </table>
                </LayoutTemplate>
                <ItemTemplate>

                    <tr>
                        <td style="text-align: center; width: 30px">

                            <input type="radio" id="receipents1" name="rec" value="receipents1" onchange="SelectedReceipent('<%#Eval("ID").ToString() %>','<%#Eval("Name").ToString() %>','<%#Eval("Email").ToString() %>');">
                        <td style="text-align: left">
                            <asp:Label ID="lblName" runat="server" Text='<%#Eval("Name").ToString() %>'></asp:Label></td>
                        <td style="text-align: left">
                            <asp:Label ID="Label1" runat="server" Text='<%#Eval("Email").ToString() %>'></asp:Label></td>


                    </tr>


                </ItemTemplate>
                <EmptyDataTemplate>
                    <table width="700px" border="1">
                        <tr>
                            <td>
                                <b></b></td>
                            <td style="text-align: left"><b>Name</b></td>
                            <td style="text-align: left"><b>Email</b></td>
                        </tr>
                         <tr>
                            <td colspan="3" style="font-weight:bolder;text-align:center;">No Recipients Added.
                            </td>
                        </tr>                      
                    </table>
                   
                </EmptyDataTemplate>
            </asp:ListView>
            <%--  --%>
            <asp:LinkButton ID="lnk" runat="server" OnClick="lnk_Click"></asp:LinkButton>
             <asp:LinkButton ID="LogOut"  runat="server" OnClick="LogOut_Click"></asp:LinkButton>

            
            <div>
                <table align="right">
                    <div>
                        <br />
                        <table align="right">
                            <tr>
                                <td colspan="3">
                                    <input type="button" id="clickMe" value="Send" onclick="javascript:DBSaving();" />

                                </td>
                                <td>
                                    <input type="button" id="clickmeLogOut" value="LogOut" onclick="javascript:LogOut();" /> 
                                </td>
                                
                            </tr>
                        </table>
                    </div>
                </table>
            </div>

        </div>
        <div class="container">
            <div class="row">
                <div class="col-md-12" style="padding: 10px">
                    <button class="btn btn-primary btn-block" style="display: none;" onclick="showCoordinates();return false;">Click here for Coordinates</button>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12" id="pdfManager" style="display: none">
                    <div class="row" id="selectorContainer">
                        <div class="col-fixed-240" id="parametriContainer" style="display: none">
                        </div>
                        <div class="col-fixed-605">
                            <div id="pageContainer" class="pdfViewer singlePageView dropzone nopadding" style="background-color: transparent">
                                <canvas id="the-canvas" style="border: 1px  solid black"></canvas>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- parameters showed on the left sidebar -->
        <input id="parameters" type="hidden" value='[{"idParametro":1,"descrizione":"Signature","valore":"X","nota":null},{"idParametro":2,"descrizione":"Name","valore":"X","nota":null},{"idParametro":3,"descrizione":"Date","valore":"X","nota":null}]' />
        <!-- Below the pdf base 64 rapresentation -->
        <input id="pdfBase64" type="hidden" runat="server" />

        <input type="hidden" id="hdnSelectedReceipentValue" runat="server" />
        <input type="hidden" id="hdnSelectedReceipentName" runat="server" />
        <input type="hidden" id="hdnSelectedReceipentEmail" runat="server" />
        <input type="hidden" id="hdnSelectedReceipentSignDate" runat="server" />
        <script src='https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js'></script>
        <script src='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/js/bootstrap.min.js'></script>
        <script src='https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.0.943/pdf.min.js'></script>
        <script src='https://cdnjs.cloudflare.com/ajax/libs/interact.js/1.2.9/interact.min.js'></script>
        <script src='https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.0.943/pdf.worker.min.js'></script>

        <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/css/bootstrap.min.css'>

        <link rel="stylesheet" href="css/style.css">

        <script src="js/indexpocmail.js"></script>



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
<%----%>