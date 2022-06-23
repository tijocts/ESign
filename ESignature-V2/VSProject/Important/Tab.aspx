<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Tab.aspx.cs" Inherits="Important.Tab" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
      <!DOCTYPE html>
     <html>
    <head>
        <meta charset="UTF-8">
          <title>Signature Upload</title>
    <style type="text/css">
.Initial
{
  display: block;
  padding: 4px 18px 4px 18px;
  float: left;
  background: url("../Images/InitialImage.png") no-repeat right top;
  color: Black;
  font-weight: bold;
}
.Initial:hover
{
  color: White;
  background: url("../Images/SelectedButton.png") no-repeat right top;
}
.Clicked
{
  float: left;
  display: block;
  background: url("../Images/SelectedButton.png") no-repeat right top;
  padding: 4px 18px 4px 18px;
  color: Black;
  font-weight: bold;
  color: White;
}
</style>
  </head>
    <body> 
        <form id="form1" runat="server">
         <table width="80%" align="center">
      <tr>
        <td>
          <asp:Button Text="Upload" BorderStyle="None" ID="Tab1" CssClass="Initial" runat="server"
              OnClick="Tab1_Click" />
          <asp:Button Text="Templates" BorderStyle="None" ID="Tab2" CssClass="Initial" runat="server"
              OnClick="Tab2_Click" /> 
            <asp:Button Text="Draw Signature" BorderStyle="None" ID="Tab3" CssClass="Initial" runat="server"
              OnClick="Tab3_Click" />
          <asp:MultiView ID="MainView" runat="server">
            <asp:View ID="View1" runat="server">
              <table style="width: 100%; border-width: 1px; border-color: #666; border-style: solid">
                <tr>
                  <td>
                    <h3>
                      <span>Signature Upload</span>
                    </h3>
                  </td>
                </tr>
              </table>
            </asp:View>
            <asp:View ID="View2" runat="server">
              <table style="width: 100%; border-width: 1px; border-color: #666; border-style: solid">
                <tr>
                  <td>
                    <h3>
                      Signature Templates
                    </h3>
                  </td>
                </tr>
              </table>
            </asp:View>
            <asp:View ID="View3" runat="server">
              <table style="width: 100%; border-width: 1px; border-color: #666; border-style: solid">
                <tr>
                  <td>
                    <h3>
                     Draw Signature
                    </h3>
                  </td>
                </tr>
              </table>
            </asp:View>
          </asp:MultiView>
        </td>
      </tr>
    </table>
        </form>
        </body></html>
</asp:Content>
