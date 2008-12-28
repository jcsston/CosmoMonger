<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="GalaxyMap.aspx.cs" Inherits="CosmoMonger.Views.Travel.GalaxyMap" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Galaxy Map</h1>
    <div id="map" style="position: relative; width: 800px; height: 400px; border: solid 2px blue; overflow: hidden;">
    <ul>
    <% int galaxySize = (int)ViewData["GalaxySize"];
       foreach (CosmoMonger.Models.CosmoSystem system in (CosmoMonger.Models.CosmoSystem[])ViewData["Systems"])
       {
           int x = (system.PositionX * (95 / galaxySize)) + 5;
           int y = (system.PositionY * (95 / galaxySize)) + 5;
    %>
        <li style="position: absolute; left: <%= x %>%; top: <%= y %>%">
            <div <% if (system == ViewData["CurrentSystem"]) { %> style="color: red;" <% } %> onmouseover="document.getElementById('<%=Html.Encode(system.Name)%>').style.display = '';" onmouseout="document.getElementById('<%=Html.Encode(system.Name)%>').style.display = 'none';">
                <%=Html.Encode(system.Name) %>
            </div>
            <div id="<%=Html.Encode(system.Name) %>" style="padding: 2px; display: none; border: solid 1px black; background: #1D60FF none;">
            Good Types: <%=system.GetGoods().Length %> <br />
            Bank: <%=system.HasBank ? "Yes" : "No" %>
            </div>
        </li>
    <% } %>
    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
