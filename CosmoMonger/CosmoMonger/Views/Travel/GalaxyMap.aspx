<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Galaxy Map</h1>
    <script language="javascript" type="text/javascript">
        function selectSystem(selectedSystemId) {
            var systemDiv;
            <% 
               foreach (CosmoMonger.Models.CosmoSystem system in (CosmoMonger.Models.CosmoSystem[])ViewData["Systems"])
               { %>
                systemDiv = document.getElementById('system<%=system.SystemId %>');
                if (systemDiv) 
                {
                    systemDiv.style.display = 'none';
                }
            <% } %>
            systemDiv = document.getElementById('system' + selectedSystemId);
            if (systemDiv)
            {
                systemDiv.style.display = '';
            }
        }
    </script>
    <table>
        <tr>
            <td>
                <div id="map" style="position: relative; width: 400px; height: 400px; border: solid 2px blue; overflow: hidden;">
                <ul>
                <% int galaxySize = (int)ViewData["GalaxySize"];
                   foreach (CosmoMonger.Models.CosmoSystem system in (CosmoMonger.Models.CosmoSystem[])ViewData["Systems"])
                   {
                       int x = (system.PositionX * (95 / galaxySize)) + 5;
                       int y = (system.PositionY * (95 / galaxySize)) + 5;
                %>
                    <li style="position: absolute; left: <%= x %>%; top: <%= y %>%;">
                        <a style="<% 
                            if (system == ViewData["CurrentSystem"])  
                            {
                                %> color: red; <% 
                            } 
                            %>" href="javascript:selectSystem(<%=system.SystemId %>)">
                            <%=Html.Encode(system.Name) %>
                        </a>
                    </li>
                <% } %>
                </ul>
                </div>
            </td>
            <td valign="top">
                Selected system details:
                <% 
                   foreach (CosmoMonger.Models.CosmoSystem system in (CosmoMonger.Models.CosmoSystem[])ViewData["Systems"])
                   {
                %>
                    <div id="system<%=system.SystemId %>" style="padding: 4px; display: none; border: solid 2px black;">
                    <b><%=Html.Encode(system.Name) %></b>
                    <hr />
                    <% if (system.Races.Count > 0)
                       { %>
                    <p>Home System of the <%=Html.Encode(system.Races.First().Name)%> Race</p>
                    <% } %>
                    Bank: <%=system.HasBank ? "Yes" : "No" %> <br />
                    <% 
                        if (system.GetGoods().Length > 0)
                        {
                    %>
                    Traded Goods
                    <ul>
                    <%
                            foreach (CosmoMonger.Models.SystemGood good in system.GetGoods())
                            {
                    %>
                        <li><%=Html.Encode(good.Good.Name)%> @ <%=good.Price%></li>
                    <%     } %>
                    </ul>
                    <%  } 
                        if (system == ViewData["CurrentSystem"])  
                        {
                        %>
                            <p>You are currently in this system.</p>
                            <script language="javascript" type="text/javascript">
                                document.onload = selectSystem(<%=system.SystemId %>);
                            </script>
                        <% 
                        } 
                        %>
                    </div>
                <% } %>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
