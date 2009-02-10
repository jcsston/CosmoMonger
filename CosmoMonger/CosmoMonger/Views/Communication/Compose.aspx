<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Messaging - Compose</title>
    <script type="text/javascript" src="/Scripts/jquery.validate.min.js"></script>
    <script type="text/javascript">
    <!--
        $(document).ready(function() {
            $("form").validate({
                rules: {
                    subject: "required",
                    message: "required"
                },
                // Error placement option, we want to show the error message to the user on a new line after the problem form element
                errorPlacement: function(label, element) {
                    label.insertAfter(element);
                    label.before('<br />');
                }
            });
        });
     -->
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Messaging</h1>
<p><%=Html.ActionLink("Inbox", "Inbox") %> | <%=Html.ActionLink("Sent", "Sent")%> | <b>Compose</b></p>
<%
    using (Html.BeginForm()) 
    {
%>
    <table>
        <tr>
            <td>
                <label for="toUserId">To:</label>
            </td>
            <td>
                <%=Html.DropDownList("toUserId")%>
            </td>
        </tr>
        <tr>    
            <td>
                <label for="subject">Subject:</label>
            </td>
            <td>
                <%=Html.TextBox("subject") %>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <label for="subject">Message:</label>
                <br />
                <%=Html.TextArea("message", (string)ViewData["message"], 10, 40, null)%>
           </td>
        </tr>
        <tr>
            <td></td>
            <td align="right">
                <input type="submit" value="Send" />
            </td>
       </tr>
   </table>
<% 
    }        
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
