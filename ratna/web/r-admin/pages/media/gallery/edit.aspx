<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.media.gallery.edit" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Admin" TagName="GalleryImages" Src="~/r-admin/controls/Gallery/GalleryImages.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor|Author"/>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/validate/1.7/jquery.validate.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/media/gallery.edit.js")%>'></script>    
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
    
    <script type="text/javascript" src='<%=ResolveUrl("~/external/imagepicker/jquery.imagepicker.js")%>'></script>
    <link rel="Stylesheet" type="text/css" href="/external/imagepicker/imagepicker.css" />

    <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
     <div id="content" class="container_16 clearfix">
        <div class="grid_16">
            <h1 id="headerH1"><asp:Literal runat="server" id="headerLabel" /></h1>
        </div>

        <Common:SavingNotification id="savingNotification" runat="server" />

        <form id="editform" method="get" action="javascript:return false;">
        <div class="grid_3">
            <%= ResourceManager.GetLiteral("Admin.Common.Url")%></div>
        <div class="grid_7">
            <input type="text" class="required relurl" style="width:400px;" name="urlInput" id="urlInput" runat="server" />
        </div>
        <div class="grid_16">
            &nbsp;</div>
        <div class="grid_3">
            <%= ResourceManager.GetLiteral("Admin.Common.Name")%></div>
        <div class="grid_4">
            <input type="text" class="required" name="name" id="name" runat="server" style="width:300px;"/>
            <input type="hidden" name="uid" id="uid" runat="server" />
        </div>
        <div class="grid_16">
            &nbsp;</div>
        <div class="grid_3">
            <%= ResourceManager.GetLiteral("Admin.Media.Gallery.Navigation")%></div>
        <div class="grid_7">
            <input type="text" style="width:300px;" name="nav" id="nav" runat="server" />
        </div>
        <div class="grid_16">
            &nbsp;</div>
        <div class="grid_3">
            <%= ResourceManager.GetLiteral("Admin.Common.Description")%></div>
        <div class="grid_7">
            <textarea style="width:600px;" rows="2" id="description" runat="server"></textarea>
        </div>
        <div class="grid_16">
            &nbsp;</div>
        <div class="grid_16" style="text-align:right">
            <p>
                <input type="submit" value="<%= ResourceManager.GetLiteral("Admin.Common.Save")%>"
                    id="savebutton" />
                <input type="reset" value="<%= ResourceManager.GetLiteral("Admin.Common.Cancel")%>"
                    id="cancelbutton" />
            </p>
        </div>
        </form>

        <div class="grid_16">
            <h2>
                <%= ResourceManager.GetLiteral("Admin.Media.List.PhotoHeader")%></h2>
            <div>
                <input type="button" value="Select" id="gallerySelectButton" />
            </div>
            <div>
                <Admin:GalleryImages id="galleryImages" runat="server" />
                <span id="galleryAddedImagesMarker"></span>
            </div>
        </div>
     </div>
</asp:Content>
