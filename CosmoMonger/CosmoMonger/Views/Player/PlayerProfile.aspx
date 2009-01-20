<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server">
<h1>Player Profile</h1>
<table style="width: 100%">
<% CosmoMonger.Models.Player player = (CosmoMonger.Models.Player)ViewData["Player"]; %>
<tr><td class="vp-playerName" colspan="6"><%= player.Name%></td></tr>
<tr><td>&nbsp;</td></tr>
<tr>
    <td class="vp-headers" colspan="2">Financial Data</td>
    <td class="vp-headers" colspan="2">Reputation</td>
    <td class="vp-headers" colspan="2">Bounty</td>
</tr>
<tr>
    <td class="vp-columnData">Net Worth:</td>
    <td class="vp-columnData"><%= player.NetWorth%></td>
    <td class="vp-columnData"><%=player.Reputation%></td>
    <td class="vp-columnData"><%=player.ReputationLevel%></td>
    <td class="vp-columnData"><%=player.BountyTotal%></td>
    <td class="vp-columnData">Credits</td>
</tr>
<tr>
    <td class="vp-columnData">Cash Credits:</td>
    <td class="vp-columnData"><%= player.CashCredits%></td>
</tr>
<tr>
    <td class="vp-columnData">Bank Credits:</td>
    <td class="vp-columnData"><%= player.BankCredits%></td>
</tr>
<tr><td>&nbsp;</td></tr>
<tr>
    <td class="vp-headers" colspan="2">Racial Data</td>
    <td class="vp-headers" colspan="2">Time Played</td>
    <td class="vp-headers" colspan="2">Want A New Player?</td>
</tr>
<tr>
    <td class="vp-columnData">Player's Race:</td>
    <td class="vp-columnData"><%= player.Race.Name%></td>
    <td class="vp-columnData"><%= player.TimePlayed%></td>
    <td class="vp-columnData">hours</td>
    <td align="center" colspan="2">Warning! This is irrevisible!</td>
</tr>
<tr>
    <td class="vp-columnData">Discount/Bonus:</td>
    <% string racePref;
       if (player.Race.RacialPreference == null)
       {
           racePref = "None";
       }
       else
       {
           racePref = player.Race.RacialPreference.Name + "s";
       }
           
            %>
    <td class="vp-columnData"><%=racePref%></td>
    <td colspan="2"></td><td align="center" colspan="2">
    <%=Html.ActionLink("Kill Current Player", "KillPlayer", new { playerId = player.PlayerId }, new { onclick = "return confirm('Are you sure you want to kill your player?');" })%>
</tr>
<tr>
    <td class="vp-columnData">Surcharge/Penalty:</td>
    <td class="vp-columnData"><%=player.Race.RacialEnemy.Name%>s</td>
</tr>
</table>
    </form>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
