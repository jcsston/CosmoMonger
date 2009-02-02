<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!--[if IE]><script type="text/javascript" src="/Scripts/excanvas.pack.js"></script><![endif]-->
    <script type="text/javascript" src="/Scripts/jquery.flot.js"></script>

<script type="text/javascript">
    $(function() {
        var d1 =
        [
<%
    Dictionary<DateTime, int> priceHistory = (Dictionary<DateTime, int>)ViewData["PriceHistory"];
    DateTime epcohTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    foreach (KeyValuePair<DateTime, int> price in priceHistory)
    {
        TimeSpan unixTime = price.Key - epcohTime;
        Response.Write("[" + unixTime.TotalMilliseconds + ", " + price.Value + "],");
    }
%>
        []
        ];

        $.plot($("#priceHistory"), [d1], { xaxis: { mode: "time" }});
    });
</script>

<% using (Html.BeginForm("GraphGoodPrice", "Trade", FormMethod.Get)) { %>
<p>
    <table>
        <tr>
            <td>System: <%=Html.DropDownList("systemId") %></td>
            <td>Good: <%=Html.DropDownList("goodId") %></td>
            <td><input type="submit"/></td>
        </tr>
    </table>
</p>
<% } %>

<div id="priceHistory" style="width:700px; height:400px;"></div>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
