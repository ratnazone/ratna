<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormsList.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.Forms.FormsList" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
<Ratna:ConfirmDialog id="confirmDialog" runat="server" />

<div class="box" id="formsListDiv">

    <span id="headerspan">
        <h2> <%= ResourceManager.GetLiteral("Admin.Forms.PageTitle")%></h2>
    </span>

    <table>
        <%
            if (this.Parameters.DisplayTableHeader)
            {
        %>
        <thead>
            <tr>
                <th><%= ResourceManager.GetLiteral("Admin.Forms.List.Edit")%></th>
                <th><%= ResourceManager.GetLiteral("Admin.Forms.List.Responses")%></th>
                <th><%= ResourceManager.GetLiteral("Admin.Forms.DisplayName")%></th>
                <th><%= ResourceManager.GetLiteral("Admin.Forms.Name")%></th>
                <th><%= ResourceManager.GetLiteral("Admin.Forms.List.Delete")%></th>
            </tr>
        </thead>
        <% } %>
        <tbody>
            <asp:Repeater runat="server" id="repeater">
                <ItemTemplate>
                    <tr>
                        <td style="width: 20px;">
                            <a id="editformanchor" href='<%# Constants.Urls.Forms.EditUrlWithKey + Eval("Name") %>'
                               runat="server"><img id="editimage" title="<%$ Resource:Admin.Forms.List.Edit%>" alt="<%$ Resource:Admin.Forms.List.Edit%>" 
                               src="/images/edit-pencil.png" class="imageButton16" runat="server" /></a>
                        </td>
                        <td style="width: 20px;">
                            <a id="viewresponsesanchor" href='<%# Constants.Urls.Forms.ResponsesUrl + Eval("Name") %>'
                               runat="server"><img id="listimage" title="<%$ Resource:Admin.Forms.List.Responses%>" alt="<%$ Resource:Admin.Forms.List.Responses%>" 
                               src="/images/list.png" class="imageButton16" runat="server" /></a>
                        </td>
                        <td>
                            <%# Eval("DisplayName") %>
                            <input type="hidden" name="formname" id="formname" value='<%# Eval("Name") %>' />
                        </td>
                         <td>
                            <%# Eval("Name") %>
                        </td>
                        <td id="deleteTD" runat="server" visible="<%# Parameters.ShowDelete %>" style="width: 20px;">
                            <input type="hidden" id="deletearticleid" value="<%# Eval("Name") %>" />
                            <img alt="delete" title="<%$ Resource:Admin.Forms.List.Delete%>" class="imageButton16 deleteFormImageButton"
                                id="deleteForForms" src="/images/delete.png" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr id="none" runat="server" visible="false">
                <td>
                    <%= ResourceManager.GetLiteral("Admin.Forms.NoForms")%>
                </td>
            </tr>
        </tbody>
    </table>

</div>