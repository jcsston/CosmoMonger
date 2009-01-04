<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="SentVerificationCode.aspx.cs" Inherits="CosmoMonger.Views.Account.SentVerificationCode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Sent Verification Code</h2>
    
    <p>Check your e-mail for a e-mail from cosmomonger.com and click the link in the e-mail</p>
    <p>You can also go <%= Html.ActionLink("here", "VerifyEmail")%> to enter in your verification code from the e-mail.</p>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
