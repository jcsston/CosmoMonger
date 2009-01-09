<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Travel</h1>
    <div id="map" style="position: relative; width: 400px; height: 400px; overflow: hidden; background-image: url('/Content/ShipRangeCircle.png');">
    <ul>
    <% CosmoMonger.Models.CosmoSystem currentSystem = (CosmoMonger.Models.CosmoSystem)ViewData["CurrentSystem"]; %>
        <li style="position: absolute; left: 50%; top: 50%;">
            <div style="color: red;" onmouseover="document.getElementById('system<%=currentSystem.SystemId %>').style.display = '';" onmouseout="document.getElementById('system<%=currentSystem.SystemId %>').style.display = 'none';">
                <%=Html.Encode(currentSystem.Name)%>
            </div>
            <div id="system<%=currentSystem.SystemId %>" style="padding: 2px; display: none; border: solid 1px black; background: #1D60FF none;">
            Good Types: <%=currentSystem.GetGoods().Length%> <br />
            Bank: <%=currentSystem.HasBank ? "Yes" : "No"%>
            </div>
        </li>
    <% 
       int shipRange = (int)ViewData["Range"];
       foreach (CosmoMonger.Models.CosmoSystem system in (CosmoMonger.Models.CosmoSystem[])ViewData["Systems"])
       {
           int x = (system.PositionX * (95 / shipRange)) + 5;
           int y = (system.PositionY * (95 / shipRange)) + 5;
    %>
        <li style="position: absolute; left: <%= x %>%; top: <%= y %>%;">
            <a href="Travel/<%=system.SystemId %>" style="<% 
                if (system == ViewData["CurrentSystem"])  
                {
                    %> color: red; <% 
                } 
                %>" onmouseover="document.getElementById('system<%=system.SystemId %>').style.display = '';" onmouseout="document.getElementById('system<%=system.SystemId %>').style.display = 'none';">
                <%=Html.Encode(system.Name) %>
            </a>
            <div id="system<%=system.SystemId %>" style="padding: 2px; display: none; border: solid 1px black; background: #1D60FF none;">
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
