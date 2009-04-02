<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Bank</title>
    <script type="text/javascript" src="/Scripts/jquery.spin-1.0.2.js"></script>
    <script type="text/javascript">
    <!--
        $(document).ready(function() {
            var cashCredits = $.trim($("#cashCredits").text().replace('$', ''));
            $('#depositCredits').spin({ min: 0, max: cashCredits, timeInterval: 100, interval: 10 });
            var bankCredits = $.trim($("#bankCredits").text().replace('$', ''));
            $('#withdrawCredits').spin({ min: 0, max: bankCredits, timeInterval: 100, interval: 10 });
        });
    -->
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<% bool hasBank = (bool)ViewData["BankAvailable"]; %>

<% if (hasBank)
   { %> 
<h1>Welcome To The Intergalactic Bank of <%= Html.Encode(ViewData["SystemName"])%></h1>
<% } %>
<% else
    { %>
<h1>Welcome To The Online Intergalactic Bank</h1>
<% } %>
<table style="width: 100%">
<tr>
    <td align="center"><img id="BankLogo" src="/Content/IGBank.png" alt="Bank Logo" /></td>
</tr>
<tr>
    <td class="bank-columnHeaders"><%= Html.Encode(ViewData["PlayerName"]) %>'s Bank Account</td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
    <td class="bank-columnHeaders">Account Balance</td>
</tr>
<tr>
    <td class="bank-data" id="bankCredits">$&nbsp;<%= Html.Encode(ViewData["BankCredits"]) %></td>
</tr>
<% if (hasBank)
   { %>
   
<tr><td class="bank-data">
<%
        using (Html.BeginForm("Withdraw", "Bank")) { 
%>
            <div>
            
            <%=Html.TextBox("withdrawCredits", 0, new { size = 3})%>
            <input type="submit" value="Withdraw" />
            </div>
<% } %>

</td></tr>   
<tr><td class="bank-columnHeaders">Credits Available for Deposit</td>
</tr>
<tr>
    <td class="bank-data" id="cashCredits">$&nbsp;<%= Html.Encode(ViewData["CashCredits"])%></td>
</tr>

<tr><td class="bank-data">
<%
        using (Html.BeginForm("Deposit", "Bank")) { 
%>
            <div>
            
            <%=Html.TextBox("depositCredits", 0, new { size = 3})%>
            <input type="submit" value="Deposit" />
            </div>
<% } %>

</td></tr>



<% } %> 
<% else
    { %>
<tr>
    <td class="bank-data">If you would like to make a deposit or a withdrawl, you must travel to a system with a bank.</td>
</tr>
<% } %>
</table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
