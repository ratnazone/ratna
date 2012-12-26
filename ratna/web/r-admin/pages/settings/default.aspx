<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.pages.settings._default" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Configuration" Src="~/r-admin/controls/Settings/Configuration.ascx" %>
<%@ Register TagPrefix="Admin" TagName="SiteConfiguration" Src="~/r-admin/controls/Settings/SiteConfiguration.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator"/>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <title><%= ResourceManager.GetLiteral("Admin.Settings.Title")%></title>
</asp:Content>

<asp:Content ID="contentPlaceHolder" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

<div id="content" class="container_16 clearfix">

    <div class="grid_16">
        <h1><asp:Literal id="headerLiteral" runat="server" Text="<%$ Resource:Admin.Settings.Title %>" /></h1>
    </div>

    <Common:Notification id="notification" runat="server" />

    <div class="grid_3 alpha">
         <Admin:Menu id="menu" runat="server" />
    </div>

    <div class="grid_13 omega">

    <!-- configuration -->
    <Admin:Configuration id="configuration" runat="server"/>

    <!-- site configuration -->
    <Admin:SiteConfiguration id="siteconfiguration" runat="server"/>

    </div>

</div>

</asp:Content>
