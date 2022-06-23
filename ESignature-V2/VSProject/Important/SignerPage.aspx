<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SignerPage.aspx.cs" Inherits="Important.SignerPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset="UTF-8">
        <title>Digital Signature -POC</title>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/interact.js/1.2.9/interact.min.js"></script>
        <script src="https://code.jquery.com/ui/1.13.1/jquery-ui.js" integrity="sha256-6XMVI0zB8cRzfZjqKcD01PBsAy3FlDASrlC8SxCpInY=" crossorigin="anonymous"></script>
        <script src="//mozilla.github.io/pdf.js/build/pdf.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/interact.js/1.2.9/interact.min.js"></script>
    </head>
    <body>
        <br>
        <br>
        <input type="button" value="Load PDF and placeholders" id="loadpdf">
        <br>
        <span class="text" id="prev">Previous</span>

        <span>Page: <span id="page_num"></span>/ <span id="page_count"></span></span>
        <span class="text" id="next">Next</span>
        <canvas id="the-canvas" class="canv"></canvas>
    </body>

    <script src="js/SignerPage.js"></script>
    <style>
        #the-canvas {
            border: 1px solid black;
            direction: ltr;
        }
    </style>
    </html>


</asp:Content>
