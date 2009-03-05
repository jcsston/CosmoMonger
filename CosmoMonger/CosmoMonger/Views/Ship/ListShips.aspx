<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Available Ships</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Ships for Purchase</h1>
<table class="goods">
<tr><th>Name</th><th>Cargo Space</th><th>Range</th><th>Price</th><th>Buy</th></tr>
<%
    Ship currentShip = (Ship)ViewData["CurrentShip"];
    SystemShip[] ships = (SystemShip[])ViewData["Ships"];

    foreach (SystemShip ship in ships)
    {
%>
    <tr>
        <td><%=Html.Encode(ship.BaseShip.Name) %></td>
        <td><%=ship.BaseShip.CargoSpace %></td>
        <td><%=ship.BaseShip.InitialJumpDrive.Range %></td>
        <td>$<%=ship.Price %></td>
        <td>
        <%
            if (ship.BaseShip != currentShip.BaseShip)
            {
                using (Html.BeginForm("BuyShip", "Ship"))
                {
                    %><div><%=Html.Hidden("shipId", ship.BaseShipId, new { id = "shipId" + ship.BaseShipId })
                    %><input type="submit" value="Buy" /></div><%
                }
            }
            else
            {
                %>Current<%
            }
        %>
        </td>
    </tr>
<%
    }        
%>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
