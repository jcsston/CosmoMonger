<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Dead Player</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Your Player's Seven Day Time Limit Has Expired!!</h1>
<br />
<table class="bigTable">
<tr>
<td class="columns45">&nbsp;</td>
<td><img id="tombstone"  src="/Content/tombstone1.png" alt="tombstone image" width="100" height="100" style="margin-left: 0px; align=center;"/></td>
<td class="columns45">&nbsp;</td>
</tr>
</table>
<p class="center">The CosmoMonger Team invites you to continue playing our game by creating a new player...</p>
<p class="center"><%= Html.ActionLink("Create A New Player", "CreatePlayer", "Player")%></p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
