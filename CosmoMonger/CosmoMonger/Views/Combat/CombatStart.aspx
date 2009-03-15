<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Combat</title>
    <script type="text/javascript">
    <!--
        var combatId = null;
        var playerNotified = false;
        
        function updateCombatStatus(data, noTimeout) {
            // Update data format is defined in CombatController.BuildCombatStatus
            
            // Update player/enemy hull/shield damage
            var playerHull = $('#playerHull');
            if (playerHull.css('height') != data.playerHull + '%') {
                if (data.playerHull > 8) {
                    playerHull.text(data.playerHull + '%');
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
                    playerShield.text(data.playerShield + '%');
                } else {
                    playerShield.text('');
                }
                playerShield.animate({ height: data.playerShield + '%' }, "normal");
            }
            
            var enemyHull = $('#enemyHull');
            if (enemyHull.css('height') != data.enemyHull + '%') {
                if (data.enemyHull > 8) {
                    enemyHull.text(data.enemyHull + '%');
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
                    enemyShield.text(data.enemyShield + '%');
                } else {
                    enemyShield.text('');
                }
                enemyShield.animate({ height: data.enemyShield + '%' }, "normal");
            }

            $("#jumpDriveCharge").text(data.jumpDriveCharge);
            if (data.turn) {
                // Enable turn buttons
                $(".turnAction").attr("disabled", "");
                $("#turnActions caption").html("Current Turn: <b>Yours</b>");
                $("#timeLeft").text(parseInt(data.timeLeft));
                //$("#turnActions").show("slow");
                $("#turnPoints").text(data.turnPoints);

                // Check if we need to prompt the player
                if (data.surrendered && !playerNotified) {
                    // Other player has offered surrender
                    playerNotified = true;
                    var acceptSurrender = confirm('Other player has offered surrender, accept?');
                    if (acceptSurrender) {
                        $(".turnAction").attr("disabled", "disabled");
                        $.getJSON('/Combat/AcceptSurrender', { combatId: combatId }, function(data) {
                            updateCombatStatus(data.status, true);
                            if (data.message) {
                                alert(data.message);
                            }
                        });
                    }
                } else if (data.cargoJettisoned && !playerNotified) {
                    // Other player has jettisoned cargo
                    playerNotified = true;
                    var pickupCargo = confirm('Other player has jettisoned ' + data.cargoJettisoned + ' cargo items, pickup? If we pickup the cargo the other player will escape, if not the cargo will be lost.');
                    if (pickupCargo) {
                        $(".turnAction").attr("disabled", "disabled");
                        $.getJSON('/Combat/PickupCargo', { combatId: combatId }, function(data) {
                            updateCombatStatus(data.status, true);
                            if (data.message) {
                                alert(data.message);
                            }
                        });
                    }
                }
            } else {
                // Hide turn actions
                //$("#turnActions").slideUp("slow");
                $(".turnAction").attr("disabled", "disabled");
                $("#turnPoints").text(0);
                $("#timeLeft").text(0);
                $("#turnActions caption").html("Current Turn: <b>Enemy</b>");

                // Reset player notifications
                playerNotified = false;
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

        $(document).ready(function() {
            $('#fireWeapon').click(function(eventObject) {
                $(".turnAction").attr("disabled", "disabled");
                $.getJSON('/Combat/FireWeapon', { combatId: combatId }, function(data) {
                    updateCombatStatus(data.status, true);
                    if (data.message) {
                        alert(data.message);
                    }
                    //$(".turnAction").attr("disabled", "");
                });
            });

            $('#chargeJumpDrive').click(function(eventObject) {
                $(".turnAction").attr("disabled", "disabled");
                $.getJSON('/Combat/ChargeJumpDrive', { combatId: combatId }, function(data) {
                    updateCombatStatus(data.status, true);
                    if (data.message) {
                        alert(data.message);
                    }
                    //$(".turnAction").attr("disabled", "");
                });
            });

            $('#jettisonCargo').click(function(eventObject) {
                $(".turnAction").attr("disabled", "disabled");
                $.getJSON('/Combat/JettisonCargo', { combatId: combatId }, function(data) {
                    updateCombatStatus(data.status, true);
                    if (data.message) {
                        alert(data.message);
                    }
                    //$(".turnAction").attr("disabled", "");
                });
            });

            $('#offerSurrender').click(function(eventObject) {
                $(".turnAction").attr("disabled", "disabled");
                $.getJSON('/Combat/OfferSurrender', { combatId: combatId }, function(data) {
                    updateCombatStatus(data.status, true);
                    if (data.message) {
                        alert(data.message);
                    }
                    //$(".turnAction").attr("disabled", "");
                });
            });

            queueCombatStatus();
        });
    //-->
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%
    Combat activeCombat = (Combat)ViewData["Combat"];
    Ship playerShip = (Ship)ViewData["PlayerShip"];
    Ship enemyShip = (Ship)ViewData["EnemyShip"];
%>
    <script type="text/javascript">
    <!--
        combatId = <%=activeCombat.CombatId %>;
    //-->
    </script>
    <h1>Combat</h1>
    <table class="combat">
        <tr>
            <td rowspan="2">
                Hull
                <div style="border: solid thin blue; background-color: blue; width: 75px; height: 175px;">
                    <div id="playerHull" style="width: 100%; height: 0%; background-color: black"></div>
                </div>
            </td>
            <td rowspan="2">
                Shield
                <div style="border: solid thin blue; background-color: green; width: 75px; height: 175px;">
                    <div id="playerShield" style="width: 100%; height: 0%; background-color: black"></div>
                </div>
            </td>
            <td>
                <%=Html.Encode(ViewData["PlayerName"]) %>
                <br />
                <img id="playerShip" alt="Your Ship" src="/Content/BaseShip/<%=Html.AttributeEncode(playerShip.BaseShip.Name) %>.png" />
                <br />
                Your Ship
            </td>
            <td>&nbsp;</td>
            <td>
                <%=Html.Encode(ViewData["EnemyName"]) %>
                <br />
                <img id="enemyShip" alt="Enemy Ship" src="/Content/BaseShip/<%=Html.AttributeEncode(enemyShip.BaseShip.Name) %>.png" />
                <br />
                Enemy Ship
            </td>
            <td rowspan="2">
                Shield
                <div style="border: solid thin blue; background-color: green; width: 75px; height: 175px;">
                    <div id="enemyShield" style="width: 100%; height: 0%; background-color: black"></div>
                </div>
            </td>
            <td rowspan="2">
                Hull
                <div style="border: solid thin blue; background-color: blue; width: 75px; height: 175px;">
                    <div id="enemyHull" style="width: 100%; height: 0%; background-color: black"></div>
                </div>
            </td>
        </tr>
        <tr>
            <td>Primary Weapon: <%=Html.Encode(playerShip.Weapon.Name) %></td>
            <td>&nbsp;</td>
            <td>Primary Weapon: <%=Html.Encode(enemyShip.Weapon.Name) %></td>
        </tr>
    </table>
    <hr />
    <table id="turnActions" class="combat">
    <caption>Current turn: Yours</caption>
    <tr>
        <td colspan="2">
            <p>Attack</p>
        </td>
        <td colspan="2">
            <p>Flee</p>
        </td>
    </tr>
    <tr>
        <td>
            Cost: <%=playerShip.Weapon.TurnCost %>
            <br />
            Power: <%=playerShip.Weapon.Power %>
        </td>
        <td>
            <button id="fireWeapon" class="turnAction" type="button">Fire Weapon</button>
        </td>
        <td>
            Cost: Rest of Turn
            <br />
            Charge: <span id="jumpDriveCharge">0</span>%
            </td>
        <td>
            <button id="chargeJumpDrive" class="turnAction" type="button">Charge JumpDrive</button>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <p>Jettison Cargo</p>
        </td>
        <td colspan="2">
            <p>Surrender</p></td>
    </tr>
    <tr>
        <td>
            Cost: Rest of Turn
            <br />
            Escape if enemy picks up cargo
        </td>
        <td>
            <button id="jettisonCargo" class="turnAction" type="button">Jettison Cargo</button>
        </td>
        <td>
            Cost: Rest of Turn
            <br />
            Lose Ship Cargo and Credits
        </td>
        <td>
            <button id="offerSurrender" class="turnAction" type="button">Offer Surrender</button>
        </td>
    </tr>
    <tr>
    <td colspan="2">
        Turn Points Left: <span id="turnPoints">0</span>
    </td>
    <td colspan="2">
        Turn Time Left: <span id="timeLeft">0</span> seconds
    </td>
    </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
