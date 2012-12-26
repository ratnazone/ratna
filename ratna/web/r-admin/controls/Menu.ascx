<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.Menu" %>

<style type="text/css">
#menu
{
    font-weight:bold;    
	margin:0 auto 0px auto;
	padding:0px;
	background:#434A48;
	color:#fff;
}
    
#menu > li {
	display:block;
	margin:0;
	width:auto;
	border:1px solid #ffffff;
	padding:2px;
}

#menu > li > a {
	display:inline-block;
	text-decoration:none;
	width:90%;
	color:White;
	margin-left:8px;
}
    
#menu a:hover {
	background:#383E3C;
}

#menu .active {
	background:#687370;
}

</style>

<ul id="menu">
    <asp:Repeater ID="repeater" runat="server" OnItemDataBound="RepeaterItemEventHandler">
        <ItemTemplate>
            <li>
                <a class="menuanchor" id='anchor' href='<%# Eval("HRef") %>' runat="server"><%# Eval("Title") %></a>
            </li>
        </ItemTemplate>
    </asp:Repeater>
</ul>
