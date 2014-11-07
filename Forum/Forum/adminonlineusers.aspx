<%@ Page Title="" Language="C#" MasterPageFile="~/Content/ForumMaster.Master" AutoEventWireup="true" CodeBehind="adminonlineusers.aspx.cs" Inherits="ForumTP.adminonlineusers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">
    <table cellpadding="7">
	<tr>
		<th><%= ForumTP.Resources.various.OnlineUsers %></th>
		<th>Current URL</th>
		<th>Last seen</th>
	</tr>
<asp:Repeater ID="rptUsers" runat="server">
<ItemTemplate>
    <tr>
    <td><%# Eval("UserName")%></td>
    <td><%# Eval("CurrentURL") %></td>
    <td><%# Eval("LastActivity")%></td>
    </tr>
</ItemTemplate>
</asp:Repeater>
</table>
</asp:Content>
