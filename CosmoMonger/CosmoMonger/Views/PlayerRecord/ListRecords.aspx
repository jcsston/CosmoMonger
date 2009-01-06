<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>List Records</h1>
<select id="RecordType" name="Record Type">
<% foreach (string recordType in (string[])ViewData["RecordTypes"])
   { %>
    <option value="<%=recordType %>"><%=recordType %></option>
<% } %>
</select>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
