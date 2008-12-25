<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="CreatePlayer.aspx.cs" Inherits="CosmoMonger.Views.Player.Create" %>
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
                    <td style="width: 15%">Glop</td>
                </tr>
                <script type="text/javascript" language="javascript">
                    function UpdateRaceImage() {
                        var raceSelect = document.getElementById('raceId');
                        var raceOption = raceSelect.options[raceSelect.selectedIndex];
                        document.getElementById('RaceImg').src = '../Content/Races/' + raceOption.text + '.jpg';
                    }
                    window.onload = UpdateRaceImage;
                </script>
                <tr>
                    <td>Race:</td>
                    <td style="width: 20%">
                        <%= Html.DropDownList("raceId", new { onChange = "UpdateRaceImage();" })%>
                        <%= Html.ValidationMessage("raceId")%>
                    </td>
              </table>
              <table style="width: 100%">    
                </tr>
                <%--Racial Modifiers --%>
                <tr>
                    <td style="width: 20%" align="left">Racial Modifiers</td>
                    <td style="width: 20%" align="center">Weapon</td>
                    <td style="width: 20%" align="center">Shield</td>
                    <td style="width: 20%" align="center">Accuracy</td>
                    <td style="width: 20%" align="center">Engine</td> 
                </tr>
                <tr>
                    <td style="width: 20%" align="left"></td>
                    <td style="width: 20%" align="center">0</td>
                    <td style="width: 20%" align="center">+1</td>
                    <td style="width: 20%" align="center">0</td>
                    <td style="width: 20%" align="center">-1</td> 
                </tr>
                <tr>
                <td colspan="5"><div id="RaceDesc">Although slow and ponderous, these crab-like aliens were the first to achieve interstellar travel in this sector.  They were also the first to start an interstellar war when they consumed the ambassador representing the Skumm nation during a scouting expedition to system D2O.  Although the resulting Skumm-Crab War has officially concluded, bad blood exists between the two races to this day.</div>
                </td>
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
