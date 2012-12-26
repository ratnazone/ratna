<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GalleryImages.ascx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.controls.gallery.GalleryImages" %>

<asp:Repeater runat="server" ID="repeater">
    <ItemTemplate>
        <img style='width:120px;height:120px;margin-top:10px;margin-right:10px;border:2px solid silver;' src='<%# Container.DataItem %>'/>
    </ItemTemplate>
</asp:Repeater>