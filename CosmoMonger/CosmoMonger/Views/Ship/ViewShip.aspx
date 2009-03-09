<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Current Ship</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<%
    Ship currentShip = (Ship)ViewData["Ship"];
%>
<h1>Current Ship</h1>
<div style="float: right; text-align: center">
    <img alt="Ship Image" src="/Content/BaseShip/<%=Html.AttributeEncode(currentShip.BaseShip.Name) %>.png" />
    <br />
    <%=Html.Encode(currentShip.BaseShip.Name) %>
</div>
<table class="equipment"><tr><td>
<table>
    <caption>Cargo Space</caption>
    <tr><th>Total</th><td><%=currentShip.CargoSpaceTotal %></td></tr>
    <tr><th>Free</th><td><%=currentShip.CargoSpaceFree %></td></tr>
</table>
<table>
    <caption>Jump Drive Specs</caption>
    <tr><th>Name</th><td><%=Html.Encode(currentShip.JumpDrive.Name) %></td></tr>
    <tr><th>Range</th><td><%=currentShip.JumpDrive.Range %></td></tr>
    <tr><th>Speed</th><td><%=currentShip.JumpDrive.ChargeTime%></td></tr>
    <tr><th>Cargo Cost</th><td><%=currentShip.JumpDrive.CargoCost %></td></tr>
</table>
<table>
    <caption>Shield Specs</caption>
    <tr><th>Name</th><td><%=Html.Encode(currentShip.Shield.Name) %></td></tr>
    <tr><th>Strength</th><td><%=currentShip.Shield.Strength %></td></tr>
    <tr><th>Cargo Cost</th><td><%=currentShip.Shield.CargoCost %></td></tr>
</table>
<table>
    <caption>Weapon Specs</caption>
    <tr><th>Name</th><td><%=Html.Encode(currentShip.Weapon.Name) %></td></tr>
    <tr><th>Power</th><td><%=currentShip.Weapon.Power %></td></tr>
    <tr><th>Turn Cost</th><td><%=currentShip.Weapon.TurnCost %></td></tr>
    <tr><th>Cargo Cost</th><td><%=currentShip.Weapon.CargoCost %></td></tr>
</table>
</td></tr></table>
<p>Trade-In Value: $<%=currentShip.TradeInValue %></p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
