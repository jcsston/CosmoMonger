<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" src="/Scripts/jquery.spin-1.0.2.js"></script>
<script type="text/javascript">
    $(document).ready(function() {
        $('input[name=quantity]').map(function(index, domElement) {
            var goodId = null;
            var goodQuantity = 0;
            if (this.id.indexOf("buyQuantity") == 0) {
                // buyQuantity is 11 chars long
                goodId = parseInt(this.id.substring(11));
                goodQuantity = $("#goodQuantity" + goodId).text();
            } else if (this.id.indexOf("sellQuantity") == 0) {
                goodId = parseInt(this.id.substring(12));
                goodQuantity = $("#shipGoodQuantity" + goodId).text();
            }
            var goodSubmitButton = $(this).next("input");
            if (goodQuantity == 0) {
                $(this).attr("disabled", "disabled");
                goodSubmitButton.attr("disabled", "disabled");
            }
            $(this).spin({ min: 0, max: goodQuantity, timeInterval: 300 });
        });
    });
</script>
<%
    CosmoSystem currentSystem = (CosmoSystem)ViewData["CurrentSystem"];
    SystemGood[] systemGoods = (SystemGood[])ViewData["SystemGoods"];
    ShipGood[] shipGoods = (ShipGood[])ViewData["ShipGoods"];
%>
<center>
<h1>Buy &amp; Sell in the <%=Html.Encode(currentSystem.Name) %> System</h1>
</center>
<hr />
<table class="goods">
<tr><th>Name</th><th>Base Price</th><th>Price</th><th># in System</th><th>Buy</th><th># in Ship</th><th>Sell</th></tr>
<%
    foreach (SystemGood good in systemGoods)
    {
        // Get the number of this good type onboard the players ship
        int shipGoodQuantity = (from g in shipGoods 
                                where g.Good == good.Good
                                select g.Quantity).SingleOrDefault();
%>
        <tr>
            <td><%=Html.Encode(good.Good.Name) + (good.Good.Contraband ? "*" : "") %></td>
            <td>$<%=good.Good.BasePrice %></td>
            <td id="goodPrice<%=good.GoodId %>">$<%=good.Price %></td>
            <td id="goodQuantity<%=good.GoodId %>"><%=good.Quantity %></td>
            <td>
<%
        using (Html.BeginForm("BuyGoods", "Trade")) { 
%>
            <div>
            <%=Html.Hidden("goodId", good.GoodId, new { id = "buyGoodId" + good.GoodId })%>
            <%=Html.TextBox("quantity", 0, new { id = "buyQuantity" + good.GoodId, size = 2, maxlength = 3 })%>
            <input id="buyGood<%=good.GoodId %>" type="submit" value="Buy" />
            </div>
<% 
        } 
%>
            </td>
            <td id="shipGoodQuantity<%=good.GoodId %>"><%=shipGoodQuantity %></td>
            <td>
<%
    using (Html.BeginForm("SellGoods", "Trade")) { 
%>
            <div>
            <%=Html.Hidden("goodId", good.GoodId, new { id = "sellGoodId" + good.GoodId })%>
            <%=Html.TextBox("quantity", 0, new { id = "sellQuantity" + good.GoodId, size = 2, maxlength = 3 })%>
            <input id="sellGood<%=good.GoodId %>" type="submit" value="Sell" />
            </div>
<% 
        } 
%>
            </td>
        </tr>
<%
    }
%>
</table>
<hr />
<table class="goods goodsCenter">
    <tr><th>Cash Credits</th><th>Bank Credits</th><th>Cargo Space Free</th></tr>
    <tr><td><%= ViewData["CashCredits"] %></td><td><%= ViewData["BankCredits"]%></td><td><%= ViewData["FreeCargoSpace"] %></td></tr>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
