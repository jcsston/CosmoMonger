<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>View Player Record History</title>
    <!--[if IE]><script type="text/javascript" src="/Scripts/excanvas.pack.js"></script><![endif]-->
    <script type="text/javascript" src="/Scripts/jquery.flot.pack.js"></script>
    <script type="text/javascript">
        var dataSetCache = {};
        function plotAccordingToSelection() {
            $("#recordType").find("option:selected").each(function() {
                var key = $(this).attr("value");
                var label = $(this).text();

                // See if the data is in the cache
                if (dataSetCache[key]) {
                    // Plot using the cached versopn
                    $.plot($("#recordHistory"), dataSetCache[key]);
                    return;
                }

                // Display loading message
                $("#loading").show();
                $.getJSON('/PlayerRecord/GetRecordHistory', { recordType: key }, function(data) {
                    var dataSet = [{ label: label, data: data}];
                    $.plot($("#recordHistory"), dataSet);

                    // Cache dataset
                    dataSetCache[key] = dataSet;

                    // Hide loading message
                    $("#loading").hide();
                });
            });
        }
        
        $(function() {
            // Attach to change event
            $("#recordType").change(plotAccordingToSelection);

            // Trigger inital selection
            plotAccordingToSelection();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>View Player Record History</h2>
    <p id="records">Record Type:
    <%=Html.DropDownList("recordType", (SelectList)ViewData["recordType"]) %>
    <span id="loading">Loading...</span>
    </p>    
    <div id="recordHistory" style="width:700px; height:400px;"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>

