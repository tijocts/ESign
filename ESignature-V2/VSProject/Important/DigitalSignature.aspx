<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DigitalSignature.aspx.cs" Inherits="Important.DigitalSignature" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function RequiredFieldChecking() {
            
        }

    </script>
    <br />
    <br />
    <!DOCTYPE>
    <html>


    <body>
        <div style="text-align:center;color:red;">
            <asp:Label ID="lblMessage" runat="server"></asp:Label></div>
        <br /><br />
        <table width="70%" border="1">
            <tr>
                <td>
                    <fieldset>
                        <legend>
                            <h5>Files</h5>
                        </legend>
                        <table width="10%">
                            <tr>
                                <td>
                                    <asp:FileUpload ID="fileInput" runat="server"/>

                                </td>
                                <td>
                                    <asp:TextBox ID="txtSaveFile" runat="server"></asp:TextBox>

                                </td>
                                <td>
                                    <asp:Button ID="btnSavefile" runat="server" Text="Save" OnClick="btnSavefile_Click" />

                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
          <%--      <td rowspan="2">
                    <fieldset>
                        <legend><h5>Meta Data</h5></legend>

                        <table border="0" align="right">
                            <tr style="text-align:right;">
                                <td>
                                    <asp:Label ID="Label1" runat="server" Text="Author:"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtAuthor" runat="server"></asp:TextBox></td>


                            </tr>
                              <tr>
                                <td>
                                    <asp:Label ID="Label2" runat="server" Text="Title:"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox></td>


                            </tr>
                        </table>

                    </fieldset>
                </td>--%>
            </tr>
            <tr>
                <td>
                    <fieldset>
                        <legend>
                            <h5>Certificate</h5>
                        </legend>
                        <table width="10%">
                            <tr>
                                <td>
                                    <asp:FileUpload ID="fileCertificate" runat="server" />

                                </td><td>&nbsp;</td>
                                <td>Password:</td>
                                <td>
                                    <asp:TextBox ID="txtCertificatePassword" runat="server" TextMode="Password"></asp:TextBox></td>
                            </tr>
                        </table>

                    </fieldset>
                </td>
            </tr>
            <tr><td colspan="3" style="text-align:right">
                <asp:Button ID="btnGenerate" runat="server" Text="Generate PDF"  OnClick="btnGenerate_Click" />
                <asp:Button ID="Button1" runat="server" Text="Validate" OnClick="Button1_Click" />
                </td></tr>
        </table>



        <br />


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
    <br />
    <br />
    <br />

</asp:Content>
