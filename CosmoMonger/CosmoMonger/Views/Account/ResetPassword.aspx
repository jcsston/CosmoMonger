<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Reset Password</title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Reset Password</h2>
    <p>
        Hello <b><%=Html.Encode(ViewData["username"]) %></b>.
    </p>
    <p>
        From this page you can reset your forgotten password to a new generated password.
        If you wish to proced click the confirm button to reset your password.
        <br />
        If you do not wish to reset your password you can <%=Html.ActionLink("login", "Login") %> here.
    </p>

    <% using (Html.BeginForm()) { %>
        <div>
            <input type="submit" value="Confirm" />
        </div>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
