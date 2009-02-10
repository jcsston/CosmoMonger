<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Messaging - Compose</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Messaging</h1>
<p><%=Html.ActionLink("Inbox", "Inbox") %> | <%=Html.ActionLink("Sent", "Sent")%> | <b>Compose</b></p>
<%
    using (Html.BeginForm()) 
    {
%>
    <p>To: <%=Html.DropDownList("toUserId")%></p>
    <p><%=Html.TextArea("Message", (string)ViewData["Message"], 10, 40, null)%></p>
    <p><input type="submit" value="Compose" /></p>
<% 
    }        
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
