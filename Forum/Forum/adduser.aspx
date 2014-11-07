<%@ Page Title="" Language="C#" MasterPageFile="~/Content/ForumMaster.Master" AutoEventWireup="true" CodeBehind="adduser.aspx.cs" Inherits="ForumTP.adduser" %>
<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
	
	<p><%= ForumTP.Resources.various.ManuallyAddingUserDescription %></p>

	<table class="roundedborder noborder" cellpadding="11">
		<tr><th colspan="2"><%= ForumTP.Resources.various.NewUser %></th></tr>
		<tr>
			<td class="gray">
				<%= ForumTP.Resources.various.Username %> *
			</td>
			<td>
				<asp:TextBox ID="txUserName" runat="server"></asp:TextBox>
				<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txUserName" Display="Dynamic" ErrorMessage="***"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td class="gray">
				<%= ForumTP.Resources.various.Password %> *
			</td>
			<td>
				<asp:TextBox ID="txPsw" runat="server"></asp:TextBox>
				<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txPsw" Display="Dynamic" ErrorMessage="***"></asp:RequiredFieldValidator></td>
		</tr>
		<tr>
			<td class="gray">
				<%= ForumTP.Resources.various.Email %> *
			</td>
			<td>
				<asp:TextBox ID="txEmail" runat="server"></asp:TextBox>
				<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txEmail" Display="Dynamic" ErrorMessage="***"></asp:RequiredFieldValidator></td>
		</tr>
		<tr>
			<td class="gray">
				<%= ForumTP.Resources.various.Homepage %>
			</td>
			<td>
				<asp:TextBox ID="txHomepage" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr><td colspan="2"><asp:Button CssClass="gradientbutton" ID="btnAdd" runat="server" Text="<%# ForumTP.Resources.various.Add %>" OnClick="btnAdd_Click" /></td></tr>
	</table>
	<asp:label id="lblError" runat="server" Visible="False" ForeColor="Red"><%= ForumTP.Resources.various.UserAlreadyExists %></asp:label>
	<asp:label id="lblSuccess" runat="server" Visible="False"><%= ForumTP.Resources.various.UserSuccessfullyCreated %></asp:label>
</asp:Content>

