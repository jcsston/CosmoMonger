<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Verify Email</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Verify Email</h2>
    
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm("VerifyEmail", "Account", FormMethod.Get)) { %>
        <table cellspacing="5">
            <tr>
                <td>Username:</td>
                <td>
                    <%= Html.TextBox("username") %>
                    <%= Html.ValidationMessage("username") %>
                </td>
            </tr>
            <tr>
                <td>Verification Code:</td>
                <td>
                    <%= Html.TextBox("verificationCode") %>
                    <%= Html.ValidationMessage("verificationCode")%>
                </td>
            </tr>
            <tr>
                <td></td>
                <td><input type="submit" value="Verify" /></td>
            </tr>
        </table>
    <% } %>
    
    <p>
        Never received your verification code?
        <br /> 
        Click <%= Html.ActionLink("here", "SendVerificationCode", new { username = ViewData["username"] })%> to send a new verification code.
    </p>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
