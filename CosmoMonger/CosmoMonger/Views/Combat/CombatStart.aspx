<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Combat</title>
    <script type="text/javascript">
    <!--
        combatId = null;
        function updateCombatStatus(data) {
            // Update data format is defined in CombatController.BuildCombatStatus
            
            // Update player/enemy hull/shield damage
            var playerHull = $('#playerHull');
            if (playerHull.css('height') != data.playerHull + '%') {
                playerHull.animate({ height: data.playerHull + '%' }, "slow");
            }
            var playerShield = $('#playerShield');
            if (playerShield.css('height') != data.playerShield + '%') {
                playerShield.animate({ height: data.playerShield + '%' }, "slow");
            }
            var enemyHull = $('#enemyHull');
            if (enemyHull.css('height') != data.enemyHull + '%') {
                enemyHull.animate({ height: data.enemyHull + '%' }, "slow");
            }
            var enemyShield = $('#enemyShield');
            if (enemyShield.css('height') != data.enemyShield + '%') {
                enemyShield.animate({ height: data.enemyShield + '%' }, "slow");
            }
            
            if (data.turn) {
                $("#turnActions").show("slow");
                $("#jumpDriveCharge").text(data.jumpDriveCharge);
                $("#turnPoints").text(data.turnPoints);
            } else {
                // Hide turn actions
                $("#turnActions").slideUp("slow");
            }

            if (data.complete) {
                document.location = '/Combat/CombatComplete?combatId=' + combatId;
            } else {
                // Queue combat status check
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
                    updateCombatStatus(data.status);
                    if (data.message) {
                        alert(data.message);
                    }
                    $(".turnAction").attr("disabled", "");
                });
            });

            $('#chargeJumpDrive').click(function(eventObject) {
                $(".turnAction").attr("disabled", "disabled");

                $(".turnAction").attr("disabled", "");
            });

            $('#jettisonCargo').click(function(eventObject) {
                $(".turnAction").attr("disabled", "disabled");

                $(".turnAction").attr("disabled", "");
            });

            $('#offerSurrender').click(function(eventObject) {
                $(".turnAction").attr("disabled", "disabled");

                $(".turnAction").attr("disabled", "");
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
                    <div id="playerHull" style="width: 100%; height: 0%; background-color: black" />
                </div>
                </td>
            <td rowspan="2">
                Shield
                <div style="border: solid thin blue; background-color: green; width: 75px; height: 175px;">
                    <div id="playerShield" style="width: 100%; height: 0%; background-color: black" />
                </div>
                </td>
            <td>
                <%=Html.Encode(ViewData["PlayerName"]) %>
                <br />
                <img alt="Your Ship" src="/Content/BaseShip/<%=playerShip.BaseShipId %>.png" />
                <br />
                Your Ship</td>
            <td>
                &nbsp;</td>
            <td>
                <%=Html.Encode(ViewData["EnemyName"]) %>
                <br />
                <img alt="Enemy Ship" src="/Content/BaseShip/<%=enemyShip.BaseShipId %>.png" />
                <br />
                Enemy Ship</td>
            <td rowspan="2">
                Shield
                <div style="border: solid thin blue; background-color: green; width: 75px; height: 175px;">
                    <div id="enemyShield" style="width: 100%; height: 0%; background-color: black" />
                </div>
                </td>
            <td rowspan="2">
                Hull
                <div style="border: solid thin blue; background-color: blue; width: 75px; height: 175px;">
                    <div id="enemyHull" style="width: 100%; height: 0%; background-color: black" />
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
    <caption>Turn Actions</caption>
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
            Cost: <%=playerShip.Weapon.CargoCost %>
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
    <td colspan="4">
        Turn Points Left: <span id="turnPoints">20</span>
        <!--
        <br />
        Turn Time Left:
        <div style="width: 200px; height: 30px; border: solid thin blue">
        <div id="timeLeft" style="background-color: red; width: 100px; height: 100%;" />
        </div>
        -->
    </td>
    </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
