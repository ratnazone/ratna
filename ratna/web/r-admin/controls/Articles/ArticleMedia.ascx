<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArticleMedia.ascx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.controls.Articles.ArticleMedia" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
<Ratna:ConfirmDialog id="confirmDialog" runat="server" />


<div class="box" runat="server" id="mediaListDiv">
    <span id="headerspan" runat="server">
        <label style="padding-left:18px"><asp:Literal id="header" runat="server"></asp:Literal> 
        <small><asp:Literal id="subheader" runat="server"></asp:Literal></small>
         <% if (ShowControls){ %> 
         <input type="checkbox" id="selectallcheckbox" style="float:right;max-width:25px;max-height:20px;"/><%} %></label>
    </span>

    <table>
        <tbody>
            <tr>
                <td>
                    <asp:Repeater ID="imageRepeater" runat="server">
                        <ItemTemplate>
                            <div style="display:inline-block">
                            <img id="image" class="frame-small" src="<%# Container.DataItem %>" />
                             <% if (ShowControls){ %><input type="checkbox" class="addimagecheckbox" style="width:20px" /><%} %>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <span id="photomarker"></span>
                    <!-- tag to insert image -->
                    <div id="addimagediv" style="display:none">
                        <img id="addimage" src="" class="frame-small"/>
                        <% if (ShowControls){ %><input type="checkbox" class="addimagecheckbox" style="width:20px" /><%} %>
                    </div>
                </td>
            </tr>
            <% if (ShowControls){ %>
            <tr>
            <td>
            <p>
                <input runat="server" type="button" id="photouploader" value="<%$ Resource:Admin.Common.Upload%>" title="<%$ Resource:Admin.Articles.Edit.Images.AddImageTitle%>"/> 
                <input runat="server" type="button" id="photoselected" value="<%$ Resource:Admin.Common.Select%>" title="<%$ Resource:Admin.Articles.Edit.Images.SelectImageTitle%>"/> 
                <input runat="server" type="button" id="deletemediaphotobutton" value="<%$ Resource:Admin.Common.Remove%>" style="float:right;"/> 
            </p>
            </td>
            </tr>
            <%} %>
        </tbody>
    </table>

    <% if (ShowControls){ %>
    <div id="photouploaderformdiv">
        <form method="post" enctype="multipart/form-data" id="theForm" action="<%=ResolveUrl(Constants.Urls.Service.UploadUrl) %>">
        <input type="hidden" name="uploadtype" value="<%= Jardalu.Ratna.Web.Upload.UploadType.BlogPhoto.ToString() %>" />
        <input type="hidden" name="urlkey" id="urlkey" value="<%= UrlKey %>" />
        </form>
    </div>
    <%} %>

</div>
