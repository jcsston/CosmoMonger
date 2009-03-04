<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Combat Complete</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1><%=Html.Encode(ViewData["Message"]) %></h1>
<h2 class="goods">Results of Combat</h2>
<%
if (ViewData["CargoLooted"] != null)
{
    Html.Grid<CombatGood>(
        "CargoLooted",
        new Hash(empty => "No cargo looted", @class => "goods"),
        column =>
        {
            column.For(g => g.Good.Name, "Good Type");
            column.For(g => g.Quantity, "Quantity Found");
            column.For(g => g.QuantityPickedUp, "Quantity Picked Up");
            column.For(g => "$" + (g.QuantityPickedUp * g.Good.BasePrice), "Picked Up Cargo Worth");
        }
    );
    %><p>Credits Looted: $<%=Html.Encode(ViewData["CreditsLooted"])%></p><%
}
else if (ViewData["CargoLost"] != null)
{
    Html.Grid<CombatGood>(
        "CargoLost",
        new Hash(empty => "No cargo lost", @class => "goods"),
        column =>
        {
            column.For(g => g.Good.Name, "Good Type");
            column.For(g => g.Quantity, "Quantity Lost");
            column.For(g => "$" + (g.Quantity * g.Good.BasePrice), "Lost Cargo Worth");
        }
    );
    %><p>Credits Lost: $<%=Html.Encode(ViewData["CreditsLooted"])%></p><%
}
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
