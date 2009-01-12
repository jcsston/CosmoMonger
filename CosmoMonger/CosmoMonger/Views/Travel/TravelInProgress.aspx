﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript" type="text/javascript">
        function checkIfTraveling() {
            var timeLeftSpan = document.getElementById('TimeLeft');
            var timeLeft = parseInt(timeLeftSpan.innerHTML) - 1;
            timeLeftSpan.innerHTML = timeLeft;
            if (timeLeft <= 0) {
                document.location = '/Travel';
            } else {
                setTimeout(checkIfTraveling, 1000);
            }
            
        }
        setTimeout(checkIfTraveling, 1000);
    </script>
    <h1>Travel In Progress...</h1>
    <p>Jump in <span id="TimeLeft"><%=ViewData["TravelTime"] %></span> seconds.</p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
