<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DatabaseSize.aspx.cs" Inherits="CosmoMonger.DatabaseSize" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Database Size</title>
</head>
<body runat="server">
    <form id="form1" runat="server">
    <h1>CosmoMonger Database</h1>
    <div>
    <asp:SqlDataSource ID="SqlDataSourceCosmoMongerDbSize" runat="server" 
        ConnectionString="<%$ ConnectionStrings:CosmoMongerConnectionString %>" 
        SelectCommand="EXEC sp_spaceused"></asp:SqlDataSource>
    <asp:DataList ID="DataListCosmoMongerDbSize" runat="server" 
            DataSourceID="SqlDataSourceCosmoMongerDbSize" BackColor="White" 
            BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            GridLines="Horizontal">
        <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
        <AlternatingItemStyle BackColor="#F7F7F7" />
        <ItemStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
        <SelectedItemStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
        <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
        <ItemTemplate>
            database_name:
            <asp:Label ID="database_nameLabel" runat="server" 
                Text='<%# Eval("database_name") %>' />
            <br />
            database_size:
            <asp:Label ID="database_sizeLabel" runat="server" 
                Text='<%# Eval("database_size") %>' />
            <br />
            unallocated space:
            <asp:Label ID="unallocated_spaceLabel" runat="server" 
                Text='<%# Eval("[unallocated space]") %>' />
            <br />
            <br />
        </ItemTemplate>
    </asp:DataList>
    <asp:SqlDataSource ID="SqlDataSourceCosmoMongerTableSize" runat="server" 
            SelectCommand="GetDBTableSize" SelectCommandType="StoredProcedure" 
            ConnectionString="<%$ ConnectionStrings:CosmoMongerConnectionString %>">
    </asp:SqlDataSource>
        <asp:GridView ID="GridViewCosmoMongerTableSize" runat="server" 
            AllowSorting="True" CellPadding="3" 
            DataSourceID="SqlDataSourceCosmoMongerTableSize" GridLines="Horizontal" 
            BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px">
            <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
            <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
            <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
            <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
            <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
            <AlternatingRowStyle BackColor="#F7F7F7" />
        </asp:GridView>
    </div>
    
    <h1>Log Database</h1>
    <div>
    <asp:SqlDataSource ID="SqlDataSourceLogDbSize" runat="server" 
        ConnectionString="<%$ ConnectionStrings:LoggingConnectionString %>" 
        SelectCommand="EXEC sp_spaceused"></asp:SqlDataSource>
    <asp:DataList ID="DataListLogDbSize" runat="server" 
            DataSourceID="SqlDataSourceLogDbSize" BackColor="White" 
            BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            GridLines="Horizontal">
        <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
        <AlternatingItemStyle BackColor="#F7F7F7" />
        <ItemStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
        <SelectedItemStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
        <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
        <ItemTemplate>
            database_name:
            <asp:Label ID="database_nameLabel" runat="server" 
                Text='<%# Eval("database_name") %>' />
            <br />
            database_size:
            <asp:Label ID="database_sizeLabel" runat="server" 
                Text='<%# Eval("database_size") %>' />
            <br />
            unallocated space:
            <asp:Label ID="unallocated_spaceLabel" runat="server" 
                Text='<%# Eval("[unallocated space]") %>' />
            <br />
            <br />
        </ItemTemplate>
    </asp:DataList>
    <asp:SqlDataSource ID="SqlDataSourceLogTabeSize" runat="server" 
            SelectCommand="GetDBTableSize" SelectCommandType="StoredProcedure" 
            ConnectionString="<%$ ConnectionStrings:LoggingConnectionString %>">
    </asp:SqlDataSource>
        <asp:GridView ID="GridViewLogTabeSize" runat="server" 
            AllowSorting="True" CellPadding="3" 
            DataSourceID="SqlDataSourceLogTabeSize" GridLines="Horizontal" 
            BackColor="White" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px">
            <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
            <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
            <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
            <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
            <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
            <AlternatingRowStyle BackColor="#F7F7F7" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
