<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="RecipientRedirectPage.aspx.cs" Inherits="Important.RecipientRedirectPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <!DOCTYPE html>
    <html>
    <head>       

    </head><body>
         <div style="text-align:center;color:red;">
            <asp:Label ID="lblMessage" runat="server"></asp:Label></div>
        <br /><br />
        <fieldset><legend>Added Recipents List</legend></fieldset>
           <asp:ListView ID="lstViewReceipents" runat="server">

                <LayoutTemplate>
                    <table width="700px" border="1">
                        <tr>
                            <td>
                                <b></b></td>
                            <td style="text-align: left"><b>Name</b></td>
                            <td style="text-align: left"><b>Email</b></td>
                            <td style="text-align: left"><b>
                                &nbsp;</b></td>
                        </tr>
                        <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
                    </table>
                </LayoutTemplate>
                <ItemTemplate>

                    <tr>
                        <td style="text-align: center; width: 30px">
                            <asp:HiddenField ID="hdnRecipienId" Value='<%#Eval("ID").ToString() %>' runat="server" />
<%--                            <input type="radio" id="receipents1" name="rec" value="receipents1" onchange="SelectedReceipent('<%#Eval("ID").ToString() %>','<%#Eval("Name").ToString() %>','<%#Eval("Email").ToString() %>');">--%>
                        <td style="text-align: left">
                            <asp:Label ID="lblName" runat="server" Text='<%#Eval("Name").ToString() %>'></asp:Label></td>
                        <td style="text-align: left">
                            <asp:Label ID="lblEmail" runat="server" Text='<%#Eval("Email").ToString() %>'></asp:Label></td>
                        <td>
                            <asp:LinkButton ID="lnkSignFlow" runat="server" CommandName='<%#Eval("ID").ToString() %>' OnClick="btnSign_Click" Text="Sign"></asp:LinkButton>
                           <%-- <asp:Button ID="btnSign" runat="server" Text="Signature Workflow"  OnClick="btnSign_Click"  /></td>--%>


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
         

           </body>
        </html>
</asp:Content>
