<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormEntries.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.Forms.FormEntries" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
<Ratna:ConfirmDialog id="confirmDialog" runat="server" />

<div class="box" runat="server" id="articleListDiv">
    <span id="headerspan" runat="server">
        <h2>
            <asp:Literal runat="server" ID="header"></asp:Literal></h2>
    </span>
    <table>
        <thead>
            <tr id="headtr" runat="server">
                <th class="checkboxtd" runat="server" id="checkboxth"><input type="checkbox" name="selectall" id="selectall" class="checkboxinput" /></th>
                <th style="width: 20px;"><%= ResourceManager.GetLiteral("Admin.Common.Edit")%></th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater runat="server" ID="repeater" OnItemDataBound="RepeaterItemEventHandler">
                <ItemTemplate>
                    <tr runat="server" id="responsetr">
                        <td class="checkboxtd">
                            <input type="checkbox" name="deletecheckbox" id="deletecheckbox" class="checkboxinput" />
                            <input type="hidden" name="uid" id="uid" value='<%# Eval("UId") %>' />
                        </td>
                        <td style="width: 20px;">
                            <a id="editentryanchor" href=''
                                runat="server">
                                <img id="editpencil" title="<%$ Resource:Admin.Common.Edit%>" alt="<%$ Resource:Admin.Common.Edit%>" 
                                    src="/images/edit-pencil.png"
                                    class="imageButton16" runat="server" /></a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr id="controlsTR" runat="server">
                <td colspan="5">
                    <input type="hidden" id="responseform" name="responseform" value="0" runat="server" />
                    <input type="button" id="deleteResponsesButton" value='<%= ResourceManager.GetLiteral("Admin.Common.Delete")%>' />
                </td>
            </tr>
            <tr id="none" runat="server" visible="false">
                <td colspan="2">
                    <%= ResourceManager.GetLiteral("Admin.Forms.NoResponses")%>
                </td>
            </tr>
        </tbody>
    </table>
</div>
