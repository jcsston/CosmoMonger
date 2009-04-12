<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Travel Map</title>
    <%
        CosmoSystem currentSystem = ViewData["CurrentSystem"] as CosmoSystem;
    %>
    <script type="text/javascript">
    <!--
        function selectSystem(selectedSystemId) {
            // Store the selected system in the input form field
            $('#targetSystem').val(selectedSystemId);

            // Only display the selected system information
            selectedSystemId = 'system' + selectedSystemId;
            $("div.system-info:not(#" + selectedSystemId + ")").css("display", "none");
            $("#" + selectedSystemId).css("display", "inline");
        }
        
        // On-load function to select the system the player currently is in
        $(document).ready(function() {
            selectSystem(<%=currentSystem.SystemId %>);
        });
    -->
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Travel</h1>
    <hr />
    <% using (Html.BeginForm()) { %>
    <div>
        <%=Html.Hidden("targetSystem")%>
    </div>
    <%=Html.ValidationSummary() %>
    <table class="travel">
        <tr>
            <td>
                <%
                    CosmoSystem[] inRangeSystems = ViewData["InRangeSystems"] as CosmoSystem[];
                    int galaxySize = (int)ViewData["GalaxySize"];
                    int displaySize = 400;
                    int currentPositionX = 0;
                    int currentPositionY = 0;
                    double pixelPerPoint = 1.0 * displaySize / galaxySize;
                %>
                <div id="map" class="galaxy-map">
                <% 
                    foreach (CosmoSystem system in (ViewData["Systems"] as CosmoSystem[]))
                    {
                        int x = (int)(system.PositionX * pixelPerPoint);
                        int y = (int)(system.PositionY * pixelPerPoint);
                        string systemClass = "system-outofrange";
                        if (system == ViewData["CurrentSystem"])  
                        {
                            // Store the x/y of the player's current position
                            currentPositionX = x;
                            currentPositionY = y;
                            systemClass = "system-current";
                        }
                        else if (inRangeSystems.Contains(system))
                        {
                            systemClass = "system-inrange";
                        }
                %>
                    <a href="javascript:selectSystem(<%=system.SystemId %>)">
                    <img style="position: absolute; left: <%= x %>px; top: <%= y %>px; " class="system-border <%=systemClass %>"
                            alt="<%=Html.AttributeEncode(system.Name)%>"
                            title="<%=Html.AttributeEncode(system.Name)%>"
                            src="/Content/System/Small/<%=Html.AttributeEncode(system.Name)%>.png"
                            width="20" height="20" />
                     </a>
                <% } %>
                <%
                    // Calcuate the size and position of the in-range circle
                    int shipRange = (int)ViewData["Range"];
                    int shipRangeSize = (int)(pixelPerPoint * shipRange) * 2;
                    int shipRangeX = currentPositionX + 10 - (shipRangeSize / 2);
                    int shipRangeY = currentPositionY + 10 - (shipRangeSize / 2);
                %>
                <img id="shipRange" class="shipRange" src="/Content/ShipRangeCircle.png" alt="Ship Range" 
                    style="left: <%=shipRangeX %>px; top: <%=shipRangeY%>px;" 
                    width="<%=shipRangeSize %>" 
                    height="<%=shipRangeSize %>" />
                </div>
            </td>
            <td valign="top">
                Selected system details:
                <% 
                   foreach (CosmoSystem system in (CosmoSystem[])ViewData["Systems"])
                   {
                %>
                    <div id="system<%=system.SystemId %>" class="system-info">
                    <b><%=Html.Encode(system.Name) %></b>
                    <hr />
                    <img alt="<%=Html.AttributeEncode(system.Name)%>" src="/Content/System/<%=Html.AttributeEncode(system.Name)%>.png" />
                    <% 
                        if (system.Races.Count > 0)
                        { 
                    %>
                    <p>Home System of the <%=Html.Encode(system.Races.First().Name)%> Race</p>
                    <% 
                        } 
                        else 
                        { 
                    %>
                    <p>Dominant Race: <%=Html.Encode(system.Race.Name)%></p>
                    <% 
                        } 
                    %>
                    Bank: 
                    <%
                        if (system.HasBank)
                        {
                            if (system == ViewData["CurrentSystem"])
                            {
                                %><%=Html.ActionLink("Yes", "Bank", "Bank")%><%
                            }
                            else
                            {
                                %>Yes<%
                            }
                        }
                        else
                        {
                            %>No<%
                        }
                    %>
                    <br />
                    <% 
                        if (system.GetGoods().Length > 0)
                        {
                            if (system == ViewData["CurrentSystem"])
                            {
                                %><%=Html.ActionLink("Traded Goods", "ListGoods", "Trade") %><%
                            }
                            else
                            {
                    %>
                    Traded Goods
                    <%
                            }
                    %>
                    <ul>
                    <%
                            foreach (CosmoMonger.Models.SystemGood good in system.GetGoods())
                            {
                    %>
                        <li><%=Html.Encode(good.Good.Name)%> @ <%=good.Price%></li>
                    <%     
                            } 
                    %>
                    </ul>
                    <%  } 
                        if (system == ViewData["CurrentSystem"])  
                        {
                        %>
                            <p class="system-current">You are currently in this system.</p>                            
                        <% 
                        }
                        else if (inRangeSystems.Contains(system))
                        {
                        %>
                            <p class="system-inrange">You can travel to this system.</p>
                            <input type="submit" value="Travel" />
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
