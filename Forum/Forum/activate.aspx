<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="activate.aspx.cs" Inherits="ForumTP.activate" MasterPageFile="~/Content/ForumMaster.Master" %>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">

	<div class="location">
		<strong><a href="default.aspx">Home</a> &raquo; Activation </strong>
	</div>
	<asp:Label ID="lblSuccess" runat="server" Visible="false"><%= ForumTP.Resources.various.ActivationSuccess %></asp:Label>
	<asp:Label ID="lblError" runat="server" Visible="false"><%= ForumTP.Resources.various.ActivationError %></asp:Label>
</asp:Content>
