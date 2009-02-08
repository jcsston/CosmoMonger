<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Find Buddy</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Search Results</h1>
<%
Html.Grid<Player>(
	"Matches",
    new Hash(empty => "No matches"),
	column => {
        column.For(p => Html.ActionLink(p.Name, "AddBuddy", new { userId = p.UserId }), "Name").DoNotEncode();
		column.For(p => p.User.UserName);
        column.For(p => p.Race.Name, "Race");
	}
);
%>

<h2>Search Again</h2>
<%
    using (Html.BeginForm("FindBuddy", "BuddyList", FormMethod.Get))
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
