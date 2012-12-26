<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="responses.aspx.cs" 
Inherits="Jardalu.Ratna.Web.Admin.pages.forms.responses" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="FromEntriesList" Src="~/r-admin/controls/Forms/FormEntries.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="SimplePager" Src="~/Controls/Pagers/SimplePager.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ActionPanel" Src="~/r-admin/controls/ActionPanel.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator"/>

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>

</asp:Content>

<asp:Content id="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

    <div id="content" class="container_16 clearfix">

    <!-- notification -->
    <Common:Notification id="commonnotification" runat="server" Class="container_16"/>

     <div class="grid_3">
        <Admin:ActionPanel runat="server" id="actionPanel" />
    </div>

    <div class="grid_13">

        <!-- entries -->
        <Admin:FromEntriesList id="entriesList" runat="server" />

        <!-- pagination goes here -->
        <Ratna:SimplePager id="Pager" runat="server" numberofpages="10" pagesize="10" />

    </div>

    </div>

</asp:Content>
