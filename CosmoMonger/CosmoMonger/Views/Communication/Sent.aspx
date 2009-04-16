<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Messaging - Sent</title>
    <script type="text/javascript" src="/Scripts/jquery.confirm-1.2.js"></script>
    <script type="text/javascript">
    //<![CDATA[
        $(document).ready(function() {
            $("tr.gridrow td:first-child").datetimeUTCtoLocal();
            $("a.delete").click(function() { document.location = this.href; });
            $("a.delete").confirm();
        });
    //]]>
    </script>
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
        column.For(m => Html.ActionLink(m.Subject, "ViewMessage", new { messageId = m.MessageId, sent = true }), "Subject").DoNotEncode();
        column.For(m => Html.ActionLink("Delete", "DeleteMessage", "Communication", new { messageId = m.MessageId, sent=true }, new { @class = "delete ui-icon ui-icon-trash" }), "Delete?").DoNotEncode();
	}
);
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
