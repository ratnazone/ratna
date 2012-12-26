<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.pages.apps._default" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ActionPanel" Src="~/r-admin/controls/ActionPanel.ascx" %>
<%@ Register TagPrefix="Admin" TagName="AppsList" Src="~/r-admin/controls/Apps/AppsList.ascx" %>
<%@ Register TagPrefix="Admin" TagName="UploadApp" Src="~/r-admin/controls/Apps/UploadApp.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Common" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
<Ratna:PageAccess ID="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />
    <title>
        <%= ResourceManager.GetLiteral("Admin.Apps.Title")%></title>

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/form/form.jquery.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/fuploader/fuploader.jquery.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/apps/apps.upload.js")%>'></script>

    <Common:ClientJavaScript id="clientJavaScript" runat="server" />
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix">

        <Common:SavingNotification id="savingNotification" runat="server" />
        <Common:Notification id="notification" runat="server" />

        <div class="grid_3">
            <Admin:ActionPanel runat="server" id="listPanel" Title="<%$ Resource:Admin.Common.List %>" />
        </div>
        <div class="grid_13">

            <!-- upload app -->
            <Admin:UploadApp id="uploadApp" runat="server" />

            <div class="grid_13 clear">&nbsp;</div>

            <!-- enabled apps -->
            <Admin:AppsList id="enabledAppsList" runat="server" />

            <!-- installed but not enabled apps -->
            <Admin:AppsList id="installedAppsList" runat="server" />

        </div>
    </div>
</asp:Content>
