<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master" AutoEventWireup="true" 
    CodeBehind="edittemplate.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.pages.settings.edittemplate" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Common" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator"/>

    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/form/form.jquery.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/validate/1.7/jquery.validate.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/fuploader/fuploader.jquery.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/settings/templates.edittemplate.js")%>'></script>

    <Common:ClientJavaScript id="clientJavaScript" runat="server" />

</asp:Content>

<asp:Content ID="contentPlaceHolder" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

<div id="content" class="container_16 clearfix" runat="server">

    <div class="grid_16">
        <h1><asp:Literal id="headerLiteral" runat="server" /></h1>
    </div>
   
    <Common:SavingNotification id="savingNotification" runat="server" />
    <Common:Notification id="notification" runat="server" />

    <div class="grid_3 alpha">
         <Admin:Menu id="menu" runat="server" />
    </div>

    <div class="grid_13 omega">

   <form id="template_form" method="get" action="" runat="server">
   <input type="hidden" name="uid" id="uid" value="" runat="server" />
   
   <div class="grid_3"><%= ResourceManager.GetLiteral("Admin.Templates.Edit.Name")%></div>
   <div class="grid_4">
        <input type="text" class="required" name="name" id="name" runat="server" />
   </div>   
   <div class="grid_16">&nbsp;</div>

   <div class="grid_3"><%= ResourceManager.GetLiteral("Admin.Templates.Edit.UrlPath")%></div>
    <div class="grid_4">
        <input type="text" class="required" name="urlPath" id="urlPath" runat="server" />
    </div>
   <div class="grid_16">&nbsp;</div>

   <div class="grid_3"><%= ResourceManager.GetLiteral("Admin.Templates.Edit.TemplatePath")%></div>
   <div class="grid_4">
        <input type="text" class="required" name="templatePath" id="templatePath" runat="server" />
    </div>
   <div class="grid_16">&nbsp;</div>
   
   <div class="grid_3"><%= ResourceManager.GetLiteral("Admin.Templates.Edit.MasterFile")%></div>
     <div class="grid_4">
          <input type="text" class="required" name="masterFile" id="masterFile" runat="server" />
     </div>
   <div class="grid_16">&nbsp;</div>
      
   <div class="grid_3"><%= ResourceManager.GetLiteral("Admin.Templates.Edit.Active")%></div>
   <div class="grid_5">
        <p>
          <input type="radio" class="grid_1" name="active" id="activatedTemplate" runat="server"/><span class="grid_1"><%= Boolean.TrueString %></span>
          <input type="radio" class="grid_1" name="active" id="deactivatedTemplate" runat="server"/><span class="grid_1"><%= Boolean.FalseString %></span>
        </p>
     </div>
   <div class="grid_16">&nbsp;</div>
                  

   <div class="grid_16">
        <p>
            <input type="submit" value="<%= ResourceManager.GetLiteral("Admin.Templates.Edit.Save")%>"
                id="savebutton" />
            <input type="reset" value="<%= ResourceManager.GetLiteral("Admin.Templates.Edit.Cancel")%>"
                id="cancelbutton" />
        </p>
    </div>

    </form>
   
   <form id="template_upload_form" method="get" action="javascript:return false;" runat="server">
   
       <div class="grid_3"><%= ResourceManager.GetLiteral("Admin.Templates.Edit.Name")%></div>
       <div class="grid_4">
            <input type="text" class="required" name="name" id="templatename" runat="server" />
       </div>
       <div class="grid_16">&nbsp;</div>
       <div class="grid_3"><%= ResourceManager.GetLiteral("Admin.Templates.Edit.UrlPath")%></div>
       <div class="grid_4">
            <input type="text" class="required" name="urlpath" id="templateurlpath" runat="server" value="/" />
       </div>
       <div class="grid_16">&nbsp;</div>
       <div class="grid_3">
            <%= ResourceManager.GetLiteral("Admin.Templates.Upload.Select")%>
            <a href="javascript:void" id="templateselector">
            <%= ResourceManager.GetLiteral("Admin.Common.Select")%></a>
       </div>
       <div class="grid_4">
            <label id="selectedTemplate"></label>
       </div>
        <div class="grid_16">&nbsp;</div>
        <div class="grid_3"><input type="button" value="<%= ResourceManager.GetLiteral("Admin.Common.Upload")%>" id="uploadTemplateButton"/>
        <input type="button" value="<%= ResourceManager.GetLiteral("Admin.Common.Cancel")%>" id="uploadCancelButton"/></div>
   </form>

    <div id="templateuploaderformdiv">
        <form method="post" enctype="multipart/form-data" id="theForm" action="<%=ResolveUrl(Constants.Urls.Service.UploadUrl) %>">
        <input type="hidden" name="uploadtype" value="<%= Jardalu.Ratna.Web.Upload.UploadType.Template.ToString() %>" />
        <input type="hidden" id="uploadtemplatename" name="uploadtemplatename" value="" />
        <input type="hidden" id="uploadtemplateurlPath" name="uploadtemplateurlPath" value="" />
        </form>
    </div>

    </div>

</div>

<div id="errorDiv" class="container_16 clearfix" runat="server">
   <label><%=  ResourceManager.GetLiteral("Admin.Templates.Edit.NoTemplatesFound")%></label>
</div>

</asp:Content>
