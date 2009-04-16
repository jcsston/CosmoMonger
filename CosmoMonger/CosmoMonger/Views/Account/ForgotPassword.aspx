<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Forgotten Password</title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Forgot Password?</h1>
    <p>
        Please enter your email below and click Send. An email message will be sent 
        to your email address with a link to reset your password.
    </p>
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) { %>
        <div>
            <table>
                <tr>
                    <td>Email:</td>
                    <td>
                        <%= Html.TextBox("email")%>
                        <%= Html.ValidationMessage("email")%>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td><input type="submit" value="Send" /></td>
                </tr>
            </table>
        </div>
    <% } %>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
