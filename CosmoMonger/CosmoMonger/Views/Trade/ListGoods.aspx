<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>List Goods</title>
    <script type="text/javascript" src="/Scripts/jquery.spin-1.0.2.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.validate.min.js"></script>
    <script type="text/javascript">
    <!--
        $(document).ready(function() {
            $('input[name=quantity]').map(function(index, domElement) {
                var buyGood = false;
                var goodId = null;
                var goodQuantity = 0;
                if (this.id.indexOf("buyQuantity") == 0) {
                    // buyQuantity is 11 chars long
                    goodId = parseInt(this.id.substring(11));
                    goodQuantity = $("#goodQuantity" + goodId).text();
                    buyGood = true;
                } else if (this.id.indexOf("sellQuantity") == 0) {
                    goodId = parseInt(this.id.substring(12));
                    goodQuantity = $("#shipGoodQuantity" + goodId).text();
                }
                
                var goodQuantityMax = goodQuantity;
                var goodQuantityMin = 1;
                var freeCargoSpace;
                if (buyGood) {
                    freeCargoSpace = parseInt($('#FreeCargoSpace').text());
                    if (goodQuantityMax > freeCargoSpace) {
                        goodQuantityMax = freeCargoSpace;
                    }
                }
                
                // Check if we need to disable the buttons due to the lack of goods
                var goodSubmitButton = $(this).next("input");
                if (goodQuantity == 0 || (buyGood && freeCargoSpace == 0)) {
                    $(this).attr("disabled", "disabled");
                    goodSubmitButton.attr("disabled", "disabled");
                    goodQuantityMin = 0;
                } else {
                    // We only add validatation to the goods the user can buy/sell
                    $(this).parent().parent('form').validate({
                        rules: {
                            quantity: {
                                required: true,
                                digits: true,
                                min: goodQuantityMin,
                                max: goodQuantityMax
                            }
                        },
                        // Error placement option, we want to show the error message to the user on a new line after the problem form element
                        errorPlacement: function (label, element) {
                            label.insertAfter(element.siblings('input[type=submit]'));
                            label.before('<br />');
                        }
                    });
                }

                // Add the good quantity spinner
                $(this).spin({ min: goodQuantityMin, max: goodQuantityMax, timeInterval: 300 });
                
                // Set the default value to buy the min number of goods
                if ($(this).val() == 0) {
                    $(this).val(goodQuantityMin);
                }
            });
        });
    -->
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<%
    CosmoSystem currentSystem = (CosmoSystem)ViewData["CurrentSystem"];
    SystemGood[] systemGoods = (SystemGood[])ViewData["SystemGoods"];
    ShipGood[] shipGoods = (ShipGood[])ViewData["ShipGoods"];
%>
<h1 class="goods">Buy &amp; Sell in the <%=Html.Encode(currentSystem.Name) %> System</h1>
<hr />
<table class="goods">
<tr><td rowspan="11"><img class="systemImg" alt="System Image" src="/Content/System/<%=Html.Encode(currentSystem.Name) %>.png" /></td><th>Name</th><th>Base Price</th><th>Price</th><th># in System</th><th>Buy</th><th># in Ship</th><th>Sell</th></tr>
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
            <%=Html.TextBox("quantity", shipGoodQuantity, new { id = "sellQuantity" + good.GoodId, size = 2, maxlength = 3 })%>
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
    <tr><td>$<%= ViewData["CashCredits"] %></td><td>$<%= ViewData["BankCredits"]%></td><td id="FreeCargoSpace"><%= ViewData["FreeCargoSpace"] %></td></tr>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
