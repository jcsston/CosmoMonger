<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1 class="goods">Good Price Table</h1>
<table class="goods">
    <tr>
        <th>System</th>
<% 
    CosmoSystem currentSystem = (CosmoSystem)ViewData["CurrentSystem"];
    List<PriceTableEntry> priceTable = (List<PriceTableEntry>)ViewData["PriceTable"];

    // Build the table header
    foreach (KeyValuePair<string, int> goodPrice in priceTable[0].GoodPrices.OrderBy(g => g.Key))
    {
%>
        <th><%=Html.Encode(goodPrice.Key) %></th>
<%
    }
%>
    </tr>
<%
    foreach (PriceTableEntry entry in priceTable)
    {
%>
    <tr>
        <td><b><%=Html.Encode(entry.SystemName)%></b></td>
<%
        foreach (KeyValuePair<string, int> goodPrice in entry.GoodPrices.OrderBy(g => g.Key))
        {
%>       
            <td><%=goodPrice.Value != 0 ? goodPrice.Value.ToString() : "N/A"%></td>
<%
        }
%> 
    </tr>
<% 
    }
%>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
