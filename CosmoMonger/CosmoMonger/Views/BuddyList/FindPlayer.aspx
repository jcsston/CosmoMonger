<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" CompilerOptions="/nowarn:0618" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Find Player</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Search Results</h1>
<%
// Note: if your tag is a reserved work, like "class", use the standard @ reserved word prefix: "@class"
Html.Grid<Player>(
	"Matches",
    new Hash(empty => "No matches", @class => "grid"),
	column => {
        column.For(p => p.Name, "Player Name").DoNotEncode();
		column.For(p => p.User.UserName);
        column.For(p => p.Race.Name, "Race");
        column.For(p => Html.ActionLink("Buddy", "AddBuddy", new { userId = p.UserId }), "Add").DoNotEncode();
        column.For(p => Html.ActionLink("Ignore", "AddIgnore", new { userId = p.UserId }), "Add").DoNotEncode();
	}
);
%>

<h2>Search Again</h2>
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
