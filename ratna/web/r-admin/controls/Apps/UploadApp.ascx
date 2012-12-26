<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadApp.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.Apps.UploadApp" %>

<div class="box">
    <span id="headerspan" runat="server">
        <h2><%= ResourceManager.GetLiteral("Admin.Apps.UploadApp")%></h2>
    </span>

   <div style="padding:10px">
        <form id="appupload_form" method="get" action="javascript:return false;">
            <a href="javascript:void" id="appselector">
            <%= ResourceManager.GetLiteral("Admin.Common.Select")%></a>

            <label id="selectedApp" style="display:inline"></label>
            <input type="button" value="<%= ResourceManager.GetLiteral("Admin.Common.Upload")%>" id="uploadAppButton" style="display:inline"/>
        </form>
    </div>

    <div id="appuploaderformdiv">
        <form method="post" enctype="multipart/form-data" id="theForm" action="<%=ResolveUrl(Constants.Urls.Service.UploadUrl) %>">
        <input type="hidden" name="uploadtype" value="<%= Jardalu.Ratna.Web.Upload.UploadType.App.ToString() %>" />
        </form>
    </div>
</div>
