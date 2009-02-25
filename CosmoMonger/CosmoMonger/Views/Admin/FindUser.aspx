<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
<title>Find User</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Find User</h1>
<%
Html.Grid<User>(
    "Matches",
    column =>
    {
        column.For(u => u.UserId);
        column.For(u => u.UserName);
        column.For(u => u.Email);
        column.For(u => u.Joined);
        column.For(u => u.LastActivity);
        column.For("Active").Do(u =>
        {%><td><%
        if (u.Active)
        {
            using (Html.BeginForm("BanUser", "Admin"))
            { 
                    %><div>
                        <input type="hidden" name="userId" value="<%=u.UserId %>" /> 
                        <input type="submit" value="Ban" onclick="return confirm('Are you sure you want to ban user <%= u.UserId %>?'));" />
                    </div><% 
            }
        }
        else
        {
            using (Html.BeginForm("UnbanUser", "Admin"))
            { 
                    %><div>
                        <input type="hidden" name="userId" value="<%=u.UserId %>" /> 
                        <input type="submit" value="Unban" onclick="return confirm('Are you sure you want to unban user <%= u.UserId %>?'));" />
                    </div><% 
            }
        }
            %></td><%
    });
        column.For(u => u.Validated
            ? "True"
            : Html.ActionLink("Validate", "VerifyEmail", "Account", new { username = u.UserName, verificationCode = u.VerificationCode }, null)
            , "Validated").DoNotEncode();
        column.For(u => u.Admin);
    }
);
%>

<h2>Find Users</h2>
<%
    using (Html.BeginForm("FindUser", "Admin", FormMethod.Get))
    {
%>
    <p>
        <label for="name">Name: </label><%=Html.TextBox("name") %>
        <input type="submit" value="Find" />
    </p>
<%
    }        
%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
</asp:Content>
