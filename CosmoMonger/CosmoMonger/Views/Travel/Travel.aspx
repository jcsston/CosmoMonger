<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Travel</h1>
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
            document.getElementById('targetSystem').value = selectedSystemId;
        }
    </script>
    <% using (Html.BeginForm()) { %>
    <%=Html.Hidden("targetSystem")%>
    <%=Html.ValidationSummary() %>
    <table>
        <tr>
            <td>
                <%
                    CosmoMonger.Models.CosmoSystem currentSystem = ViewData["CurrentSystem"] as CosmoMonger.Models.CosmoSystem;
                    CosmoMonger.Models.CosmoSystem[] inRangeSystems = ViewData["InRangeSystems"] as CosmoMonger.Models.CosmoSystem[];
                    int galaxySize = (int)ViewData["GalaxySize"];
                    int displaySize = 400;
                    int currentPositionX = 0;
                    int currentPositionY = 0;
                    double pixelPerPoint = 1.0 * displaySize / galaxySize;
                %>
                <div id="map" style="position: relative; width: <%=displaySize%>px; height: <%=displaySize%>px; border: solid 2px blue; overflow: hidden; padding: 15px;">
                <ul>
                <% 
                   foreach (CosmoMonger.Models.CosmoSystem system in (ViewData["Systems"] as CosmoMonger.Models.CosmoSystem[]))
                   {
                       int x = (int)(system.PositionX * pixelPerPoint);
                       int y = (int)(system.PositionY * pixelPerPoint);
                %>
                    <a href="javascript:selectSystem(<%=system.SystemId %>)"
                    <img style="position: absolute; left: <%= x %>px; top: <%= y %>px; <% 
                            if (system == ViewData["CurrentSystem"])  
                            {
                                // Store the x/y of the player's current position
                                currentPositionX = x;
                                currentPositionY = y;
                                %> color: red; <% 
                            }
                            else if (inRangeSystems.Contains(system))
                            {
                                %> color: blue; <% 
                            }
                            else
                            {
                                %> border: none; <%
                            }
                            %>"
                            alt="<%=Html.AttributeEncode(system.Name)%>"
                            title="<%=Html.AttributeEncode(system.Name)%>"
                            src="/Content/System.png"
                            width="20px" height="20px" />
                     </a>
                <% } %>
                </ul>
                <%
                    // Calcuate the size and position of the in-range circle
                    int shipRange = (int)ViewData["Range"];
                    int shipRangeSize = (int)(pixelPerPoint * shipRange) * 2;
                    int shipRangeX = currentPositionX + 10 - (shipRangeSize / 2);
                    int shipRangeY = currentPositionY + 10 - (shipRangeSize / 2);
                %>
                <img id="shipRange" src="/Content/ShipRangeCircle.png" alt="Ship Range" 
                    style="position: absolute; left: <%=shipRangeX %>px; top: <%=shipRangeY%>px; z-index: -1;" 
                    width="<%=shipRangeSize %>px" 
                    height="<%=shipRangeSize %>px" />
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
                            <p style="color: red;">You are currently in this system.</p>
                            <script language="javascript" type="text/javascript">
                                document.onload = selectSystem(<%=system.SystemId %>);
                            </script>
                        <% 
                        }
                        else if (inRangeSystems.Contains(system))
                        {
                        %>
                            <p style="color: Blue;">You can travel to this system.</p>
                            <input type="submit" value="Travel" onclick="" />
                        <%
                        }
                        %>
                    </div>
                <% } %>
            </td>
        </tr>
    </table>
    <% } %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
