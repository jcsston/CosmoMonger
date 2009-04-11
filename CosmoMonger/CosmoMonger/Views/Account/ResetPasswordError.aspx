<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Reset Password Error</title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Reset Password Error</h2>

    <%= Html.ValidationSummary() %>
    
    <p>Back to <%=Html.ActionLink("login", "Login") %> page.</p>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
