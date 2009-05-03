<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" CompilerOptions="/nowarn:0618" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Messaging - Inbox</title>
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
<p><b>Inbox</b> | <%=Html.ActionLink("Sent", "Sent")%> | <%=Html.ActionLink("Compose", "Compose")%></p>
<%
Html.Grid<Message>(
    "Messages",
    new Hash(empty => "No messages"),
	column => {
        column.For(m => m.Time);
        column.For(m => m.SenderUser.UserName, "From");
        column.For(m => Html.ActionLink(m.Subject, "ViewMessage", new { messageId = m.MessageId }), "Subject").DoNotEncode();
        column.For(m => m.Received, "Read");
        column.For(m => Html.ActionLink("Delete", "DeleteMessage", "Communication", new { messageId = m.MessageId, sent=false }, new { @class = "delete ui-icon ui-icon-trash" }),"Delete?").DoNotEncode();
        
        
	}
);
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
