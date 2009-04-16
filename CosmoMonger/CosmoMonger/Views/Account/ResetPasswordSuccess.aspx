<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Password Successfully Reset</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Password Successfully Reset</h1>
    <p>
        Welcome <b><%=Html.Encode(ViewData["username"]) %></b>.
    </p>
    <p>
        Your password has been sucessfully reset to: <b><%=Html.Encode(ViewData["newPassword"]) %></b>
        <br />
        Please be sure to note the new password as you will need it the next time you login.
    </p>
    <% using (Html.BeginForm("Login", "Account")) { %>
        <p>
            <%=Html.Hidden("username", ViewData["username"]) %>
            <%=Html.Hidden("password", ViewData["newPassword"]) %>
            <input type="submit" value="Continue" />
        </p>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
