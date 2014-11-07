using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Web.DynamicData;
using ForumTP.Utility;
using System.Data.Common;

namespace ForumTP
{
    public partial class recentposts1 : UserControl
    {
        private int _currentUserId;
        protected Repeater rptMessagesList;

        public static void BindRecentPostsRepeater(Repeater rptMessagesList, int pageSize)
        {
            using (DbConnection connection = DataAccessUtil.CreateOpenConnection())
            {
                DbDataReader reader;
                if (User.CurrentUserID == 0)
                {
                    reader = connection.ExecuteReader("SELECT TOP " + Settings.PageSize + " ForumMessages.Body, ForumMessages.CreationDate, ForumTopics.TopicID, ForumTopics.Subject, ForumUsers.UserName, ForumMessages.UserID, ForumUsers.PostsCount, ForumUsers.AvatarFileName, ForumMessages.MessageID, ForumUsers.FirstName, ForumUsers.LastName\r\n\t\t\t\t\tFROM (ForumMessages INNER JOIN ForumTopics ON ForumMessages.TopicID=ForumTopics.TopicID)\r\n\t\t\t\t\tLEFT JOIN ForumUsers ON ForumMessages.UserID=ForumUsers.UserID\r\n\t\t\t\t\tWHERE ForumMessages.Visible=?\r\n\t\t\t\t\tAND ForumTopics.ForumID NOT IN (SELECT DISTINCT ForumID FROM ForumGroupPermissions WHERE AllowReading=?)\r\n\t\t\t\t\tAND ForumTopics.ForumID NOT IN (SELECT ForumID FROM Forums WHERE MembersOnly=?)\r\n\t\t\t\t\tORDER BY ForumMessages.MessageID DESC", new object[] { true, true, true });
                }
                else
                {
                    string readableForumsForUserString = Forum.GetReadableForumsForUserString(User.CurrentUserID);
                    reader = connection.ExecuteReader(string.Concat(new object[] { "SELECT TOP ", Settings.PageSize, " ForumMessages.Body, ForumMessages.CreationDate, ForumTopics.TopicID, ForumTopics.Subject, ForumUsers.UserName, ForumMessages.UserID, ForumUsers.PostsCount, ForumUsers.AvatarFileName, ForumMessages.MessageID, ForumUsers.FirstName, ForumUsers.LastName\r\n\t\t\t\t\tFROM (ForumMessages INNER JOIN ForumTopics ON ForumMessages.TopicID=ForumTopics.TopicID)\r\n\t\t\t\t\tLEFT JOIN ForumUsers ON ForumMessages.UserID=ForumUsers.UserID\r\n\t\t\t\t\tWHERE ForumMessages.Visible=?\r\n\t\t\t\t\tAND ForumTopics.ForumID IN (", readableForumsForUserString, ")\r\n\t\t\t\t\tORDER BY ForumMessages.MessageID DESC" }), new object[] { true });
                }
                rptMessagesList.DataSource = reader;
                rptMessagesList.DataBind();
                reader.Close();
                connection.Close();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this._currentUserId = User.CurrentUserID;
            if (this.Visible)
            {
                BindRecentPostsRepeater(this.rptMessagesList, ((ForumPage)this.Page).PageSize);
            }
        }

        protected void rptMessagesList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                IDataRecord dataItem = (IDataRecord)e.Item.DataItem;
                if (this._currentUserId != 0)
                {
                    HtmlAnchor anchor = (HtmlAnchor)e.Item.FindControl("lnkQuote");
                    anchor.Visible = true;
                    anchor.HRef = string.Concat(new object[] { "addpost.aspx?TopicID=", dataItem["TopicID"], "&Quote=", dataItem["MessageID"] });
                }
            }
        }
    }
}
