<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="recent.aspx.cs" 
Inherits="Jardalu.Ratna.Web.Admin.pages.forms.recent" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="SimplePager" Src="~/Controls/Pagers/SimplePager.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ActionPanel" Src="~/r-admin/controls/ActionPanel.ascx" %>
<%@ Register TagPrefix="Admin" TagName="FormEntriesRecent" Src="~/r-admin/controls/Forms/FormEntriesRecent.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator"/>
    <title><%= ResourceManager.GetLiteral("Admin.Forms.Entry.Recent") %></title>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

    <div id="content" class="container_16 clearfix">

    <h1 id="headerH1" class="grid_16"><%= ResourceManager.GetLiteral("Admin.Forms.Entry.Recent") %></h1>

     <div class="grid_3">
        <Admin:ActionPanel runat="server" id="listPanel" Title="<%$ Resource:Admin.Common.List %>" />
    </div>

    <div class="grid_13">

        <!-- recent form entries -->
        <Admin:FormEntriesRecent id="formEntriesRecent" runat="server" />

        <!-- pagination goes here -->
        <Ratna:SimplePager id="Pager" runat="server" numberofpages="10" pagesize="10" />

    </div>

    </div>

</asp:Content>
