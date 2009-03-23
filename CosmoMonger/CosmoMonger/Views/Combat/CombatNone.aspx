<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true"  Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
<title>No Combat</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>No Combat</h1>
<%=Html.ValidationSummary() %>
<p>No combat is in progress.</p>
<p>Back to <%=Html.ActionLink("Attack", "Attack") %> page.</p>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
