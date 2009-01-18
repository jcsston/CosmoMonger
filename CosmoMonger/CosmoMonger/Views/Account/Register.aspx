<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../../Scripts/jquery.validate.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function() {
            $("#regForm").validate({
                rules: {
                    username: "required",
                    email: {
                        required: true,
                        email: true
                    },
                    password: {
                        required: true,
                        minlength: 8
                    },
                    confirmPassword: {
                        equalTo: "#password"
                    }
                },
                messages: {
                    username: "Please enter in a username",
                    email: {
                        required: "We need your email address to contact you",
                        email: "Your email address must be in the format of name@domain.com"
                    },
                    password: {
                        required: "Please enter in a password",
                        minlength: "Passwords need to be at least 8 characters"
                    },
                    confirmPassword: "Please make sure this matches the password you entered in above"
                }
            });
        });
    </script>

    <h2>Account Creation</h2>
    <p>
        Use the form below to create a new account. 
    </p>
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm("Register", "Account", FormMethod.Post, new { id = "regForm"})) { %>
        <div>
            <table cellspacing="5">
                <tr>
                    <td>Username:</td>
                    <td>
                        <%= Html.TextBox("username") %>
                        <%= Html.ValidationMessage("username") %>
                    </td>
                </tr>
                <tr>
                    <td>Email:</td>
                    <td>
                        <%= Html.TextBox("email")%>
                        <%= Html.ValidationMessage("email") %>
                    </td>
                </tr>
                <tr>
                    <td>Password:</td>
                    <td>
                        <%= Html.Password("password")%>
                        <%= Html.ValidationMessage("password") %>
                    </td>
                </tr>
                <tr>
                    <td>Confirm password:</td>
                    <td>
                        <%= Html.Password("confirmPassword")%>
                        <%= Html.ValidationMessage("confirmPassword") %>
                    </td>
                </tr>
                <tr>
                    <td>Verify you are human:</td>
                    <td>
                        <% /* The RecaptchaControl pulls our private/public key from the AppSettings. */ %>
                        <recaptcha:RecaptchaControl
                          ID="recaptcha"
                          runat="server"
                          Theme="blackglass"
                          />
                        <%= Html.ValidationMessage("recaptcha")%>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td><input id="submit" name="submit" type="submit" value="Register" /></td>
                </tr>
            </table>
        </div>
    <% } %>
</asp:Content>
