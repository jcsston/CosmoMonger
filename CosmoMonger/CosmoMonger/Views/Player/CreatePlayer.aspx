<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="CreatePlayer.aspx.cs" Inherits="CosmoMonger.Views.Player.Create" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <p>
            <span style="font-size: medium;">
            Welcome! Before you can begin playing, you 
            need to set up a player profile.
            </span>
        </p>
        <p>
            <span style="font-size: medium;">
            You may wish to glance at the rules before selecting your character's race.
            </span>
        </p>
        <%= Html.ValidationSummary() %>
        <% using (Html.BeginForm()) { %>
            <table>
                <tr>
                    <td>Name:</td>
                    <td>
                        <%= Html.TextBox("name") %>
                        <%= Html.ValidationMessage("name")%>
                    </td>
                </tr>
                <tr>
                    <td>Race:</td>
                    <td>
                        <%= Html.DropDownList("raceId") %>
                        <%= Html.ValidationMessage("raceId")%>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td><input type="submit" value="Create Player" /></td>
                </tr>
            </table>
        <% } %>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
