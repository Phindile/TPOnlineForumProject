using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ForumTP
{
    public partial class recent : ForumPage
    {
        protected recentposts1 _recentPosts;

        private string GetRssXML()
        {
            if (base.Cache["RecentRSS"] != null)
            {
                return (base.Cache["RecentRSS"] as string);
            }
            string str = "";
            str = ((((((str + "<?xml version=\"1.0\"?>\r\n") + "<rss version=\"2.0\">\r\n" + "<channel>\r\n") + "<title>" + Settings.ForumTitle.Replace("&", "&amp;") + " - Recent Posts</title>\r\n") + "<link>" + Various.ForumURL + "recent.aspx</link>\r\n") + "<description>" + Settings.ForumTitle.Replace("&", "&amp;") + " - Recent Posts</description>\r\n") + "<language>en-us</language>\r\n") + "<docs>http://blogs.law.harvard.edu/tech/rss</docs>\r\n" + "<generator>Jitbit AspNetForum</generator>\r\n";
            base.Cn.Open();
            DbDataReader reader = base.Cn.ExecuteReader("SELECT TOP 30 ForumMessages.Body, ForumMessages.CreationDate, ForumTopics.TopicID, ForumTopics.Subject,\r\n\t\t\t\t\tForumUsers.UserName, ForumUsers.FirstName, ForumUsers.LastName, ForumMessages.UserID, ForumUsers.PostsCount\r\n\t\t\t\tFROM (ForumMessages INNER JOIN ForumTopics ON ForumMessages.TopicID=ForumTopics.TopicID)\r\n\t\t\t\tLEFT JOIN ForumUsers ON ForumMessages.UserID=ForumUsers.UserID\r\n\t\t\t\tWHERE ForumTopics.ForumID NOT IN (SELECT ForumID FROM ForumGroupPermissions WHERE AllowReading=?)\r\n\t\t\t\tAND ForumTopics.ForumID NOT IN (SELECT ForumID FROM Forums WHERE MembersOnly=?)\r\n\t\t\t\tORDER BY ForumMessages.MessageID DESC", new object[] { true, true });
            if (reader.HasRows)
            {
                int num = 0;
                while (reader.Read())
                {
                    if (num == 0)
                    {
                        DateTime time = (DateTime)reader["CreationDate"];
                        DateTime time2 = (DateTime)reader["CreationDate"];
                        str = str + string.Format("<pubDate>{0}</pubDate>\r\n", time.ToString("r")) + string.Format("<lastBuildDate>{0}</lastBuildDate>\r\n", time2.ToString("r"));
                    }
                    num++;
                    string str2 = str + "<item>\r\n" + string.Format("<link>{0}</link>\r\n", Various.ForumURL + Various.GetTopicURL(reader["TopicID"], reader["Subject"], false));
                    DateTime time3 = (DateTime)reader["CreationDate"];
                    str = ((str2 + "<title>Topic &quot;" + reader["Subject"].ToString().Replace("&", "&amp;") + "&quot; a message from " + ForumTP.Utility.User.GetUserDisplayName(reader["UserName"], reader["FirstName"], reader["LastName"]).Replace("&", "&amp;") + "</title>\r\n") + string.Format("<description><![CDATA[{0}]]></description>\r\n", Formatting.FormatMessageHTML(reader["Body"].ToString()))) + string.Format("<pubDate>{0}</pubDate>\r\n", time3.ToString("r")) + "</item>\r\n";
                }
            }
            reader.Close();
            base.Cn.Close();
            str = str + "</channel>\r\n" + "</rss>\r\n";
            base.Cache.Add("RecentRSS", str.ToString(), null, DateTime.Now.AddMinutes(15.0), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            return str;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bool flag = base.Request.QueryString["rss"] == "1";
            base.MetaDescription = Settings.ForumTitle + " - most recent forum posts. Sorted top to bottom.";
            if (flag)
            {
                base.Response.Clear();
                base.Response.ContentType = "text/xml";
                base.Response.Write(this.GetRssXML());
                base.Response.End();
            }
            else if (!string.IsNullOrEmpty(base.Request.QueryString["rss"]))
            {
                base.Response.TrySkipIisCustomErrors = true;
                base.Response.StatusCode = 400;
                base.Response.Write("Bad request");
                base.Response.End();
            }
        }
    }
}