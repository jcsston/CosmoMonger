﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Messaging - Sent</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Messaging</h1>
<p><%=Html.ActionLink("Inbox", "Inbox") %> | <b>Sent</b> | <%=Html.ActionLink("Compose", "Compose")%></p>
<%
Html.Grid<Message>(
    "Messages",
    new Hash(empty => "No messages sent"),
	column => {
        column.For(m => m.Time);
        column.For(m => m.RecipientUser.UserName, "To");
        column.For(m => Html.ActionLink(m.Subject, "ViewMessage", new { messageId = m.MessageId }), "Subject").DoNotEncode();
	}
);
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
