<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>View Player Record</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>View Record</h1>
<% Player player = (Player)ViewData["Player"]; %>
<table style="width: 100%">
<tr>
    <td class="vr-playerName" colspan="3"><%= Html.Encode(player.Name) %></td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td class="vr-highBounty">Bounty</td>
    <td class="vr-highBounty">Distance Traveled</td>
    <td class="vr-highBounty">Goods Traded</td>
</tr>
<tr>
    <td class="vr-highBounty"><%= player.Bounty.ToString("C0")%></td>
    <td class="vr-highBounty"><%= Html.Encode(player.DistanceTraveled.ToString("N02"))%> sectors</td>
    <td class="vr-highBounty"><%= Html.Encode(player.GoodsTraded) %> sold</td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td class="vr-columnHeaders">Victory Records</td>
    <td>&nbsp;</td>
    <td class="vr-columnHeaders">Defeat Records</td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td class="vr-columns">Ships You Destroyed</td>
    <td class="vr-columnMiddle">&nbsp;</td>
    <td class="vr-columns">Ships You Lost</td></tr>
<tr>
    <td class="vr-columns"><%= Html.Encode(player.ShipsDestroyed) %></td>
    <td class="vr-columnMiddle">&nbsp;</td>
    <td class="vr-columns"><%= Html.Encode(player.ShipsLost) %></td>
</tr>
<tr>
    <td class="vr-columns">Ships You Forced To Surrender</td>
    <td class="vr-columnMiddle">&nbsp;</td>
    <td class="vr-columns">Ships You Surrendered</td>
</tr>
<tr>
    <td class="vr-columns"><%= Html.Encode(player.ForcedSurrenders) %></td>
    <td class="vr-columnMiddle">&nbsp;</td>
    <td class="vr-columns"><%= Html.Encode(player.SurrenderCount) %></td>
</tr>
<tr>
    <td class="vr-columns">Ships You Forced To Flee</td>
    <td class="vr-columnMiddle">&nbsp;</td>
    <td class="vr-columns">Combats You Fled</td>
</tr>
<tr>
    <td class="vr-columns"><%= Html.Encode(player.ForcedFlees) %></td>
    <td class="vr-columnMiddle">&nbsp;</td>
    <td class="vr-columns"><%= Html.Encode(player.FleeCount) %></td>
</tr>
<tr>
    <td class="vr-columns">Value Of Cargo You Looted</td>
    <td class="vr-columnMiddle">&nbsp;</td>
    <td class="vr-columns">Value Of Cargo You Lost</td>
</tr>
<tr>
    <td class="vr-columns"><%= player.CargoLootedWorth.ToString("C0") %></td>
    <td class="vr-columnMiddle">&nbsp;</td>
    <td class="vr-columns"><%= player.CargoLostWorth.ToString("C0") %></td>
</tr>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
