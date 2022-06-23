<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadFile.aspx.cs" Inherits="Important.UploadFile" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br /><asp:FileUpload ID="FileUpload" runat="server" /><asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_UploadFile"/>
</asp:Content>
