<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Good Price Table</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1 class="goods">Good Price Table</h1>
<table class="goods">
    <tr>
        <th>System</th>
<% 
    CosmoSystem currentSystem = (CosmoSystem)ViewData["CurrentSystem"];
    List<PriceTableEntry> priceTable = (List<PriceTableEntry>)ViewData["PriceTable"];

    // Build the table header
    foreach (KeyValuePair<string, int> goodPrice in priceTable[0].GoodPrices)
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
    <tr class="<%=(currentSystem.Name == entry.SystemName) ? "priceTableCurrentSystem" : "" %>">
        <td><b><%=Html.Encode(entry.SystemName)%></b></td>
<%
        foreach (KeyValuePair<string, int> goodPrice in entry.GoodPrices)
        {
            int maxPrice = priceTable.Max(g => g.GoodPrices[goodPrice.Key]);
            int minPrice = priceTable.Where(g => g.GoodPrices[goodPrice.Key] > 0).Min(g => g.GoodPrices[goodPrice.Key]);
%>       
            <td class="<%=(goodPrice.Value == maxPrice) ? "priceTableHigh" : (goodPrice.Value == minPrice) ? "priceTableLow" : "" %>"><%=goodPrice.Value != 0 ? goodPrice.Value.ToString() : "N/A"%></td>
<%
        }
%> 
    </tr>
<% 
    }
%>
</table>
<p class="center">
Legend:
<br />
<span class="priceTableLow">The Lowest Prices for a good are marked like this. </span>
<br />
<span class="priceTableHigh">The Highest Prices for a good are marked like this.</span>
<br />
<span class="priceTableCurrentSystem">The System you are located in is marked like this.</span>
</p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
