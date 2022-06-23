<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Important._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
      
        <p class="lead"></p>
        <p></p>
        <div class="row">
            <div class="">
                <asp:FileUpload ID="UploadFile" runat="server" Visible="false" />
                <div style="text-align:center"><asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
</div>
                <fieldset>
                    <legend>Login</legend>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblName" runat="server" Text="Name:"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                            </td>
                       
                            <td>
                                <asp:Label ID="lblPassword" runat="server" Text="Password:"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                            </tr>
                        <tr>
                            <td colspan="4" style="text-align:right">   <asp:LinkButton ID="lnkLogin" runat="server" OnClick="lnkLogin_Click">Login</asp:LinkButton>
         </td>
                        </tr>
                    </table>
                </fieldset>

                </div>
        </div>
    </div>



</asp:Content>
