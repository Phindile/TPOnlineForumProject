<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="True" EnableViewState="false" Inherits="ForumTP.ForumPage" MasterPageFile="~/Content/ForumMaster.Master" %>
<%@ Register TagPrefix="cc" TagName="RecentPosts" Src="recentposts.ascx" %>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
	<asp:Repeater ID="rptGroupsList" Runat="server" EnableViewState="False" >
		<ItemTemplate>
			<table style="width:100%;" class="roundedborder biglist">
				<tr><th colspan="2"><h2><%# Eval("GroupName") %></h2></th><th class="mobilehidden"><%= ForumTP.Resources.various.Threads %></th><th class="mobilehidden"><%= ForumTP.Resources.various.LatestPost %></th></tr>
				<tbody>
			<asp:repeater id="rptForumsList" runat="server" EnableViewState="False">
				<ItemTemplate>
					<tr <%# Container.ItemType == ListItemType.AlternatingItem ? " class='altItem'" : "" %> >
						<td align="center" style="width:10%;border-right:none;"><img alt="" src="<%# ForumTP.forums.GetForumIcon(Eval("IconFile")) %>" height="32" width="32" /></td>
						<td style="width:55%;border-left:none;padding-left: 0;"><h2>
							<a href='<%# ForumTP.Utility.Various.GetForumURL(Eval("ForumID"), Eval("Title")) %>'><%# Eval("Title") %></a>
							</h2>
							<span class="gray2"><%# Eval("Description") %></span>
						</td>
						<td width="50" class="gray2 mobilehidden" style="text-align: center">
							<%# Eval("TopicCount") %></td>
						<td style="white-space:nowrap" class="gray2 mobilehidden">
							<%# ForumTP.Utility.Topic.GetTopicInfoBMessageyID(Eval("LatestMessageID"), Cmd)%></td>
					</tr>
				</ItemTemplate>
			</asp:repeater>
			</table>
		</ItemTemplate>
		<FooterTemplate></tbody></FooterTemplate>
	</asp:Repeater>
	<div ID="lblNoForums" style="margin-top:20px;" runat="server" visible="false" enableviewstate="false"><%= ForumTP.Resources.various.NoForums %></div>
	<div id="divNoForumsAdmin" style="margin-top:20px;" runat="server" visible="false">No forums have been created yet. Please go to the <a href="admin.aspx">administrator panel</a>.</div>

	<table style="width:100%;" cellpadding="11" cellspacing="0" class="roundedborder">
		<tr><th><h2><%= ForumTP.Resources.various.WhatsGoingOn %></h2></th></tr>
		<tbody>
	<tr>
		<td>
			<span class="gray"><%= ForumTP.Resources.various.UsersOnline %></span>
			<%= ForumTP.Utility.User.OnlineUsersCount %>&nbsp;&nbsp;
			<span class="gray"><%= ForumTP.Resources.various.Members %></span>
			<%= ForumTP.Utility.User.OnlineRegisteredUsersCount %>&nbsp;&nbsp;
			<span class="gray"><%= ForumTP.Resources.various.Guests %></span>
			<%= ForumTP.Utility.User.OnlineUsersCount-ForumTP.Utility.User.OnlineRegisteredUsersCount%>
			<br /><br />
			<span class="gray"><%= ForumTP.Resources.various.Threads %></span>
			<%= ForumTP.Utility.Various.GetStats().ThreadCount %>&nbsp;&nbsp;
			<span class="gray"><%= ForumTP.Resources.various.Posts %></span>
			<%= ForumTP.Utility.Various.GetStats().PostCount %>&nbsp;&nbsp;
			<span class="gray"><%= ForumTP.Resources.various.Members %></span>
			<%= ForumTP.Utility.Various.GetStats().MemberCount %>
		</td>
	</tr>
			</tbody>
	</table>

	<div id="divRecent" runat="server" enableviewstate="false" visible="false">
	<br />
	<cc:RecentPosts id="recentPosts" runat="server"></cc:RecentPosts>
	</div>
</asp:Content>