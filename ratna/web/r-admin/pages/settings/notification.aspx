<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="notification.aspx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.pages.settings.notification" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />

    <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/settings/settings.notification.js")%>'></script>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

<div id="content" class="container_16 clearfix" runat="server"> 

    <div class="grid_16">
        <h1><asp:Literal id="headerLiteral" runat="server" Text="<%$ Resource:Admin.Templates.Title %>" /></h1>
    </div>

    <div class="grid_3 alpha">
        <Admin:Menu id="menu" runat="server" />
    </div>

     <div class="grid_13 omega">

            <Common:Notification id="cnotification" runat="server" />

            <div class="grid_13 bottommargin">
                <p>
                    <label for="smtpaddress">
                        <%=  ResourceManager.GetLiteral("Admin.Settings.Notification.NotifyTo")%>
                        <small>
                            <%=  ResourceManager.GetLiteral("Admin.Settings.Notification.NotifyTo.Help")%></small>
                    </label>
                    <input type="text" name="notifyto" id="notifyto" runat="server" class="grid_7 nomargin" />
                </p>
            </div>

            <div class="grid_13" style="margin-left:0px;margin-bottom:10px;">
                <div class="grid_4">
                    <label><%= ResourceManager.GetLiteral("Admin.Settings.Notification.Comment")%></label>
                </div>
                <div class="grid_3">
                    <select id="commentNotificationSelect" runat="server" />
                </div>
            </div>

            <div class="grid_13" style="margin-left:0px;margin-bottom:10px;">
                <div class="grid_4">
                    <label><%= ResourceManager.GetLiteral("Admin.Settings.Notification.FormsResponse")%></label>
                </div>
                <div class="grid_3">
                    <select id="formsResponseNotificationSelect" runat="server" />
                </div>
            </div>


            <div class="grid_13" style="margin-top:10px;">
                <input type="button" id="notificationSaveButton" runat="server" value="<%$Resource:Admin.Common.Save %>" />
                <input type="button" id="notificationCancelButton" runat="server" value="<%$Resource:Admin.Common.Cancel %>" />
            </div>

    </div>

</div>

</asp:Content>
