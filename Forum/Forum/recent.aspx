<%@ Page Title="" Language="C#" MasterPageFile="~/Content/ForumMaster.Master" AutoEventWireup="true" CodeBehind="recent.aspx.cs" Inherits="ForumTP.recent" %>
<%@ Register TagPrefix="cc" TagName="RecentPosts" Src="recentposts.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="alternate" type="application/rss+xml" title="recent posts" href="recent.aspx?rss=1" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">
    <div class="location">
	<h2><a href="default.aspx"><%= ForumTP.Resources.various.Home %></a></h2>
    </div>

    <cc:RecentPosts id="recentPosts" runat="server"></cc:RecentPosts>
</asp:Content>
