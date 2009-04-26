<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Attack Ship</title>
    <script type="text/javascript">
    //<![CDATA[
        function updateShipList(data) {
            var shipList = $("#shipList");

            // Clear table
            shipList.empty();

            if (data.length > 0) {
                // Build table
                var listTable = '<table class="grid">';

                // Add header row
                listTable += "<tr><th>Type &amp; Name</th><th>Ship Type</th><th>Last Activity</th><th>Attack</th></tr>";

                // Build attack rows
                for (var i = 0; i < data.length; i++) {
                    var ship = data[i];
                    var npcImage = '';
                    if (ship.npcType) {
                        npcImage = '<img class="shipImage" src="/Content/Npc/' + ship.npcType + '.png" alt="' + ship.npcType + '" title="' + ship.npcType + '" width="15" /> ';
                    }
                    
                    listTable += "<tr><td>" + npcImage + ship.shipName + "</td><td>"
                            + ship.shipType + "</td><td>"
                            + ship.lastActivity + "</td><td>";
                    if (ship.inCombat) {
                        listTable += 'In Combat';
                        
                    }  else if (ship.attackable) {
                        listTable += '<form action="/Combat/Attack" method="post"><div>'
                             + '<input type="hidden" name="shipId" value="' + ship.shipId + '" />'
                             + '<input type="submit" value="Attack" />'
                             + '</div></form>';
                    } else {
                        listTable += "Docked"
                    }

                    listTable += "</td></tr>";
                }

                // Complete table and add to the page
                listTable += '</table>';
                shipList.html(listTable);
            } else {
                shipList.text("No ships are currently in the system");
            }

            setTimeout(queueShipList, 2000);
        }

        function queueShipList() {
            $.getJSON('/Combat/GetShipList', updateShipList);
        }

        $(document).ready(function() {
            queueShipList();
        });
    //]]>
    </script>
    
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Attack Ship</h1>
<p>
<%=Html.ValidationSummary("Unable to Attack") %>
</p>
<p id="shipList">
Loading...
</p>
<p>Refreshed every 2 seconds</p>
<p class="legend">Legend</p>
<p>No Icon = Player
<img class="shipImage" src="/Content/Npc/Pirate.png" alt="Pirate" title="Pirate" />= Pirate 
<img class="shipImage" src="/Content/Npc/Trader.png" alt="Trader" title="Trader" />= Trader 
<img class="shipImage" src="/Content/Npc/Police.png" alt="Police" title="Police" />= Police
</p>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
