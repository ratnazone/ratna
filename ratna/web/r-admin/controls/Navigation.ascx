<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Navigation.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.Navigation" %>

<ul id="navigation">
    <asp:Repeater runat="server" ID="repeater" OnItemDataBound="RepeaterItemEventHandler">
        <ItemTemplate>            
            <li runat="server" id="navli">
                <a href='<%# Eval("Url") %>' runat="server" id="navlink"><asp:Literal runat="server" Text='<%# Eval("Label") %>' /></a>
            </li>
        </ItemTemplate>
    </asp:Repeater>
</ul>
