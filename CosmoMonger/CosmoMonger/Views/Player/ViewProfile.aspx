<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Player Profile</h1>
<table style="width: 100%">
<% CosmoMonger.Models.Player player = (CosmoMonger.Models.Player)ViewData["Player"]; %>
<tr><td align="center" colspan="4"><h2><%= player.Name%></h2></td></tr>
<tr></tr>
<tr>
    <td align="center" style="width: 33%" colspan="2"><h3>Financial</h3></td>
    <td align="center" style="width: 33%"><h3>Reputation</h3></td>
    <td align="center" style="width: 33%"><h3>Bounty</h3></td>
</tr>
<tr></tr>
<tr><td>Net Worth:</td><td><%= player.NetWorth%></td><td></td><td></td></tr>
<tr><td>Cash Credits:</td><td><%= player.CashCredits%></td><td></td><td></td></tr>
<tr><td>Bank Credits:</td><td><%= player.BankCredits%></td><td></td><td></td></tr>
<tr>
    <td align="center" style="width: 33%" colspan="2"><h3>Racial Data</h3></td>
    <td align="center" style="width: 33%"><h3>Time Played</h3></td>
    <td align="center" style="width: 33%"><h3>Status</h3></td>
</tr>
<tr>
    <td>Race:</td><td><%= player.Race.Name%></td>
    <td align="center"><%= player.TimePlayed%></td>
    <% string status = "Dead";%>
    <% if (player.Alive)
       {%>
       <%status = "Alive"; %>
     <% } %> %>  
    <td align="center"><%= status%></td>
</tr>
<tr><td></td><td></td><td></td><td></td></tr>
<tr><td></td><td></td></tr>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
