<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Start Chat</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Start Chat</h1>
<% 
    using (Html.BeginForm("Chat", "Chat"))
    { 
%>
    <p>
        Chat: <%=Html.DropDownList("friendId")%>
    </p>
    <p>
        <input type="submit" value="Chat" />
    </p>
<%
    } 
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
