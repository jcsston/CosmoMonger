<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Ignore List</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Ignore List</h1>
    <p>Messages sent by users on your ignore list are ignored and discarded. The ignored user is not notified.</p>
    <ul>
<% 
    IgnoreList[] ignoreList = (IgnoreList[])ViewData["IgnoreList"];

    if (ignoreList.Length > 0)
    {
        foreach (IgnoreList ignore in ignoreList)
        {
            %><li><%=ignore.AntiFriend.UserName%> 
            <%=Html.ActionLink("Remove", "RemoveIgnore", new { antiBuddyId = ignore.AntiFriendId })%></li><% 
        }
    }
    else
    {
        %><li>Empty</li><% 
    }
%>
</ul>
<h3>Add user to ignore list</h3>
<%
    using (Html.BeginForm("FindPlayer", "BuddyList", FormMethod.Get))
    {
%>
    <p>
        <label for="name">Name: </label><%=Html.TextBox("name") %>
        <input type="submit" value="Find" />
    </p>
<%
    }        
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
