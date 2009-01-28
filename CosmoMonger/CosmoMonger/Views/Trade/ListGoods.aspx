<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    $(document).ready(function() {
        $(".buyGood").bind("click", function() {
            document.forms.buyForm.goodId.value = this.value;
            document.forms.buyForm.quantity.value = prompt("How many of this good would you like to buy?", 0);
            document.forms.buyForm.submit();
        });
        $(".sellGood").bind("click", function() {
            document.forms.sellForm.goodId.value = this.value;
            document.forms.sellForm.quantity.value = prompt("How many of this good would you like to sell?", 0);
            document.forms.sellForm.submit();
        });
    });
</script>
<h1>Buy &amp; Sell Goods</h1>
<br />
<table class="goods">
<tr><th>Name</th><th>Base Price</th><th>Price</th><th># in System</th><th># in Ship</th><th></th></tr>
<%
    CosmoMonger.Models.SystemGood [] systemGoods = (CosmoMonger.Models.SystemGood [])ViewData["SystemGoods"];
    CosmoMonger.Models.ShipGood [] shipGoods = (CosmoMonger.Models.ShipGood [])ViewData["ShipGoods"];

    foreach (CosmoMonger.Models.SystemGood good in systemGoods)
    {
%>
        <tr>
            <td>
                <%=Html.Encode(good.Good.Name) %>
                <%
                    if (good.Good.Contraband)
                    {
                %>
                *
                <% 
                    } 
                %>
            </td>
            <td>
                $<%=good.Good.BasePrice %>
            </td>
            <td>
                $<%=good.Price %>
            </td>
            <td>
                <%=good.Quantity %>
            </td>
            <td>
                <%
                    int shipCount = (from g in shipGoods
                                     where g.Good == good.Good
                                     select g.Quantity).SingleOrDefault();
                %>
                <%=shipCount %>
            </td>
            <td>
                <button class="buyGood" value="<%=good.GoodId %>">Buy</button>
                <button class="sellGood" value="<%=good.GoodId %>">Sell</button>
            </td>
        </tr>
<%
    }
%>
</table>
<% using (Html.BeginForm("BuyGoods", "Trade", FormMethod.Get, new { id = "buyForm" })) { %>
<div>
    <%=Html.Hidden("goodId") %>
    <%=Html.Hidden("quantity") %>
</div>
<% } %>
<% using (Html.BeginForm("SellGoods", "Trade", FormMethod.Get, new { id = "sellForm" })) { %>
<div>
    <%=Html.Hidden("goodId") %>
    <%=Html.Hidden("quantity") %>
</div>
<% } %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
