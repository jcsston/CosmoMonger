<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Attack Ship</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Attack Ship</h1>
<%
// Note: if your tag is a reserved work, like "class", use the standard @ reserved word prefix: "@class"
Html.Grid<Ship>(
	"Ships",
    new Hash(empty => "No ships are currently leaving the system", @class => "grid"),
	column => {
        column.For(s => s.Players.Select(p => p.Name).SingleOrDefault(), "Player Name");
        column.For(s => s.BaseShip.Name, "Ship Type");
        column.For("Attack").Do(s =>
        {%><td><%
            using (Html.BeginForm())
            { 
            %><div>
                <input type="hidden" name="shipId" value="<%=s.ShipId %>" /> 
                <input type="submit" value="Attack" />
            </div><% 
            }
            %></td><%
        });
	}
);
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
