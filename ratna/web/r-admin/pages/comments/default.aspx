<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master"
    AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.comments._default" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="CommentsList" Src="~/r-admin/controls/Comments/CommentsList.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator"/>

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>

    <title>
        <%= ResourceManager.GetLiteral("Admin.Comments.Title")%></title>

</asp:Content>
<asp:Content ID="mainPlaceHolder" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix">
        <Admin:CommentsList ID="pendingComments" runat="server" />
        <Admin:CommentsList ID="approvedComments" runat="server" />
    </div>
</asp:Content>
