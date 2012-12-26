<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master"
    AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.users._default" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="UsersList" Src="~/r-admin/controls/Users/UsersList.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ActionPanel" Src="~/r-admin/controls/ActionPanel.ascx" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess ID="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />
    <title>
        <%= ResourceManager.GetLiteral("Admin.Users.Title")%></title>
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix">
        <div class="grid_3">
            <Admin:ActionPanel runat="server" ID="actionPanel" />
        </div>
        <div class="grid_13">
            <!-- active users -->
            <Admin:UsersList ID="activeUsersList" runat="server" Title="<%$ Resource:Admin.Users.Active %>" />
            <!-- pending users -->
            <Admin:UsersList ID="pendingUsersList" runat="server" Title="<%$ Resource:Admin.Users.Pending %>"
                IsActive="false" />
            <!-- deleted users -->
            <Admin:UsersList ID="deletedUsersList" runat="server" Title="<%$ Resource:Admin.Users.Deleted %>"
                IsDeleted="true" />
        </div>
    </div>
</asp:Content>
