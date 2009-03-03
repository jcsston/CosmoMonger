<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>List Top Player Records</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Top Records</h1>
<br />
<h3>Please select a record for display...</h3> 
<% using (Html.BeginForm("ListRecords", "PlayerRecord")) { %>

<p>
<%=Html.DropDownList("recordType", new { onchange = "form.submit();" })%>

<input type="submit" value="Refresh" />
</p>
<% } %>

<table class="bigTable">
 <tr>
    <td class="lr-columnEmpty">&nbsp;</td>
    <td class="lr-columnHeaders">(User Name) - Player Name</td>
    <td class="lr-columnHeaders"><% 
            switch ((string)ViewData["SelectedRecordType"])
            {
                case "NetWorth":
                    Response.Write("Net Worth");
                    break;
                case "BountyTotal":
                    Response.Write("Bounties Collected");
                    break;
                case "HighestBounty":
                    Response.Write("Highest Bounty");
                    break;
                case "ShipsDestroyed":
                    Response.Write("Opponent's Destroyed");
                    break;
                case "ForcedSurrenders":
                    Response.Write("Surrendered Opponents");
                    break;
                case "ForcedFlees":
                    Response.Write("Fled Opponents");
                    break;
                case "CargoLooted":
                    Response.Write("Captured Cargo");
                    break;
                case "ShipsLost":
                    Response.Write("Ships Lost");
                    break;
                case "SurrenderCount":
                    Response.Write("Times Surrendered");
                    break;
                case "FleeCount":
                    Response.Write("Times Fled");
                    break;
                case "CargoLost":
                    Response.Write("Lost Cargo");
                    break;
                default:
                    throw new ArgumentException("Invalid recordType in GetTopPlayers", "recordType");
            }%></td>
      <td class="lr-columnEmpty">&nbsp;</td>      
 </tr>
<% foreach (CosmoMonger.Models.Player player in (CosmoMonger.Models.Player[])ViewData["TopRecords"])
{  %>
 <tr>
    <td class="lr-columnEmpty">&nbsp;</td>
    <td class="lr-columnLeft">(<%=player.User.UserName %>) - <%= player.Name %></td>
    <td class="lr-columnRight"><% 
            switch ((string)ViewData["SelectedRecordType"])
            {
                case "NetWorth":
                    Response.Write("$" + player.NetWorth);
                    break;
                case "BountyTotal":
                    Response.Write("$" + player.BountyTotal);
                    break;
                case "HighestBounty":
                    Response.Write("$" + player.HighestBounty);
                    break;
                case "ShipsDestroyed":
                    Response.Write(player.ShipsDestroyed);
                    break;
                case "ForcedSurrenders":
                    Response.Write(player.ForcedSurrenders);
                    break;
                case "ForcedFlees":
                    Response.Write(player.ForcedFlees);
                    break;
                case "CargoLooted":
                    Response.Write("$" + player.CargoLootedWorth);
                    break;
                case "ShipsLost":
                    Response.Write(player.ShipsLost);
                    break;
                case "SurrenderCount":
                    Response.Write(player.SurrenderCount);
                    break;
                case "FleeCount":
                    Response.Write(player.FleeCount);
                    break;
                case "CargoLost":
                    Response.Write("$" + player.CargoLostWorth);
                    break;
                default:
                    throw new ArgumentException("Invalid recordType in GetTopPlayers", "recordType");
            } %></td>
    <td class="lr-columnEmpty">&nbsp;</td>
 </tr>
 <% } %>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
