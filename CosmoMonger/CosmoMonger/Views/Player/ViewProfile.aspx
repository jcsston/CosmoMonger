<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="ViewProfile.aspx.cs" Inherits="CosmoMonger.Views.Player.ViewProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h2>Player Profile</h2>
<table bgcolor="White" border="1">
<tr><td>Name:</td><td><%=ViewData.Model.Name %></td></tr>
<tr><td>Race:</td><td><%=ViewData.Model.Race.Name %></td></tr>
<tr><td>Net Worth:</td><td><%=ViewData.Model.NetWorth %></td></tr>
<tr><td>Alive:</td><td><%=ViewData.Model.Alive %></td></tr>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
