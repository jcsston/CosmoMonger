<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>List Top Player Records</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Top Records</h1>
<br />
<h3>Please select a record for display...</h3> 
<% 
SelectList recordType = (SelectList)ViewData["recordType"];
using (Html.BeginForm("ListRecords", "PlayerRecord")) { 
%>
<p>
<%=Html.DropDownList("recordType", recordType, new { onchange = "form.submit();" })%>
<input type="submit" value="Refresh" />
</p>
<% } %>

<table class="bigTable">
 <tr>
    <td class="lr-columnEmpty">&nbsp;</td>
    <td class="lr-columnHeaders">Player Name - (User Name)</td>
    <td class="lr-columnHeaders"><%
            switch ((Player.RecordType)recordType.SelectedValue)
            {
                case Player.RecordType.Bounty:
                    Response.Write("Player Bounty");
                    break;
                case Player.RecordType.ShipsDestroyed:
                    Response.Write("Opponent's Destroyed");
                    break;
                case Player.RecordType.ForcedSurrenders:
                    Response.Write("Surrendered Opponents");
                    break;
                case Player.RecordType.ForcedFlees:
                    Response.Write("Fled Opponents");
                    break;
                case Player.RecordType.CargoLootedWorth:
                    Response.Write("Captured Cargo");
                    break;
                case Player.RecordType.SurrenderCount:
                    Response.Write("Times Surrendered");
                    break;
                case Player.RecordType.FleeCount:
                    Response.Write("Times Fled");
                    break;
                case Player.RecordType.CargoLostWorth:
                    Response.Write("Lost Cargo");
                    break;
                default:
                    Response.Write(Html.Encode(ViewData["SelectedRecordType"]));
                    break;
                    
            } %></td>
      <td class="lr-columnEmpty">&nbsp;</td>      
 </tr>
<% foreach (KeyValuePair<Player, string> record in (KeyValuePair<Player, string>[])ViewData["TopRecords"])
{  %>
 <tr>
    <td class="lr-columnEmpty">&nbsp;</td>
    <td class="lr-columnLeft"><%=Html.Encode(record.Key.Name) %> - (<%=Html.Encode(record.Key.User.UserName) %>)</td>
    <td class="lr-columnRight"><%=Html.Encode(record.Value) %></td>
    <td class="lr-columnEmpty">&nbsp;</td>
 </tr>
 <% } %>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
