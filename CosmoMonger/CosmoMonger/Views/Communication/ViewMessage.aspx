<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>View Message</title>
    <script type="text/javascript">
    //<![CDATA[
        $(document).ready(function() {
            $("#messageTime").datetimeUTCtoLocal();
        });
    //]]>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>View Message</h1>
<br />
<table class="message">
    <tr>
        <td>From:</td>
        <td><%=Html.Encode(ViewData["From"]) %></td>
        <td>To:</td>
        <td><%=Html.Encode(ViewData["To"]) %></td>
    </tr>
    <tr>
        <td>Subject:</td>
        <td colspan="2"><%=Html.Encode(ViewData["Subject"]) %></td>
        <td id="messageTime"><%=Html.Encode(ViewData["Time"]) %></td>
    </tr>
    <tr>
        <td colspan="4"><pre class="messageBody"><%=Html.Encode(ViewData["Content"]) %></pre></td>
    </tr>
</table>
<p>
Back to <% 
    if ((bool)ViewData["Sent"])
    { 
        %><%=Html.ActionLink("Sent", "Sent") %><% 
    }
    else
    { 
        %><%=Html.ActionLink("Inbox", "Inbox")%><% 
    }
%> messages.
</p>

<% 
if (!(bool)ViewData["Sent"])
{
    
%><p> <%=Html.ActionLink("Reply", "Compose", new { toUserId = ViewData["SenderId"] })%></p><%


}
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
