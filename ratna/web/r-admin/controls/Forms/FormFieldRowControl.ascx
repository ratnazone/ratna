<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormFieldRowControl.ascx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.controls.Forms.FormFieldRowControl" %>

<asp:Repeater runat="server" id="repeater" OnItemDataBound="RepeaterItemEventHandler">
    <ItemTemplate>
        <tr>
            <td><%# Eval("Name") %>
            <input type="hidden" name="fieldname" id="fieldname" value='<%# Eval("Name") %>' />
            </td>
            <td><%# Eval("FieldType") %></td>
            <td><input type="checkbox" runat="server" id="required" name="required" /></td>
            <td id="deletefieldTd" runat="server">                         
                <img 
                    alt="<%$ Resource:Admin.Common.Delete%>" 
                    title="<%$ Resource:Admin.Common.Delete%>" 
                    class="imageButton16 deleteFieldImageButton"
                    id="deleteForField" 
                    src="/images/delete.png" 
                    name="deleteForField"
                    runat="server" />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
