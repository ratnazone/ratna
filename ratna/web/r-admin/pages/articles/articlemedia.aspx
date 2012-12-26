<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="articlemedia.aspx.cs" 
Inherits="Jardalu.Ratna.Web.Admin.pages.articles.articlemedia" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ArticlesMedia" Src="~/r-admin/controls/Articles/ArticleMedia.ascx" %>


<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
     <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor|Author" />
     <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
     <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
     <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
     <script type="text/javascript" src='<%=ResolveUrl("~/external/form/form.jquery.js")%>'></script>
     <script type="text/javascript" src='<%=ResolveUrl("~/external/fuploader/fuploader.jquery.js")%>'></script>

    <script type="text/javascript" src='<%=ResolveUrl("~/external/imagepicker/jquery.imagepicker.js")%>'></script>
    <link rel="Stylesheet" type="text/css" href="/external/imagepicker/imagepicker.css" />
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix" runat="server"> 
        <div class="grid_16">
            <h1><asp:Literal id="headerLiteral" runat="server" Text="<%$ Resource:Admin.Articles.Media %>" /></h1>
        </div>


        <div class="grid_3">
            <Admin:Menu id="menu" runat="server" />
        </div>

        
        <!-- gallery media -->
        <div class="grid_13">

            <Common:Notification id="commonnotification" runat="server" ClassName="grid_12"/>
            <Common:SavingNotification id="savingNotification" runat="server" ClassName="grid_12"/>

            <Admin:ArticlesMedia id="articleGallery" runat="server" />
            
            <Admin:ArticlesMedia id="articleReferencedImages" runat="server" />
        </div>

        <!-- referenced media -->

    </div>
    <div id="errorDiv" class="container_16 clearfix" runat="server">
        <label>
            <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.NoArticle")%></label>
    </div>
</asp:Content>
