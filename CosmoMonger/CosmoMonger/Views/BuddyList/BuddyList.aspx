<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Buddy List</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Buddy List</h1>
    <p>You can only send messages to users on your buddy list.</p>
    <ul>
<% 
    BuddyList[] buddyList = (BuddyList[])ViewData["BuddyList"];

    if (buddyList.Length > 0)
    {
        foreach (BuddyList buddy in buddyList)
        {
            %><li><%=buddy.Friend.UserName %> 
            <%=Html.ActionLink("Remove", "RemoveBuddy", new { buddyId = buddy.FriendId }) %></li><% 
        }
    }
    else
    {
        %><li>Empty</li><% 
    }
%>
</ul>
<h3>Add another Buddy</h3>
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
