<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Travel in Progress...</title>
    <style type="text/css">
        #starField
        {
            margin-left: auto;
            margin-right: auto;
            background: black;
            width: 600px; 
            height: 400px;
            border: solid thin blue; 
            position: relative;
        }
        .star 
        {
            padding: 0px;
            margin: 0px;
            color: white;
            font: bold 13px Arial,sans-serif;
            position: absolute;
        }
    </style>
    <script type="text/javascript">
    <!--
        function checkIfTraveling() {
            $.getJSON('/Combat/CombatPending', function(data) {
                if (data.combat) {
                    document.location = '/Combat/CombatStart';
                } else {
                    setTimeout(checkIfTraveling, 1000);
                }
            });
        }

        function updateTravelTime() {
            var travelTime = parseInt($('#TimeLeft').text());
            if (travelTime > 1) {
                $('#TimeLeft').text(travelTime - 1);
                setTimeout(updateTravelTime, 1000);
            } else {
                document.location = '/Travel/Travel';
            }
        }

        var x = [], y = [], z = [];
        var R = Math.random;

        function moveStar(index, domElement) {
            var a = 50 + x[index] * z[index];
            var b = 50 + y[index] * z[index]++;
            if (!a | a < 0 | a > 96 | b < 0 | b > 94) {
                x[index] = R() * 2 - 1;
                y[index] = R() * 2 - 1;
                z[index] = 9;
            } else {
                var star = $(domElement);
                star.css('left', a + '%');
                star.css('top', b + '%');
            }
        }
            
        function moveStars() {
            var stars = $('#starField > p');
            stars.each(moveStar);
            setTimeout(moveStars, 30);
        }
        
        function buildStars() {
            var starField = $('#starField');
            for (var i = 0; i < 50; i++) {
                starField.append('<p id="star' + i + '" class="star">&bull;</p>');
            }
            moveStars();
        }

        $(document).ready(function() {
            //var totalTime = parseFloat($('#TimeLeft').text());
            //$('#ShipBlock').animate({ width: '350px' }, totalTime * 1000);
            checkIfTraveling();
            buildStars();

            setTimeout(updateTravelTime, 1000);
        });
    -->
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Travel In Progress...</h1>
    <div id="starField">    
    </div>
    <p class="center">Jump in <span id="TimeLeft"><%=ViewData["TravelTime"] %></span> seconds.</p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
