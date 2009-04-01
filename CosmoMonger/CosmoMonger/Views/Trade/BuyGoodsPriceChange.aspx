<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Good Price Change</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Price Change on Buying Good</h2>
    <p>
    The price has changed on the <%=Html.Encode(ViewData["goodName"]) %> good you were buying.
    </p>
    <table>
    <tr>
        <th>Old Price:</th>
        <td>$<%=Html.Encode(ViewData["oldPrice"]) %></td>
        <td>*</td>
        <td><%=Html.Encode(ViewData["quantity"]) %></td>
        <td>=</td>
        <td>$<%=(int)ViewData["quantity"] * (int)ViewData["oldPrice"]%></td>
    </tr>
    <tr>
        <th>New Price:</th>
        <td>$<%=Html.Encode(ViewData["newPrice"]) %></td>
        <td>*</td>
        <td><%=Html.Encode(ViewData["quantity"]) %></td>
        <td>=</td>
        <td>$<%=(int)ViewData["quantity"] * (int)ViewData["newPrice"]%></td>
    </tr>
    </table>
    <p>
    Do you still wish to buy <%=Html.Encode(ViewData["quantity"]) %> <%=Html.Encode(ViewData["goodName"]) %> good(s)?
    </p>
<%
        using (Html.BeginForm("BuyGoods", "Trade")) { 
%>
            <div>
            <%=Html.Hidden("goodId", ViewData["goodId"])%>
            <input id="price" type="hidden" value="<%=Html.AttributeEncode(ViewData["newPrice"])%>" name="price"/>
            <%=Html.Hidden("quantity", ViewData["quantity"])%>
            <input type="submit" value="Confirm" />
            </div>
<% 
        } 
%>
<p>
    <%=Html.ActionLink("Back to Trading Goods", "ListGoods") %>
</p>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>