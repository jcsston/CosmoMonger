<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<% CosmoMonger.Models.Player player = (CosmoMonger.Models.Player)ViewData["CurrentPlayer"]; %>
<% CosmoMonger.Models.CosmoSystem cSystem = ViewData["CurrentSystem"] as CosmoMonger.Models.CosmoSystem; %>
<% if (cSystem.HasBank == true)
   { %> 
<h1><%= Html.Encode(player.Name)%>, Welcome To The Intergalactic Bank of <%= Html.Encode(cSystem.Name)%></h1>
<% } %>
<% else
    { %>
<h1><%= Html.Encode(player.Name)%>, Welcome To The Online Intergalactic Bank!!!</h1>
<% } %>
<table style="width: 100%">
<tr>
    <td class="bank-columnHeaders" colspan="3"><%= Html.Encode(player.Name) %>'s Bank Account</td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td class="bank-columnHeaders">Account Balance</td>
</tr>
<tr>
    <td class="bank-data">$&nbsp;<%= Html.Encode(player.BankCredits) %></td>
</tr>
<% if (cSystem.HasBank == true)
   { %>
<tr><td class="bank-columnHeaders">Cash Available for Deposit</td>
</tr>
<tr>
    <td class="bank-data">$&nbsp;<%= Html.Encode(player.CashCredits) %></td>
</tr>
<tr>
    <td class="bank-data"><%= Html.ActionLink("Deposit Credits", "Deposit", "Bank")%></td>
</tr>
<tr>
    <td class="bank-data"><%= Html.ActionLink("Withdraw Credits", "Withdraw", "Bank")%></td>
</tr>

<% } %> 
<% else
    { %>
<tr>
    <td class="bank-data">If you would like to make a deposit or a withdrawl, you must travel to a system with a bank.</td>
</tr>
<% } %>
</table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
