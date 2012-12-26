<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" 
    CodeBehind="list.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.media.list" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Admin" TagName="MediaList" Src="~/r-admin/controls/Media/MediaList.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ImagesList" Src="~/r-admin/controls/Media/ImageViewList.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="SimplePager" Src="~/Controls/Pagers/SimplePager.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ActionPanel" Src="~/r-admin/controls/ActionPanel.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor|Author"/>

     <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $("#searchbutton").click(function () {
                OnSearchClick();
            });
        });

        function OnSearchClick() {
            var query = $("#searchquery").val();
            window.location = "<%=SearchUrl%>".formatEscape(query);
        }
        
    </script>

</asp:Content>

<asp:Content ID="contentHolder" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

    <div id="content" class="container_16 clearfix">

        <h1 id="headerH1" runat="server" class="grid_16"></h1>

        <div class="grid_3">
            <Admin:ActionPanel runat="server" id="actionPanel"/>

            <Admin:ActionPanel runat="server" id="listPanel" Title="<%$ Resource:Admin.Common.List %>" />
        </div>

        <div class="grid_13">
   
        <!-- search option -->
        <div id="searchdiv" style="margin-bottom:10px">
            <input type="text" id="searchquery" style="width:160px" />
            <input type="button" value="<%$ Resource:Admin.Common.Search %>" id="searchbutton" runat="server"/>
        </div>

        <!-- media -->        
        <Admin:MediaList id="mediaList" runat="server" />

        <!-- for images -->
        <Admin:ImagesList id="imagesList" runat="server" />

        <!-- pagination goes here -->
        <Ratna:SimplePager id="Pager" runat="server" numberofpages="10" pagesize="20" />

        </div>

    </div>

</asp:Content>
