<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <p>
            <span style="font-size: medium;">
            Welcome! Before you can begin playing, you 
            need to set up a player profile.
            </span>
        </p>
        <p>
            <span style="font-size: medium;">
            You may wish to glance at the rules before selecting your character's race.
            </span>
        </p>
        <%= Html.ValidationSummary() %>
        <% using (Html.BeginForm()) { %>
            <table style="width: 100%">
                <tr>
                    <td style="width: 10%">Name:</td>
                    <td style="width: 20%">
                        <%= Html.TextBox("name") %>
                        <%= Html.ValidationMessage("name")%>
                    </td>
                    <td rowspan="2" style="width: 40%" align="center">
                        <img id="RaceImg" src="" alt="Race Image" width="100px" height="100px" 
                            style="margin-left: 0px"/></td>
                    <td style="width: 15%">Home System:</td>
                    <td style="width: 15%">?</td>
                </tr>
                <script type="text/javascript" language="javascript">
                    function UpdateRaceImage() {
                        var raceSelect = document.getElementById('raceId');
                        var raceOption = raceSelect.options[raceSelect.selectedIndex];
                        document.getElementById('RaceImg').src = '../Content/Races/' + raceOption.text + '.jpg';
                        //setting the display style to "none" for all racial divs
                        document.getElementById("SkummWeapon").style.display = "none";
                        document.getElementById("SkummShield").style.display = "none";
                        document.getElementById("SkummEngine").style.display = "none";
                        
                        document.getElementById("DecapodianWeapon").style.display = "none";
                        document.getElementById("DecapodianShield").style.display = "none";
                        document.getElementById("DecapodianEngine").style.display = "none";
                        
                        document.getElementById("BinariteWeapon").style.display = "none";
                        document.getElementById("BinariteShield").style.display = "none";
                        document.getElementById("BinariteEngine").style.display = "none";
                        
                        document.getElementById("ShrodinoidWeapon").style.display = "none";
                        document.getElementById("ShrodinoidShield").style.display = "none";
                        document.getElementById("ShrodinoidEngine").style.display = "none";
                        
                        document.getElementById("HumanWeapon").style.display = "none";
                        document.getElementById("HumanShield").style.display = "none";
                        document.getElementById("HumanEngine").style.display = "none";
                        
                        //setting the display style to "inline" for the selected race's divs
                        if (raceSelect.selectedIndex == 0)//if true, Skumm has been selected
                        {
                        document.getElementById("SkummWeapon").style.display = "inline";
                        document.getElementById("SkummShield").style.display = "inline";
                        document.getElementById("SkummEngine").style.display = "inline";
                        document.getElementById("SkummName").style.display = "inline";
                        } 
                        if (raceSelect.selectedIndex == 1)//if true, Decapodian has been selected
                        {
                        document.getElementById("DecapodianWeapon").style.display = "inline";
                        document.getElementById("DecapodianShield").style.display = "inline";
                        document.getElementById("DecapodianEngine").style.display = "inline";
                        document.getElementById("DecapodianName").style.display = "inline";
                        }
                        if (raceSelect.selectedIndex == 2)//if true, Binarite has been selected
                        {
                        document.getElementById("BinariteWeapon").style.display = "inline";
                        document.getElementById("BinariteShield").style.display = "inline";
                        document.getElementById("BinariteEngine").style.display = "inline";
                        document.getElementById("BinariteName").style.display = "inline";
                        } 
                        if (raceSelect.selectedIndex == 3)//if true, Shrodinoid has been selected
                        {
                        document.getElementById("ShrodinoidWeapon").style.display = "inline";
                        document.getElementById("ShrodinoidShield").style.display = "inline";
                        document.getElementById("ShrodinoidEngine").style.display = "inline";
                        document.getElementById("ShrodinoidName").style.display = "inline";
                        } 
                        if (raceSelect.selectedIndex == 4)//if true, Human has been selected
                        {
                        document.getElementById("HumanWeapon").style.display = "inline";
                        document.getElementById("HumanShield").style.display = "inline";
                        document.getElementById("HumanEngine").style.display = "inline";
                        document.getElementById("HumanName").style.display = "inline";
                        }  
                        
                    }
                    window.onload = UpdateRaceImage;
                </script>
                <tr>
                    <td>Race:</td>
                    <td style="width: 20%">
                        <%= Html.DropDownList("raceId", new { onChange = "UpdateRaceImage();" })%>
                        <%= Html.ValidationMessage("raceId")%> 
                        
                    </td>
                </tr>     
              </table>
              <table style="width: 100%">    
                
                <%--Racial Modifiers --%>
                <tr>
                    <td style="width: 20%" align="left">Racial Modifiers</td>
                    <td style="width: 20%" align="center">Weapon</td>
                    <td style="width: 20%" align="center">Shield</td>
                    <td style="width: 20%" align="center">Accuracy</td>
                    <td style="width: 20%" align="center">Engine</td> 
                </tr>
                <% string[,] races = new string[6, 6]; %>
                <% int i = 1; %>
                <tr>
                <% foreach (Race race in (Race [])ViewData["Races"])
                   { %>
                  
                   <% races[i, 1] = race.Weapons.ToString(); %>
                   <% races[i, 2] = race.Shields.ToString(); %>
                   <% races[i, 3] = race.Engine.ToString(); %>
                   <% i++; %>
                   <% } %>
                    <td style="width: 20%" align="left"></td>
                    <td style="width: 20%" align="center"><div class="race" id="SkummWeapon"><%= races[1, 1]%></div><div class="race" id="DecapodianWeapon"><%= races[2, 1]%></div>
                            <div class="race" id="BinariteWeapon"><%= races[3, 1]%></div><div class="race" id="ShrodinoidWeapon"><%= races[4, 1]%></div><div class="race" id="HumanWeapon"><%= races[5, 1]%></div></td>
                    <td style="width: 20%" align="center"><div class="race" id="SkummShield"><%= races[1, 2]%></div><div class="race" id="DecapodianShield"><%= races[2, 2]%></div>
                            <div class="race" id="BinariteShield"><%= races[3, 2]%></div><div class="race" id="ShrodinoidShield"><%= races[4, 2]%></div><div class="race" id="HumanShield"><%= races[5, 2]%></div></td>
                    <td style="width: 20%" align="center">?</td>
                    <td style="width: 20%" align="center"><div class="race" id="SkummEngine"><%= races[1, 3]%></div><div class="race" id="DecapodianEngine"><%= races[2, 3]%></div>
                            <div class="race" id="BinariteEngine"><%= races[3, 3]%></div><div class="race" id="ShrodinoidEngine"><%= races[4, 3]%></div><div class="race" id="HumanEngine"><%= races[5, 3]%></div></td> 
                </tr>
                <tr>
                
                
                </tr>
                
                <tr>
                    <td colspan="5" align="center"><input type="submit" value="Create Player" /></td>
                </tr>
              </table>
        <% } %>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
