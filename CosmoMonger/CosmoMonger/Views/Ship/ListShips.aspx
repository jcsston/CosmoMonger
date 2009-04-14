<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Available Ships</title>
    <script type="text/javascript" src="/Scripts/jquery.tooltip.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.bgiframe.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.dimensions.js"></script>
    <script type="text/javascript">
    <!--
        $(document).ready(function() {
            // Show image of ship
            $('.shipImage').tooltip({
                showURL: false,
                extraClass: "good-tooltip",
                bodyHandler: function() {
                    return $("<img/>").attr("src", this.src);
                }
            });
        });
    -->
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Ships for Purchase</h1>
<table class="goods">
<tr><th>Name</th><th>Cargo Space</th><th>Level</th><th>Price</th><th>Buy</th></tr>
<%
    Ship currentShip = (Ship)ViewData["CurrentShip"];
    SystemShip[] ships = (SystemShip[])ViewData["Ships"];

    foreach (SystemShip ship in ships)
    {
%>
    <tr>
        <td><img class="shipImage" src="/Content/BaseShip/<%=Html.AttributeEncode(ship.BaseShip.Name) %>.png" alt="<%=Html.AttributeEncode(ship.BaseShip.Name) %>" width="75"/> <%=Html.Encode(ship.BaseShip.Name) %></td>
        <td><%=ship.BaseShip.CargoSpace %></td>
        <td><%=ship.BaseShip.Level %></td>
        <td><%=ship.Price.ToString("C0") %></td>
        <td>
        <%
            if (ship.BaseShip != currentShip.BaseShip)
            {
                string disabled = "";
                if (ship.Price > (currentShip.Credits + currentShip.TradeInValue)) 
                {
                    // Not enough to buy
                    disabled = "disabled=\"disabled\"";
                }
                using (Html.BeginForm("BuyShip", "Ship"))
                {
                    %><div><%=Html.Hidden("shipId", ship.BaseShipId, new { id = "shipId" + ship.BaseShipId })
                    %><input type="submit" value="Buy" <%=disabled %>/></div><%
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
<table class="goods goodsCenter">
    <tr><th>Ship Trade-In Value</th><th>Credits</th><th>Bank Credits</th><th>Cargo Space Free</th></tr>
    <tr>
        <td><%= ((int)ViewData["TradeInValue"]).ToString("C0")%></td>
        <td><%= ((int)ViewData["CashCredits"]).ToString("C0") %></td>
        <td><%= ((int)ViewData["BankCredits"]).ToString("C0") %></td>
        <td id="FreeCargoSpace"><%= ViewData["FreeCargoSpace"] %></td>
    </tr>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
