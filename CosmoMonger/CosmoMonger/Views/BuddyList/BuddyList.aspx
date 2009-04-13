<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Buddy List</title>
    <script type="text/javascript" src="/Scripts/jquery.confirm-1.2.js"></script>
    <script type="text/javascript">
        $(document).ready(function() {
            $("a.delete").click(function() { document.location = this.href; });
            $("a.delete").confirm();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Buddy List</h1>
    <p>You can only send messages to users on your buddy list.</p>
<%
    Html.Grid<BuddyList>(
    "BuddyList",
    new Hash(empty => "No buddies", @class => "grid center"),
	column => {
        column.For(b => b.Friend.UserName, "Buddy Username");
        column.For(b => b.Friend.Players.Where(p => p.Alive).Select(p => p.Name).SingleOrDefault(), "Player Name");
        column.For(b => Html.ActionLink("Remove", "RemoveBuddy", new { buddyId = b.FriendId }, new { @class = "delete ui-icon ui-icon-closethick" }), "Remove?").DoNotEncode();
	}
);
%>
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
