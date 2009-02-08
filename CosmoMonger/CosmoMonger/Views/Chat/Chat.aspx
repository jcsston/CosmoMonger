<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Chat</title>
    <script type="text/javascript">
    <!--
        $(document).ready(function() {
            // Load previous chat messages
            $.getJSON("/Chat/FetchMessages", {}, function(data) {
                // Insert new messages
                //alert(data);
            });

            // Setup message submit
            $("#chatform").submit(function() {
                $.getJSON("/Chat/SendMessage", {
                    toUserId: $("#friendId").val(),
                    message: $("#message").val()
                }, function(data) {
                    // Handle message success
                    alert(data);
                });
            });
        });
    -->
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Chat</h1>
<div id="wrapper">
    <p id="messagewindow">
        <span id="loading">Loading...</span>
    </p>
<% 
    using (Html.BeginForm("Chat", "Chat", FormMethod.Post, new { id = "chatform" }))
    { 
%>
    <p>
        <%=Html.Hidden("friendId")%>
        Message: <%=Html.TextBox("message") %>
        <input id="submit" type="submit" value="Send" /><br />
    </p>
<%
    } 
%>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
