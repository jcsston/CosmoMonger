<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Player Profile</h1>
<table border="1">
<% CosmoMonger.Models.Player player = (CosmoMonger.Models.Player)ViewData["Player"]; %>
<tr><td>Name:</td><td><%= player.Name %></td></tr>
<tr><td>Race:</td><td><%= player.Race.Name %></td></tr>
<tr><td>Net Worth:</td><td><%= player.NetWorth %></td></tr>
<tr><td>Alive:</td><td><%= player.Alive %></td></tr>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
