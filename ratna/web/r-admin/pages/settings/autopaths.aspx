<%@ Page Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="autopaths.aspx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.pages.settings.autopaths" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Admin" TagName="AutoPaths" Src="~/r-admin/controls/Settings/autopathscontrol.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />

    <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/settings/settings.customerrors.js")%>'></script>
    <title><%=  ResourceManager.GetLiteral("Admin.Settings.AutoPaths.Title")%></title>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
<div id="content" class="container_16 clearfix" runat="server"> 
    <div class="grid_16">
        <h1><asp:Literal id="headerLiteral" runat="server" Text="<%$ Resource:Admin.Settings.AutoPaths.Title %>" /></h1>
    </div>

    <div class="grid_3 alpha">
        <Admin:Menu id="menu" runat="server" />
    </div>

    <div class="grid_13 omega">
        <Common:Notification id="notification" runat="server" />

         <!-- auto paths list -->
        <Admin:AutoPaths id="autoPathsList" runat="server"/>
    </div>
</div>
</asp:Content>
