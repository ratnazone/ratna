<%@ Page Language="C#" MasterPageFile="~/r-admin/template/admin.Master"
    AutoEventWireup="true" CodeBehind="editarticle.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.articles.editarticle" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor|Author" />
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/form/form.jquery.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/ckeditor/3.6.2/ckeditor.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/ckeditor/3.6.2/sample.js")%>'></script>

    <Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <Ratna:ConfirmDialog id="confirmDialog" runat="server" />

    <div id="content" class="container_16 clearfix" runat="server">  
        
        <div class="grid_16">
            <h1><asp:Literal id="headerLiteral" runat="server" Text="<%$ Resource:Admin.Articles.Edit %>" /></h1>
        </div>

        <div class="grid_3">
            <Admin:Menu id="menu" runat="server" />
        </div>

        <div class="grid_13">

        <div class="grid_13" style="margin-bottom:3px">
        <Common:Notification id="urlnotification" runat="server" />
        <p>
            <label for="articletitle">
                <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.Title")%>
                <small>
                    <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.Title.Help")%></small>
            </label>
            <input type="text" name="articletitle" class="grid_12 noleftmargin" id="articletitle" runat="server"/>
            <input type="hidden" runat="server" id="articleview" value="article" />
        </p>
        </div>

        <div class="grid_13">
            <p>
                <label for="urlkey">
                    <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.Url")%>
                    <small>
                        <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.Url.Help")%></small>
                </label>
                <input class="grid_11" style="margin-left:0px" type="text" name="urlkey" id="urlkey" runat="server" />               
                <input type="button" value="<%$ Resource:Admin.Articles.Edit.ValidateKey%>" id="validatebutton"
                    runat="server" />
            </p>
        </div>
        <div class="grid_13">
            <p>
                <textarea name="post" rows="20" cols="80" id="articlebody" runat="server"></textarea>
            </p>
        </div>        
        <div class="grid_13">
            <Common:SavingNotification id="savingNotification" runat="server" ClassName="grid_12"/>
            <p>
                <input type="submit" value="<%= ResourceManager.GetLiteral("Admin.Articles.Edit.Save")%>"
                    id="savebutton" />
                <input type="reset" value="<%= ResourceManager.GetLiteral("Admin.Articles.Edit.Cancel")%>"
                    id="cancelbutton" />
                <input type="button" value="<%= ResourceManager.GetLiteral("Admin.Articles.Edit.Publish")%>" 
                    id="publishbutton" style="float:right"/>
                <span style="clear:both" />
            </p>
        </div>        
        </div>

    </div>
    
    <div id="errorDiv" class="container_16 clearfix" runat="server">
        <div class="grid_16">
            <label>
                <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.NoArticle")%></label>
        </div>
    </div>
    <script type="text/javascript">
        CKEDITOR.replace('articlebody',
	    {
		    toolbar:
		    [
			    ['Source', 'Preview'],
                ['Maximize', 'ShowBlocks'],
                ['Format', 'Font', 'FontSize'],
                ['TextColor', 'BGColor'],
			    '/',
			    ['Bold', 'Italic', 'Underline', 'Subscript', 'Superscript', '-', 'RemoveFormat'],
                ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
                ['Link', 'Unlink', 'Anchor'],
                ['Image', 'Table', 'HorizontalRule']
		    ]
		});

		CKEDITOR.config.height = '260px';

		function Get_ArticleBodyContents() {

		    // Get the editor instance that you want to interact with.
		    var oEditor = CKEDITOR.instances.articlebody;

		    return oEditor.getData();
		}
    </script>
</asp:Content>
