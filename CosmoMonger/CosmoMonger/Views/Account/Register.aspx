<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="CosmoMonger.Views.Account.Register" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="../../Scripts/digitialspaghetti.password.min.js">
    </script>
    <script type="text/javascript">
        $(function() {
            $('.password').pstrength({ minChar: <%=Html.Encode(ViewData["PasswordLength"])%> });
        });
    </script>

    <h2>Account Creation</h2>
    <p>
        Use the form below to create a new account. 
    </p>
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) { %>
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
                        <%= Html.TextBox("email") %>
                        <%= Html.ValidationMessage("email") %>
                    </td>
                </tr>
                <tr>
                    <td>Password:</td>
                    <td>
                        <%= Html.Password("password", ViewData["password"], new { Class = "password"}) %>
                        <%= Html.ValidationMessage("password") %>
                    </td>
                </tr>
                <tr>
                    <td>Confirm password:</td>
                    <td>
                        <%= Html.Password("confirmPassword") %>
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
                    <td><input type="submit" value="Register" /></td>
                </tr>
            </table>
        </div>
    <% } %>
</asp:Content>
