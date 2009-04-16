<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Change E-Mail</title>
    <script type="text/javascript" src="/Scripts/jquery.validate.min.js"></script>
    <script type="text/javascript">
    //<![CDATA[
        $(document).ready(function() {
            $("#changeEmailForm").validate({
                rules: {
                    email: {
                        required: true,
                        email: true
                    }
                },
                messages: {
                    email: {
                        required: "We need your email address to contact you",
                        email: "Your email address must be in the format of name@domain.com"
                    }
                }
            });
        });
     //]]>
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Change Email</h2>
    
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm("ChangeEmail", "Account", FormMethod.Post, new { id = "changeEmailForm" }))
       { %>
        <div>
            <table cellspacing="5">
                <tr>
                    <td>Email:</td>
                    <td>
                        <%= Html.TextBox("email")%>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td><input id="submit" name="submit" type="submit" value="Change Email" /></td>
                </tr>
            </table>
        </div>
    <% } %>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
