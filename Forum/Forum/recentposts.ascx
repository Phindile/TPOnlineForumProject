<%@ Control Language="C#" CodeBehind="recentposts.ascx.cs" Inherits="ForumTP.recentposts1" %>

<asp:repeater id="rptMessagesList" runat="server" EnableViewState="False" OnItemDataBound="rptMessagesList_ItemDataBound">
	<HeaderTemplate>
		<table width="100%" cellpadding="19" class="roundedborder postlist">
			<tr>
				<th><h2><%= ForumTP.Resources.various.RecentPosts %>&nbsp;<a href="recent.aspx?rss=1" runat="server" id="rssLink" enableviewstate="false"><img alt="recent posts - RSS" src="images/rss.png" /></a></h2></th>
			</tr>
			<tbody>
	</HeaderTemplate>
	<ItemTemplate>
		<tr valign="top" <%# Container.ItemType == ListItemType.AlternatingItem ? " class='altItem'" : "" %>>
			<td>
				<span class="gray">
				<%# ForumTP.ForumPage.ToAgoString((DateTime)Eval("CreationDate"))%><br />
				Topic:</span>
				<a href='<%# ForumTP.Utility.Various.GetTopicURL(Eval("TopicID"), Eval("Subject"), true) %>#post<%# Eval("MessageID") %>'><b><%# Eval("Subject") %></b></a>
				<br/><br/>
				<%# ForumTP.Utility.User.DisplayUserInfo(Eval("UserID"), Eval("UserName"), Eval("PostsCount"), Eval("AvatarFileName"), Eval("FirstName"), Eval("LastName"))%>
			</td>
			<td style="border-bottom:none;">
				<div class="mobileshown">
					<%# ForumTP.Utility.User.DisplayUserInfo(Eval("UserID"), Eval("UserName"), Eval("PostsCount"), Eval("AvatarFileName"), Eval("FirstName"), Eval("LastName"))%>
					<span class="gray2">Topic: <a href='<%# ForumTP.Utility.Various.GetTopicURL(Eval("TopicID"), Eval("Subject"), true) %>#post<%# Eval("MessageID") %>'><b><%# Eval("Subject") %></b></a></span>
				</div>
				<%# ForumTP.Utility.Formatting.FormatMessageHTML(ForumTP.Utility.Formatting.FormatInlineAttachmetns(Eval("Body").ToString(), Convert.ToInt32(Eval("MessageID"))))%>
			</td>
		</tr>
		<tr class="utils <%# Container.ItemType == ListItemType.AlternatingItem ? " altItem" : "" %>">
			<td></td>
			<td class="messageActions" align="right"><a runat="server" id="lnkQuote" visible="False"><%= ForumTP.Resources.various.ReplyWithQuote %></a></td>
		</tr>
	</ItemTemplate>
	<FooterTemplate></tbody></table></FooterTemplate>
</asp:repeater>
