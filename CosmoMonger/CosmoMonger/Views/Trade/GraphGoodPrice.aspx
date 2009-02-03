<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!--[if IE]><script type="text/javascript" src="/Scripts/excanvas.pack.js"></script><![endif]-->
    <script type="text/javascript" src="/Scripts/jquery.flot.js"></script>

<script type="text/javascript">
    $(function() {
        var datasets = {
<%
    SystemGood[] goods = (SystemGood[])ViewData["Goods"];
    IEnumerable<Dictionary<DateTime, int>> priceHistory = (IEnumerable<Dictionary<DateTime, int>>)ViewData["PriceHistory"];
    DateTime epcohTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    int goodIndex = 0;
    foreach (Dictionary<DateTime, int> goodHistory in priceHistory)
    {
        SystemGood good = goods[goodIndex];
        string goodLabel = "\"" + Html.AttributeEncode(good.Good.Name) + "\"";
        Response.Write(goodLabel + ":\n { label: " + goodLabel + ", data: [");
        foreach (KeyValuePair<DateTime, int> price in goodHistory)
        {
            TimeSpan unixTime = price.Key - epcohTime;
            Response.Write("[" + unixTime.TotalMilliseconds + ", " + price.Value + "],");
        }
        Response.Write("[]]},\n");
        goodIndex++;
    }
%>
            "" : {}
        };

        // hard-code color indices to prevent them from shifting as
        // goods are turned on/off
        var i = 0;
        $.each(datasets, function(key, val) {
            val.color = i;
            ++i;
        });

        // insert checkboxes 
        var choiceContainer = $("#goods");
        $.each(datasets, function(key, val) {
            if (key) {
                choiceContainer.append('&nbsp;<input type="checkbox" name="' + key + '" checked="checked" >' + val.label + '</input>');
            }
        });
        choiceContainer.find("input").click(plotAccordingToChoices);

        
        function plotAccordingToChoices() {
            var data = [];

            choiceContainer.find("input:checked").each(function () {
                var key = $(this).attr("name");
                if (key && datasets[key])
                    data.push(datasets[key]);
            });

            if (data.length > 0) {
                $.plot($("#priceHistory"), data, { 
                    points: { show: true },
                    lines: { show: true },
                    grid: { hoverable: true}, 
                    xaxis: { mode: "time" }
                });
            }
        }

        plotAccordingToChoices();
        
        function showTooltip(x, y, contents) {
            $('<div id="tooltip">' + contents + '</div>').css( {
                position: 'absolute',
                display: 'none',
                top: y + 5,
                left: x + 5,
                border: '1px solid #fdd',
                padding: '2px',
                'background-color': '#fee',
                'color': 'black',
                opacity: 0.80
            }).appendTo("body").fadeIn(200);
        }
        
        var previousPoint = null;
        $("#priceHistory").bind("plothover", function (event, pos, item) {
            if (item) {
                if (previousPoint != item.datapoint) {
                    previousPoint = item.datapoint;
                    
                    $("#tooltip").remove();
                    var x = item.datapoint[0],
                        y = item.datapoint[1];
                    
                    showTooltip(item.pageX, item.pageY,
                                item.series.label + "<br />" + new Date(x) + "<br />$" + y);
                }
            } else {
                $("#tooltip").remove();
                previousPoint = null;            
            }
        });

    });
</script>

<% using (Html.BeginForm("GraphGoodPrice", "Trade", FormMethod.Get)) { %>
<p>
    <table>
        <tr>
            <td>System: <%=Html.DropDownList("systemId") %></td>
            <td><input type="submit"/></td>
        </tr>
    </table>
</p>
<% } %>

<div id="priceHistory" style="width:700px; height:400px;"></div>

<p id="goods">Goods:</p>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
