<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Ship Upgrades</title>
    <script type="text/javascript">
    <!--
        $(document).ready(function() {
            //$("#tabs").tabs();
        });
    -->
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>Ship Upgrades</h1>
<hr />
<%
    Ship currentShip = (Ship)ViewData["CurrentShip"];    
%>
<div id="tabs">
<!--
	<ul>
		<li><a href="#tabs-1">Jump Drives</a></li>
		<li><a href="#tabs-2">Shields</a></li>
		<li><a href="#tabs-3">Weapons</a></li>
	</ul>
-->
	<div id="tabs-1">
	<h2>Jump Drives</h2>
<%
    JumpDrive currentJumpDrive = (JumpDrive)ViewData["CurrentJumpDrive"];
    Html.Grid<SystemJumpDriveUpgrade>(
        "JumpDriveUpgrades",
        new Hash(empty => "No jumpdrive upgrades avaiable", @class => "goods"),
        column =>
        {
            column.For(u => u.JumpDrive.Name, "Type");
            column.For(u => u.JumpDrive.Range);
            column.For(u => u.JumpDrive.ChargeTime);
            column.For(u => u.JumpDrive.CargoCost);
            column.For(u => u.GetPrice(currentShip).ToString("C0"), "Price");
            column.For("Buy").Do(u =>
            {%><td><%
            if (u.JumpDrive == currentJumpDrive)
            {
                %>Current<% 
            }
            else
            {
                using (Html.BeginForm("BuyJumpDriveUpgrade", "Ship"))
                { 
                    string disabled = "";
                    if (u.GetPrice(currentShip) > (currentShip.Credits + currentJumpDrive.GetTradeInValue(currentShip))) 
                    {
                        // Not enough to buy
                        disabled = "disabled=\"disabled\"";
                    }
                    %><div>
                        <input type="hidden" name="jumpDriveId" value="<%=u.JumpDriveId %>" /> 
                        <input type="submit" value="Buy" <%=disabled %>/>
                    </div><% 
                }
            }
            %></td><%
        });

        }
    );
%>
    <p class="center">Current Jump Drive Trade-In Value: <%=currentJumpDrive.GetTradeInValue(currentShip).ToString("C0") %></p>
    </div>
    <div id="tabs-2">
	<h2>Shields</h2>
<%
    Shield currentShield = (Shield)ViewData["CurrentShield"];
    Html.Grid<SystemShieldUpgrade>(
        "ShieldUpgrades",
        new Hash(empty => "No shield upgrades avaiable", @class => "goods"),
        column =>
        {
            column.For(u => u.Shield.Name, "Type");
            column.For(u => u.Shield.Strength);
            column.For(u => u.Shield.CargoCost);
            column.For(u => u.GetPrice(currentShip).ToString("C0"), "Price");
            column.For("Buy").Do(u =>
            {%><td><%
            if (u.Shield == currentShield)
            {
                %>Current<% 
            }
            else
            {
                using (Html.BeginForm("BuyShieldUpgrade", "Ship"))
                { 
                    string disabled = "";
                    if (u.GetPrice(currentShip) > (currentShip.Credits + currentShield.GetTradeInValue(currentShip))) 
                    {
                        // Not enough to buy
                        disabled = "disabled=\"disabled\"";
                    }
                    %><div>
                        <input type="hidden" name="shieldId" value="<%=u.ShieldId %>" /> 
                        <input type="submit" value="Buy" <%=disabled %>/>
                    </div><% 
                }
            }
            %></td><%
        });

        }
    );
%>
    <p class="center">Current Shield Trade-In Value: <%=currentShield.GetTradeInValue(currentShip).ToString("C0") %></p>
    </div>
    <div id="tabs-3">
    <h2>Weapons</h2>
<%
    Weapon currentWeapon = (Weapon)ViewData["CurrentWeapon"];
    Html.Grid<SystemWeaponUpgrade>(
        "WeaponUpgrades",
        new Hash(empty => "No weapon upgrades avaiable", @class => "goods"),
        column =>
        {
            column.For(u => u.Weapon.Name, "Type");
            column.For(u => u.Weapon.Power);
            column.For(u => u.Weapon.TurnCost);
            column.For(u => u.Weapon.CargoCost);
            column.For(u => u.GetPrice(currentShip).ToString("C0"), "Price");
            column.For("Buy").Do(u =>
            {%><td><%
            if (u.Weapon == currentWeapon)
            {
                %>Current<% 
            }
            else
            {
                using (Html.BeginForm("BuyWeaponUpgrade", "Ship"))
                { 
                    string disabled = "";
                    if (u.GetPrice(currentShip) > (currentShip.Credits + currentWeapon.GetTradeInValue(currentShip))) 
                    {
                        // Not enough to buy
                        disabled = "disabled=\"disabled\"";
                    }
                    %><div>
                        <input type="hidden" name="weaponId" value="<%=u.WeaponId %>" /> 
                        <input type="submit" value="Buy" <%=disabled %>/>
                    </div><% 
                }
            }
            %></td><%
        });

        }
    );
%>
    <p class="center">Current Weapon Trade-In Value: <%=currentWeapon.GetTradeInValue(currentShip).ToString("C0") %></p>
    </div>
</div>
<table class="goods goodsCenter">
    <tr><th>Credits</th><th>Bank Credits</th><th>Cargo Space Free</th></tr>
    <tr>
        <td><%= ((int)ViewData["Credits"]).ToString("C0")%></td>
        <td><%= ((int)ViewData["BankCredits"]).ToString("C0")%></td>
        <td><%= ViewData["FreeCargoSpace"] %></td>
    </tr>
</table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
