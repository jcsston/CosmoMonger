<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Dead Player</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Your Player's Seven Day Time Limit Has Expired!!</h1>
<br />
<div class="center deadPlayer">
<img src="/Content/tombstone1.png" alt="tombstone image" />
</div>
<p class="center">The CosmoMonger Team invites you to continue playing our game by creating a new player...</p>
<p class="center"><%= Html.ActionLink("Create A New Player", "CreatePlayer", "Player")%></p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
