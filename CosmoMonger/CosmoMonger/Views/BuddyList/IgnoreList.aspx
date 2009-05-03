<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" CompilerOptions="/nowarn:0618" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Ignore List</title>
    <script type="text/javascript" src="/Scripts/jquery.confirm-1.2.js"></script>
    <script type="text/javascript">
    //<![CDATA[
        $(document).ready(function() {
            $("a.delete").click(function() { document.location = this.href; });
            $("a.delete").confirm();
        });
    //]]>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Ignore List</h1>
    <p>Messages sent by users on your ignore list are ignored and discarded. The ignored user is not notified.</p>
<%
    Html.Grid<IgnoreList>(
    "IgnoreList",
    new Hash(empty => "Empty", @class => "grid center"),
	column => {
        column.For(b => b.AntiFriend.UserName, "Ignore User");
        column.For(b => b.AntiFriend.Players.Where(p => p.Alive).Select(p => p.Name).SingleOrDefault(), "Player Name");
        column.For(b => Html.ActionLink("Remove", "RemoveIgnore", new { antiBuddyId = b.AntiFriendId }, new { @class = "delete ui-icon ui-icon-closethick" }), "Remove?").DoNotEncode();
	}
);
%>
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
