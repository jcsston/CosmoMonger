<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Combat</title>
    <script type="text/javascript" src="/Scripts/jquery.tooltip.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.bgiframe.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.dimensions.js"></script>
    <script type="text/javascript">
    //<![CDATA[
        var combatId = null;
        var playerNotified = false;

        function updateShipStats(data) {
            // Update player/enemy hull/shield damage
            var playerHull = $('#playerHull');
            if (playerHull.css('height') != data.playerHull + '%') {
                if (data.playerHull > 8) {
                    playerHull.text(100 - data.playerHull + '%');
                } else {
                    playerHull.text('');
                }
                playerHull.animate({ height: data.playerHull + '%' }, "normal");
                if (data.playerHull == 100) {
                    // Ship explodes...
                    $('#playerShip').hide("explode", { number: 9 }, 1000);
                }
            }

            var playerShield = $('#playerShield');
            if (playerShield.css('height') != data.playerShield + '%') {
                if (data.playerShield > 8) {
                    playerShield.text(100 - data.playerShield + '%');
                } else {
                    playerShield.text('');
                }
                playerShield.animate({ height: data.playerShield + '%' }, "normal");
            }

            var enemyHull = $('#enemyHull');
            if (enemyHull.css('height') != data.enemyHull + '%') {
                if (data.enemyHull > 8) {
                    enemyHull.text(100 - data.enemyHull + '%');
                } else {
                    enemyHull.text('');
                }
                enemyHull.animate({ height: data.enemyHull + '%' }, "normal");
                if (data.enemyHull == 100) {
                    // Ship explodes...
                    $('#enemyShip').hide("explode", { number: 9 }, 1000);
                }
            }

            var enemyShield = $('#enemyShield');
            if (enemyShield.css('height') != data.enemyShield + '%') {
                if (data.enemyShield > 8) {
                    enemyShield.text(100 - data.enemyShield + '%');
                } else {
                    enemyShield.text('');
                }
                enemyShield.animate({ height: data.enemyShield + '%' }, "normal");
            }

            $("#jumpDriveChargeBar").progressbar('value', data.jumpDriveCharge);
            $("#jumpDriveCharge").text(data.jumpDriveCharge);
        }

        function showPlayerDialog(title, content, buttons) {
            $("<div />")
            .html(content)
            .addClass("dialog")
            .dialog({
                modal: true,
                title: title,
                buttons: buttons
            });
        }

        function acceptSurrender() {
            $(".turnAction").attr("disabled", "disabled");
            $.getJSON('/Combat/AcceptSurrender', { combatId: combatId }, function(data) {
                updateCombatStatus(data.status, true);
                if (data.message) {
                    alert(data.message);
                }
            });
        }
        
        function surrenderOffered(data) {
            playerNotified = true;
            showPlayerDialog("Accept Surrender?", "Other player has offered surrender, accept?", {
                "Accept": acceptSurrender,
                "Ignore": function() {
                    $(this).dialog("close");
                }
            });
        }

        function pickupCargo() {
            $(".turnAction").attr("disabled", "disabled");
            $.getJSON('/Combat/PickupCargo', { combatId: combatId }, function(data) {
                updateCombatStatus(data.status, true);
                if (data.message) {
                    alert(data.message);
                }
            });
        }
        
        function cargoToPickup(data) {
            playerNotified = true;
            var content = 'Other player has jettisoned ' + data.cargoJettisoned 
                       + ' cargo items, pickup? <br />'
                       + ' If we pickup the cargo the other player will escape, if not the cargo will be lost.';

            showPlayerDialog("Pickup Cargo?", content, {
                "Pickup": pickupCargo,
                "Ignore": function() {
                    $(this).dialog("close");
                }
            });
        }

        function consentToSearch() {
            $(".turnAction").attr("disabled", "disabled");
            $.getJSON('/Combat/AcceptSearch', { combatId: combatId }, function(data) {
                updateCombatStatus(data.status, true);
                if (data.message) {
                    showPlayerDialog("Problem with search", data.message, { "Flee": chargeJumpDrive });
                }
            });
        }
        
        function beingSearched(data) {
            playerNotified = true;
            var content = 'You have been intercepted by the police. <br />'
                        + 'Do you consent to a search of your cargo for contraband items? <br />'
                        + 'If you have any aboard you will be fined.';

            showPlayerDialog("Consent to Search?", content, {
                "Consent": consentToSearch,
                "Flee": chargeJumpDrive
            });
        }
        
        function processPlayerTurn(data) {
            // Enable turn buttons
            $(".turnAction").attr("disabled", "");
            $(".turnActions").show("slow");
            $("#currentTurn").text("Yours");
            
            // Update stats
            $("#timeLeft").text(parseInt(data.timeLeft));
            $("#turnPoints").text(data.turnPoints);
            $("#playerWeaponHits").text(data.playerHits);
            $("#playerWeaponMisses").text(data.playerMisses);
            $("#enemyWeaponHits").text(data.enemyHits);
            $("#enemyWeaponMisses").text(data.enemyMisses);
            
            // Check if we need to prompt the player
            if (data.surrendered && !playerNotified) {
                // Other player has offered surrender
                surrenderOffered(data);

            } else if (data.cargoJettisoned && !playerNotified) {
                // Other player has jettisoned cargo
                cargoToPickup(data);
                
            } else if (data.beingSearched && !playerNotified) {
                // Player is being searched
                beingSearched(data);
            }
        }

        function processEnemyTurn(data) {
            // Hide turn actions
            $(".turnActions").slideUp("slow");
            $(".turnAction").attr("disabled", "disabled");
            $(".dialog").dialog("close");
            $("#currentTurn").text("Enemy");

            // Update stats
            $("#timeLeft").text(parseInt(data.timeLeft));
            $("#turnPoints").text(data.turnPoints);
            $("#playerWeaponHits").text(data.playerHits);
            $("#playerWeaponMisses").text(data.playerMisses);
            $("#enemyWeaponHits").text(data.enemyHits);
            $("#enemyWeaponMisses").text(data.enemyMisses);

            // Reset player notifications
            playerNotified = false;
        }
        
        function updateCombatStatus(data, noTimeout) {
            // Update data format is defined in CombatController.BuildCombatStatus
            updateShipStats(data);    
            
            if (data.turn) {
                processPlayerTurn(data);
            } else {
                processEnemyTurn(data);
            }
            
            // Done updating status, Has combat been completed?
            if (data.complete) {
                document.location = '/Combat/CombatComplete?combatId=' + combatId;
                return;
            }
            
            // Queue combat status check if needed
            if (noTimeout != true) {    
                setTimeout(queueCombatStatus, 1000);
            }
        }
        
        function queueCombatStatus() {
            $.getJSON('/Combat/CombatStatus', {combatId: combatId}, updateCombatStatus);
        }

        function fireWeapon() {
            $(".turnAction").attr("disabled", "disabled");
            $.getJSON('/Combat/FireWeapon', { combatId: combatId }, function(data) {
                updateCombatStatus(data.status, true);
                if (data.message) {
                    alert(data.message);
                }
            });
        }

        function chargeJumpDrive() {
            $(".turnAction").attr("disabled", "disabled");
            $.getJSON('/Combat/ChargeJumpDrive', { combatId: combatId }, function(data) {
                updateCombatStatus(data.status, true);
                if (data.message) {
                    alert(data.message);
                }
            });
        }

        function jettisonCargo() {
            $(".turnAction").attr("disabled", "disabled");
            $.getJSON('/Combat/JettisonCargo', { combatId: combatId }, function(data) {
                updateCombatStatus(data.status, true);
                if (data.message) {
                    alert(data.message);
                }
            });
        }

        function offerSurrender() {
            $(".turnAction").attr("disabled", "disabled");
            $.getJSON('/Combat/OfferSurrender', { combatId: combatId }, function(data) {
                updateCombatStatus(data.status, true);
                if (data.message) {
                    alert(data.message);
                }
            });
        }

        $(document).ready(function() {
            // Link action buttons
            $('#fireWeapon').click(fireWeapon);
            $('#chargeJumpDrive').click(chargeJumpDrive);
            $('#jettisonCargo').click(jettisonCargo);
            $('#offerSurrender').click(offerSurrender);

            // Enable tooltips
            $('.tooltipDetails').tooltip({
                showURL: false,
                extraClass: "ui-widget-content",
                bodyHandler: function() {
                    var content = $(this).children("div").html();
                    return '<center>' + $(this).html() + '</center><hr />' + content;
                }
            });

            // Setup JumpDrive charge status bar
            $("#jumpDriveChargeBar").progressbar({ range: true, width: 100, value: 0 });

            // Start combat updates!
            queueCombatStatus();
        });
    //]]>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%
    Combat activeCombat = (Combat)ViewData["Combat"];
    Ship playerShip = (Ship)ViewData["PlayerShip"];
    Ship enemyShip = (Ship)ViewData["EnemyShip"];
%>
    <script type="text/javascript">
    //<![CDATA[
        combatId = <%=activeCombat.CombatId %>;
    //]]>
    </script>
    <h1>Combat</h1>
    <table class="combat">
        <tr>
            <td rowspan="2">
                Hull
                <div class="combatBar combatHull">
                    <div id="playerHull"></div>
                </div>
            </td>
            <td rowspan="2">
                Shield
                <div class="combatBar combatShield">
                    <div id="playerShield"></div>
                </div>
            </td>
            <td>
                <%=Html.Encode(ViewData["PlayerName"]) %>
                <br />
                <img id="playerShip" alt="Your Ship" 
                    title="<%=Html.AttributeEncode(playerShip.BaseShip.Name) %>" 
                    src="/Content/BaseShip/<%=Html.AttributeEncode(playerShip.BaseShip.Name) %>.png" />
                <br />
                Your 
                <span class="tooltipDetails">
                    <%=Html.Encode(ViewData["PlayerRaceName"]) %>
                    <div style="display: none;">
                        <table class="center">
                            <tr>
                                <td>
                                    <center>
                                        <div class="ui-icon ui-icon-arrowthick-1-n"></div>
                                    </center>
                                </td>
                                <td></td>
                                <td>
                                    <center>
                                        <div class="ui-icon ui-icon-minusthick"></div>
                                    </center>
                                </td>
                                <td></td>
                                <td>
                                    <center>
                                        <div class="ui-icon ui-icon-arrowthick-1-s"></div>
                                    </center>
                                </td>
                            </tr>
                            <%
                                Dictionary<string, List<string>> playerRacialModifiers = (Dictionary<string, List<string>>)ViewData["PlayerRaceModifiers"];
                                List<string> plusModifiers = playerRacialModifiers["Plus"];
                                List<string> neturalModifiers = playerRacialModifiers["Netural"];
                                List<string> minusModifiers = playerRacialModifiers["Minus"];

                                while (plusModifiers.Count > 0 || neturalModifiers.Count > 0 || minusModifiers.Count > 0)
                                {
                                    string plus = plusModifiers.FirstOrDefault() ?? String.Empty;
                                    string netural = neturalModifiers.FirstOrDefault() ?? String.Empty;
                                    string minus = minusModifiers.FirstOrDefault() ?? String.Empty;
                                    if (plusModifiers.Count > 0)
                                    {
                                        plusModifiers.RemoveAt(0);
                                    }
                                    if (neturalModifiers.Count > 0)
                                    {
                                        neturalModifiers.RemoveAt(0);
                                    }
                                    if (minusModifiers.Count > 0)
                                    {
                                        minusModifiers.RemoveAt(0);
                                    }
                                    %><tr>
                                        <td><%=Html.Encode(plus) %></td>
                                        <td></td>
                                        <td><%=Html.Encode(netural) %></td>
                                        <td></td>
                                        <td><%=Html.Encode(minus) %></td>
                                      </tr><%
                                }
                            %>
                        </table>
                    </div>
                </span>
                Ship
            </td>
            <td>&nbsp;</td>
            <td>
                <% if (ViewData["NpcType"] != null) { %>
                <img class="shipImage" src="/Content/Npc/<%=Html.AttributeEncode(ViewData["NpcType"]) %>.png" alt="<%=Html.AttributeEncode(ViewData["NpcType"]) %>" title="<%=Html.AttributeEncode(ViewData["NpcType"]) %>" />
                <% } %>
                <%=Html.Encode(ViewData["EnemyName"]) %>
                <br />
                <img id="enemyShip" alt="Enemy Ship" 
                    title="<%=Html.AttributeEncode(enemyShip.BaseShip.Name) %>" 
                    src="/Content/BaseShip/Flipped/<%=Html.AttributeEncode(enemyShip.BaseShip.Name) %>.png" />
                <br />
                Enemy 
                <span class="tooltipDetails">
                    <%=Html.Encode(ViewData["EnemyRaceName"]) %>
                    <div style="display: none;">
                        <table class="center">
                            <tr>
                                <td>
                                    <center>
                                        <div class="ui-icon ui-icon-arrowthick-1-n"></div>
                                    </center>
                                </td>
                                <td></td>
                                <td>
                                    <center>
                                        <div class="ui-icon ui-icon-minusthick"></div>
                                    </center>
                                </td>
                                <td></td>
                                <td>
                                    <center>
                                        <div class="ui-icon ui-icon-arrowthick-1-s"></div>
                                    </center>
                                </td>
                            </tr>
                            <%
                                Dictionary<string, List<string>> enemyRacialModifiers = (Dictionary<string, List<string>>)ViewData["EnemyRaceModifiers"];
                                plusModifiers = enemyRacialModifiers["Plus"];
                                neturalModifiers = enemyRacialModifiers["Netural"];
                                minusModifiers = enemyRacialModifiers["Minus"];

                                while (plusModifiers.Count > 0 || neturalModifiers.Count > 0 || minusModifiers.Count > 0)
                                {
                                    string plus = plusModifiers.FirstOrDefault() ?? String.Empty;
                                    string netural = neturalModifiers.FirstOrDefault() ?? String.Empty;
                                    string minus = minusModifiers.FirstOrDefault() ?? String.Empty;
                                    if (plusModifiers.Count > 0)
                                    {
                                        plusModifiers.RemoveAt(0);
                                    }
                                    if (neturalModifiers.Count > 0)
                                    {
                                        neturalModifiers.RemoveAt(0);
                                    }
                                    if (minusModifiers.Count > 0)
                                    {
                                        minusModifiers.RemoveAt(0);
                                    }
                                    %><tr>
                                        <td><%=Html.Encode(plus) %></td>
                                        <td></td>
                                        <td><%=Html.Encode(netural) %></td>
                                        <td></td>
                                        <td><%=Html.Encode(minus) %></td>
                                      </tr><%
                                }
                            %>
                        </table>
                    </div>
                </span>
                Ship
            </td>
            <td rowspan="2">
                Shield
                <div class="combatBar combatShield">
                    <div id="enemyShield"></div>
                </div>
            </td>
            <td rowspan="2">
                Hull
                <div class="combatBar combatHull">
                    <div id="enemyHull"></div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                Weapon: 
                <span class="tooltipDetails">
                    <%=Html.Encode(playerShip.Weapon.Name) %>
                    <div style="display: none;">
                        Turn Cost: <%=playerShip.Weapon.TurnCost %>
                        <br />
                        Power: <%=playerShip.Weapon.Power %>
                        <br />
                        Accuracy: <%=(int)(Weapon.BaseAccuracy*100.0) %>%
                    </div>
                </span>
                <br />
                Shield: 
                <span class="tooltipDetails">
                    <%=Html.Encode(playerShip.Shield.Name) %>
                    <div style="display: none;">
                        Strength: <%=playerShip.Shield.Strength %>
                    </div>
                </span>
            </td>
            <td>&nbsp;</td>
            <td>
                Weapon: 
                <span class="tooltipDetails">
                    <%=Html.Encode(enemyShip.Weapon.Name) %>
                    <div style="display: none;">
                        Turn Cost: <%=enemyShip.Weapon.TurnCost %>
                        <br />
                        Power: <%=enemyShip.Weapon.Power%>
                        <br />
                        Accuracy: <%=(int)(Weapon.BaseAccuracy*100.0) %>%
                    </div>
                </span>
                <br />
                Shield: 
                <span class="tooltipDetails">
                    <%=Html.Encode(enemyShip.Shield.Name) %>
                    <div style="display: none;">
                        Strength: <%=enemyShip.Shield.Strength %>
                    </div>
                </span>
            </td>
        </tr>
    </table>
    <hr />
    <table class="combat">
    <tr>
        <td colspan="5">Current turn: <b><span id="currentTurn">Yours</span></b></td>
    </tr>
    <tr>
        <td align="right">
            <div class="turnActions">
                <button id="fireWeapon" class="turnAction tooltipDetails" type="button">
                    Fire Weapon
                   <div style="display: none">
                        Fires your primary weapon at the enemy ship.
                    </div>
                </button>
                <br />
                <button id="jettisonCargo" class="turnAction tooltipDetails" type="button">
                    Jettison Cargo
                    <div style="display: none">
                        <p>
                            Escape if enemy picks up cargo.
                        </p>
                        <p>
                            Current Cargo: <%=Html.Encode(ViewData["CargoCount"]) %> items
                        </p>
                    </div>
                </button>
            </div>
        </td>
        <td>    
        </td>
        <td align="left">
            <div class="turnActions">
                <button id="offerSurrender" class="turnAction tooltipDetails" type="button">
                        Offer Surrender
                        <div style="display: none">
                            <p>
                                If enemy accepts you lose your cargo and credits on-board, but keep your ship intact.
                            </p>
                            <p>
                                Cargo: <%=Html.Encode(ViewData["CargoCount"]) %> items
                                <br />
                                Credits: $<%=Html.Encode(ViewData["Credits"]) %>
                            </p>
                        </div>
                </button>
                <br />
                <button id="chargeJumpDrive" class="turnAction tooltipDetails" type="button">
                    Charge JumpDrive
                    <div style="display: none">
                        Use the rest of your turn points to charge your JumpDrive.
                    </div>
                </button>
            </div>
        </td>
        <td rowspan="2"></td>
        <td rowspan="2">
            <p>
                <b>Player Weapon</b>
                <br />
                <span id="playerWeaponHits">0</span> Hits
                <br />
                <span id="playerWeaponMisses">0</span> Misses
            </p>
            <p>
                <b>Enemy Weapon</b>
                <br />
                <span id="enemyWeaponHits">0</span> Hits
                <br />
                <span id="enemyWeaponMisses">0</span> Misses
            </p>
        </td>
    </tr>
    <tr>
        <td colspan="3">
            JumpDrive Charge: <span id="jumpDriveCharge">0</span>%
            <div id="jumpDriveChargeBar"></div>
        </td>
    </tr>
    <tr>
        <td colspan="2"> 
            Turn Points Left: <span id="turnPoints">0</span>
        </td>
        <td colspan="3">
            Turn Time Left: <span id="timeLeft">0</span> seconds
        </td>
    </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
