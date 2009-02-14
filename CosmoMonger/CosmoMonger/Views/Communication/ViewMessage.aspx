<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>View Message</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>View Message</h1>
<br />
<table width="100%" border="1" cellpadding="2">
    <tr>
        <td>From:</td>
        <td><%=Html.Encode(ViewData["From"]) %></td>
        <td>To:</td>
        <td><%=Html.Encode(ViewData["To"]) %></td>
    </tr>
    <tr>
        <td>Subject:</td>
        <td colspan="2"><%=Html.Encode(ViewData["Subject"]) %></td>
        <td><%=Html.Encode(ViewData["Time"]) %></td>
    </tr>
    <tr>
        <td colspan="4"><pre style="font-size: 1.5em;"><%=Html.Encode(ViewData["Content"]) %></pre></td>
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
