<%@ Page MasterPageFile="~/r-admin/template/admin.Master" Language="C#" AutoEventWireup="true"
    Inherits="Jardalu.Ratna.Web.Admin._default" %>

<%@ MasterType  virtualPath="~/r-admin/template/admin.Master"%> 

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ArticlesList" Src="~/r-admin/controls/Articles/ArticlesList.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ProfileSummary" Src="~/r-admin/controls/ProfileSummary.ascx" %>
<%@ Register TagPrefix="Admin" TagName="MediaList" Src="~/r-admin/controls/Media/MediaList.ascx" %>
<%@ Register TagPrefix="Admin" TagName="UsersList" Src="~/r-admin/controls/Users/UsersList.ascx" %>
<%@ Register TagPrefix="Admin" TagName="CommentsList" Src="~/r-admin/controls/Comments/CommentsList.ascx" %>
<%@ Register TagPrefix="Admin" TagName="FormEntriesRecent" Src="~/r-admin/controls/Forms/FormEntriesRecent.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor"/>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <title>ratna - admin</title>
</asp:Content>
<asp:Content ID="contentPlaceHolder" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix">
		<div class="grid_5">

			<!-- profile summary -->
            <Admin:ProfileSummary id="profileSummary" runat="server" />

			<!-- media -->
            <Admin:MediaList id="mediaList" runat="server" />

            <!-- users -->
            <Admin:UsersList id="usersList" ConciseView="true" runat="server" visible="false"/>

		</div>
		<div class="grid_11">

			<!-- draft articles -->
            <Admin:ArticlesList id="draftArticlesList" Stage="Draft" runat="server" />

            <!-- draft pages -->
            <Admin:ArticlesList id="draftPagesList" Stage="Draft"
               runat="server" />

            <!-- pending comments -->
            <Admin:CommentsList id="pendingCommentsList" runat="server" />

            <!-- recent form entries -->
            <Admin:FormEntriesRecent id="formEntriesRecent" runat="server" />

		</div>
	</div>
</asp:Content>
