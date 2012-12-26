<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="templates.aspx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.pages.settings.templates" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>
<%@ Register TagPrefix="Admin" TagName="TemplatesList" Src="~/r-admin/controls/Settings/Templates.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator" />

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
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

    <Common:Notification id="notification" runat="server" />

        <!-- template list -->
    <Admin:TemplatesList id="templatesList" runat="server"/>

    </div>

</div>

</asp:Content>
