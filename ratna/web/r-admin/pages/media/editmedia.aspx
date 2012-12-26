<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" 
    CodeBehind="editmedia.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.media.editmedia"%>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>


<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor|Author"/>

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/fuploader/fuploader.jquery.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/form/form.jquery.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/validate/1.7/jquery.validate.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/media/media.editmedia.js")%>'></script>

    <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

</asp:Content>

<asp:Content ID="contentHolder" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    
    <div id="content" class="container_16 clearfix">
        
        <div class="grid_16">
            <h1 id="headerH1"><asp:Literal runat="server" id="headerLabel" /></h1>
        </div>

        <Common:SavingNotification id="savingNotification" runat="server" />

        <div id="mediadiv" class="grid_6">

            <img id="photoimage" src="~/images/empty.png" alt="photo" runat="server" class ="grid_5" style="margin-bottom:10px;" />

            <div id="mediaaction" class="grid_3">
                <a href="javascript:void" id="mediauploader" runat="server"><input type="button" id="mediauploaderButton" runat="server"/></a>
                <input runat="server" id="deletemedia" type="button" value="<%$ Resource:Admin.Common.Delete%>" />
            </div>

        </div>


        <form id="media_form" method="get" action="">
        <div id="mediadetailsdiv" class="grid_10">
            <div id="urlLabeldiv" class="grid_2">
                <label><%= ResourceManager.GetLiteral("Admin.Media.Edit.Photo.Url")%></label>
            </div>
            <div id="urlInputdiv" class="grid_7">
                <input id="urlInput" runat="server" class="grid_5 required absorrelurl" disabled="disabled" />
            </div>
            <div class="grid_9">&nbsp;</div>
            <div id="nameLabeldiv" class="grid_2">
                <label><%= ResourceManager.GetLiteral("Admin.Media.List.Name")%></label>
            </div>
            <div id="namediv" class="grid_7">
                <input type="text" class="grid_5 required" name="name" id="nameInput" runat="server"/>
            </div>
            <div class="grid_9">&nbsp;</div>
            <% if (View == Jardalu.Ratna.Core.Media.MediaType.Photo)
               { %>
            <div id="heightdiv" class="grid_2">
                <label><%= ResourceManager.GetLiteral("Admin.Media.Edit.Photo.Height")%></label>
            </div>
            <div id="heightspandiv" class="grid_7">
                <span id="heightspan" runat="server"></span>
            </div>
            <div class="grid_9">&nbsp;</div>
            <div id="widthdiv" class="grid_2">
                <label><%= ResourceManager.GetLiteral("Admin.Media.Edit.Photo.Width")%></label>
            </div>
            <div id="widthspandiv" class="grid_7">
                <span id="widthspan" runat="server"></span>
            </div>
            <div class="grid_9">&nbsp;</div>
            <% } %>
            <div id="tagsdiv" class="grid_2">
                <label><%= ResourceManager.GetLiteral("Admin.Media.Edit.Photo.Tags")%></label>
            </div>
            <div id="tagsspandiv" class="grid_7">
                <input id="tagsInput" class="grid_5" runat="server" />
            </div>
            <div class="grid_9">&nbsp;</div>
            <div class="grid_9">
                <input type="submit" value="save" id="savebutton" />
                <input type="reset" value="cancel" id="cancelbutton" />
            </div>
        </div>
        </form>

        <div id="nomediafounddiv" class="grid_10" runat="server">
            
        </div>


        <div id="photouploaderformdiv">
            <form method="post" enctype="multipart/form-data" id="theForm" action="<%=ResolveUrl(Constants.Urls.Service.UploadUrl) %>">
            <input type="hidden" name="uploadtype" id="uploadtype" runat="server" value="" />
        </form>
    </div>

    </div>

</asp:Content>
