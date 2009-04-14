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
<% 
}

Html.Grid<PlayerTopRecord>(
    "TopRecords",
    new Hash(empty => "No top records", @class => "grid topRecords"),
    column =>
    {
        column.For(r => string.Format("{0} - ({1})", r.Player.Name, r.Player.User.UserName), "Player Name - (Username)");
        column.For(r => r.FormattedRecordValue, ViewData["SelectedRecordType"].ToString());
    }
);
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
