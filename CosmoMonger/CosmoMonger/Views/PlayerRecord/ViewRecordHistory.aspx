<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>View Player Record History</title>
    <!--[if IE]><script type="text/javascript" src="/Scripts/excanvas.pack.js"></script><![endif]-->
    <script type="text/javascript" src="/Scripts/jquery.flot.pack.js"></script>
    <script type="text/javascript">
        $(function() {
            var datasets = {
<% 
    PlayerRecord[] recordHistory = (PlayerRecord[])ViewData["RecordHistory"];
    var recordTypes = from Player.RecordType s in Enum.GetValues(typeof(Player.RecordType))
                      select new { ID = s, Name = Regex.Replace(s.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim() };
    foreach (var recordType in recordTypes)
    {
        FastDynamicPropertyAccessor.PropertyAccessor prop = new FastDynamicPropertyAccessor.PropertyAccessor(typeof(PlayerRecord), recordType.ID.ToString());
        string recordLabel = "\"" + Html.AttributeEncode(recordType.Name) + "\"";
        %>
        <%=recordType.ID%> : { label: <%=recordLabel%>, data: [ 
        <%
        foreach (PlayerRecord record in recordHistory)
        {
            %>[<%=record.TimePlayed%>, <%=Html.AttributeEncode(prop.Get(record))%>], <%
        }
        %>
        []]},
        <%
    }
%>
            "" : {}
            };
            
            $("#recordType").change(plotAccordingToSelection);

            function plotAccordingToSelection() {
                var data = [];

                $("#recordType").find("option:selected").each(function () {
                    var key = $(this).attr("value");
                    if (key && datasets[key])
                        data.push(datasets[key]);
                });

                if (data.length > 0) {
                    $.plot($("#recordHistory"), data);
                }
            }

            plotAccordingToSelection();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>View Player Record History</h2>
    
    <div id="recordHistory" style="width:700px; height:400px;"></div>
    <p id="records">Records:
    <%=Html.DropDownList("recordType", (SelectList)ViewData["recordType"]) %>
    </p>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>

