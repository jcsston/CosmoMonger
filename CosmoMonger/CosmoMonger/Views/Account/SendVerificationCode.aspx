<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="SendVerificationCode.aspx.cs" Inherits="CosmoMonger.Views.Account.SendVerificationCode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Send Verification Code Email</h2>
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) { %>
        <table cellspacing="5">
            <tr>
                <td>Username:</td>
                <td>
                    <%= Html.TextBox("username") %>
                    <%= Html.ValidationMessage("username") %>
                </td>
            </tr>
            <tr>
                <td></td>
                <td><input type="submit" value="Send" /></td>
            </tr>
        </table>
    <% } %>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
