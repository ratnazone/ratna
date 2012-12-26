<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" 
    CodeBehind="default.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.media.gallery._default" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ActionPanel" Src="~/r-admin/controls/ActionPanel.ascx" %>
<%@ Register TagPrefix="Admin" TagName="GalleryList" Src="~/r-admin/controls/Gallery/GalleryList.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor|Author"/>
    <title><%= ResourceManager.GetLiteral("Admin.Media.Gallery")%></title>

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/media/control.gallerylist.js")%>'></script>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    
<div id="content" class="container_16 clearfix">

    <div class="grid_3">
        <Admin:ActionPanel runat="server" id="actionPanel" />
    </div>

    <div class="grid_13">

        <!-- common notification -->
        <Common:Notification id="commonnotification" runat="server" Class="container_13"/>
      
          <!-- galleries -->        
          <Admin:GalleryList id="galleryList" runat="server" />

    </div>

</div>

</asp:Content>
