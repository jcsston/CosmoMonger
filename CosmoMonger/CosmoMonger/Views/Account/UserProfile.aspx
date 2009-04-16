<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>User Profile</title>
    <script type="text/javascript">
    //<![CDATA[
        $(document).ready(function() {
            $("#joinDate").datetimeUTCtoLocal();
        });
    //]]>
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h1>User Profile for <%= Html.Encode(ViewData["Name"])%></h1>
<br />
<p class="up-links">Join Date: <span id="joinDate"><%= Html.Encode(ViewData["JoinDate"])%></span></p>
<p class="up-links"><%= Html.ActionLink("Change My Password", "ChangePassword", "Account")%></p>
<p class="up-links">Email: <%= Html.Encode(ViewData["Email"])%></p>
<p class="up-links"><%= Html.ActionLink("Change My Email Account", "ChangeEmail", "Account")%></p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
