<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Messaging - Inbox</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Messaging</h1>
<p><b>Inbox</b> | <%=Html.ActionLink("Sent", "Sent")%> | <%=Html.ActionLink("Compose", "Compose")%></p>
<%
Html.Grid<Message>(
    "Messages",
    new Hash(empty => "No messages"),
	column => {
        column.For(m => m.Time);
        column.For(m => m.SenderUser.UserName, "From");
        column.For(m => m.Received, "Read");
        column.For(m => Html.ActionLink(m.Subject, "ViewMessage", new { messageId = m.MessageId }), "Subject").DoNotEncode();
	}
);
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
