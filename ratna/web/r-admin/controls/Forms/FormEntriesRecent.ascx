<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormEntriesRecent.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.Forms.FormEntriesRecent" %>
<div class="box" runat="server" id="articleListDiv">
    <span id="headerspan" runat="server">
        <h2>
            <%= ResourceManager.GetLiteral("Admin.Forms.Entry.Recent")%></h2>
        <div class="utils">
            <a href="#" id="more" runat="server">
                <%= ResourceManager.GetLiteral("Admin.Common.More")%></a>
        </div>
    </span>
    <table>     
        <%
            if (this.Parameters.DisplayTableHeader)
            {
        %>   
        <thead>
            <tr id="headtr" runat="server">
                <th style="width: 20px;"><%= ResourceManager.GetLiteral("Admin.Common.Edit")%></th>
                <th><%= ResourceManager.GetLiteral("Admin.Forms.Form")%></th>
                <th>&nbsp;</th>
                <th style="width:80px;"><%= ResourceManager.GetLiteral("Admin.Common.UpdatedDate")%></th>
            </tr>
        </thead>
        <%
            } 
        %>
        <tbody>
            <asp:Repeater runat="server" ID="repeater" OnItemDataBound="RepeaterItemEventHandler">
                <ItemTemplate>
                    <tr>
                        <td style="width: 20px;">
                            <a id="editentryanchor" href=''
                                runat="server">
                                <img id="editpencil" title="<%$ Resource:Admin.Common.Edit%>" alt="<%$ Resource:Admin.Common.Edit%>" 
                                    src="/images/edit-pencil.png"
                                    class="imageButton16" runat="server" /></a>
                        </td>
                        <td><a id="formentriesanchor" href="" runat="server"><%# Eval("Form") %></a></td>
                        <td><%# Eval("DisplayValue") %></td>
                        <td style="width:80px;"><span id="updatedTime" runat="server"></span></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr id="none" runat="server" visible="false">
                <td colspan="2">
                    <%= ResourceManager.GetLiteral("Admin.Forms.NoResponses")%>
                </td>
            </tr>
        </tbody>
    </table>
</div>
