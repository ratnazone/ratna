<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" 
Inherits="Jardalu.Ratna.Web.Admin.posts._default" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ArticlesList" Src="~/r-admin/controls/Articles/ArticlesList.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ActionPanel" Src="~/r-admin/controls/ActionPanel.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor|Author"/>
    <!-- title set -->

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>

</asp:Content>
<asp:Content ID="contentPlaceHolder" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

    <div id="content" class="container_16 clearfix">

        <!-- common notification -->
        <Common:Notification id="commonnotification" runat="server" />

        <div class="grid_3">
            <Admin:ActionPanel runat="server" id="actionPanel" />
        </div>

        <div class="grid_13">
       
        <!-- draft articles -->        
        <Admin:ArticlesList id="draftArticlesList" Stage="Draft" runat="server" />

     
        <!-- published articles -->   
        <Admin:ArticlesList id="publishedArticlesList" Stage="Published" runat="server" />

        </div>

    </div>

</asp:Content>
