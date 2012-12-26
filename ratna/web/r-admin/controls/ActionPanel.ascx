<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActionPanel.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.ActionPanel" %>
<div class="box">
    <h2 style="margin-bottom:5px;"><%= Title %></h2>
    <span>
        <asp:Repeater ID="repeater" runat="server">
            <ItemTemplate>
                <div  style="margin-bottom:4px;">
                    <a href='<%# Eval("HRef") %>' runat="server" visible='<%# Eval("Image") != null ? true : false %>'>
                        <img class="imageButton16 textmiddle" runat="server"
                            alt='<%# Eval("Title") %>' src='<%# Eval("Image")%>' /></a>
                    <a id="A1" href='<%# Eval("HRef") %>' runat="server"><%# Eval("Title") %></a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </span>
</div>
