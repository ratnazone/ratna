<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" 
    CodeBehind="default.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.media._default" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="MediaList" Src="~/r-admin/controls/Media/MediaList.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ImageViewList" Src="~/r-admin/controls/Media/ImageViewList.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ActionPanel" Src="~/r-admin/controls/ActionPanel.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor|Author"/>

    <title><%= ResourceManager.GetLiteral("Admin.Media.PageTitle")%></title>
</asp:Content>

<asp:Content ID="mainContentHolder" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

<div id="content" class="container_16 clearfix">

    <div class="grid_3">
        <Admin:ActionPanel runat="server" id="actionPanel" />

        <Admin:ActionPanel runat="server" id="listPanel" Title="<%$ Resource:Admin.Common.List %>" />
    </div>

    <div class="grid_13">
    
      <!-- photos -->        
      <Admin:ImageViewList id="photosMediaList" runat="server" ShortMode=true />

      <!-- videos -->        
      <Admin:MediaList id="videosMediaList" runat="server" />

      <!-- documents -->        
      <Admin:MediaList id="documentsMediaList" runat="server" />

    </div>

</div>

</asp:Content>
