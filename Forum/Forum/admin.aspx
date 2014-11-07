<%@ Page Title="" Language="C#" MasterPageFile="~/Content/ForumMaster.Master" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="ForumTP.admin" %>
<%@ Register TagPrefix="cc" Namespace="ForumTP" Assembly="ForumTP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">

<p>
	<cc:THDataGrid id="gridForums" runat="server" AutoGenerateColumns="False" Width="100%" EnableViewState="False"
		ShowHeader="true" UseAccessibleHeader="true" OnItemCommand="gridForums_ItemCommand" OnItemDataBound="gridForums_ItemDataBound" CellPadding="11"
		CssClass="roundedborder horizseparated" GridLines="None" CellSpacing="-1">
		<AlternatingItemStyle CssClass="altItem" />
		<Columns>
			<asp:BoundColumn Visible="False" DataField="ForumID" HeaderText="ForumID"></asp:BoundColumn>
			<asp:HyperLinkColumn DataTextField="Title" DataNavigateUrlField="ForumID" DataNavigateUrlFormatString="editforum.aspx?ForumID={0}" HeaderText="Forums" ItemStyle-Width="100%"></asp:HyperLinkColumn>
			<asp:ButtonColumn Text="<img src='images/up.png' alt='move up' title='move up'/>" CommandName="up"></asp:ButtonColumn>
			<asp:ButtonColumn Text="<img src='images/down.png' alt='move down' title='move down'/>" CommandName="down"></asp:ButtonColumn>
			<asp:TemplateColumn>
				<ItemTemplate><asp:Button runat="server" CommandName="delete" Text="<%# ForumTP.Resources.various.Delete %>" /></ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</cc:THDataGrid>
	<asp:Label ID="lblNoForums" Runat="server" Visible="False"><%= ForumTP.Resources.various.NoForumsFound %></asp:Label>
</p>
<br />
<p>
	<table class="roundedborder" cellpadding="7">
		<tr><th colspan="2"><%= ForumTP.Resources.various.AddNewForum %></th></tr>
		<tbody>
		<tr>
			<td align="right" class="gray" nowrap="nowrap"><%= ForumTP.Resources.various.Name %>:</asp:Label></td>
			<td style="width:100%;">
				<asp:TextBox id="tbTitle" runat="server" Width="100%" MaxLength="50"></asp:TextBox></td>
		</tr>
		<tr>
			<td align="right" class="gray" nowrap="nowrap"><%= ForumTP.Resources.various.Description %>:</asp:Label></td>
			<td>
				<asp:TextBox id="tbDescr" runat="server" Width="100%" MaxLength="255"></asp:TextBox></td>
		</tr>
		<tr>
			<td align="right" class="gray" nowrap="nowrap"><%= ForumTP.Resources.various.ForumCategory %></td>
			<td>
				<span id="lblSelectGroup" class="gray" runat="server" enableviewstate="false"><%= ForumTP.Resources.various.SelectCategory %></span>
				<asp:DropDownList Width="130px" id="ddlForumGroup" runat="server" DataTextField="GroupName" DataValueField="GroupID"></asp:DropDownList>
				<a href="forumgroups.aspx" title="edit available forum groups" runat="server" id="lnkEditForumGroups" enableviewstate="false">.?.</a>
				&nbsp;&nbsp;&nbsp;
				<span id="lblEnterGroup" class="gray" runat="server" enableviewstate="false"><%= ForumTP.Resources.various.OrEnterNew %></span>
				<asp:TextBox id="tbForumGroup" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr><td></td><td><asp:Button id="btnAdd" runat="server" Text="<%# ForumTP.Resources.various.Add %>" onclick="btnAdd_Click"></asp:Button></td></tr>
		</tbody>
	</table>
</p>
<asp:Label ID="lblError" runat="server" Visible="False" EnableViewState="False" ForeColor="Red">Error: a forum was not created. Please fill all the fields.</asp:Label>

</asp:Content>
