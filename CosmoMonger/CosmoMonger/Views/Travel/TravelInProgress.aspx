<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Travel in Progress...</title>
    <script type="text/javascript">
    <!--
        function checkIfTraveling() {
            $.getJSON('/Travel/TravelProgress', function(data) {
                if (data.combat) {
                    document.location = '/Combat/CombatStart';
                } else if (!data.travel) {
                    document.location = '/Travel';
                } else {
                    setTimeout(checkIfTraveling, 1000);
                }
            });
        }

        function updateTravelTime() {
            $('#TimeLeft').text(parseInt($('#TimeLeft').text()) - 1);
            setTimeout(updateTravelTime, 1000);
        }

        $(document).ready(function() {
            var totalTime = parseFloat($('#TimeLeft').text());
            $('#ShipBlock').animate({ width: '350px' }, totalTime * 1000);
            checkIfTraveling();
            setTimeout(updateTravelTime, 1000);
        });
    -->
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Travel In Progress...</h1>
    <p>Jump in <span id="TimeLeft"><%=ViewData["TravelTime"] %></span> seconds.</p>
    <p>JumpDrive Charging...</p>
    <div style="width: 350px; height: 50px; border: solid thin blue; position: relative;">    
        <div id="ShipBlock" style="background-color: grey; position: absolute; width: 0px; height: 50px; left: 0px;" />
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
