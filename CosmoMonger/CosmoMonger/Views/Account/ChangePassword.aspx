<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <title>Change Password</title>
    <script type="text/javascript" src="/Scripts/jquery.validate.min.js"></script>
    <script type="text/javascript">
    //<![CDATA[
        $(document).ready(function() {
            $("#passwordForm").validate({
                rules: {
                    newPassword: {
                        required: true,
                        minlength: 8
                    },
                    confirmPassword: {
                        equalTo: "#newPassword"
                    }
                },
                messages: {
                    newPassword: {
                        required: "Please enter in a new password",
                        minlength: "Passwords need to be at least 8 characters"
                    },
                    confirmPassword: "Please make sure this matches the new password you entered in above"
                }
            });
        });
    //]]>
    </script>
</asp:Content>
<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Change Password</h1>
    <p>
        Use the form below to change your password. 
    </p>
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm("ChangePassword", "Account", FormMethod.Post, new { id = "passwordForm" }))
       { %>
        <div>
            <table>
                <tr>
                    <td>Current password:</td>
                    <td>
                        <%= Html.Password("currentPassword") %>
                        <%= Html.ValidationMessage("currentPassword") %>
                    </td>
                </tr>
                <tr>
                    <td>New password:</td>
                    <td>
                        <%= Html.Password("newPassword") %>
                        <%= Html.ValidationMessage("newPassword") %>
                    </td>
                </tr>
                <tr>
                    <td>Confirm new password:</td>
                    <td>
                        <%= Html.Password("confirmPassword") %>
                        <%= Html.ValidationMessage("confirmPassword") %>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td><input type="submit" value="Change Password" /></td>
                </tr>
            </table>
        </div>
    <% } %>
</asp:Content>
