using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ForumTP.Utility;

namespace ForumTP
{
    public partial class users : ForumPage
    {
        protected Label lblAll;
        protected Label lblHome;
        protected Label lblNonActivated;
        protected Label lblUsers;
        protected HtmlAnchor lnkOnlineUsers;
        protected Repeater rptMostActive;
        protected Repeater rptRecent;
        protected Repeater rptRecentlyActive;
        protected HtmlGenericControl spanAddUser;
        protected HtmlGenericControl spanNonActive;

        private void BindActiveUsers()
        {
            DbDataReader reader = base.Cn.ExecuteReader("SELECT TOP 15 ForumUsers.UserID, ForumUsers.UserName, COUNT(ForumMessages.MessageID) AS MsgCount, ForumUsers.AvatarFileName, ForumUsers.FirstName, ForumUsers.LastName\r\n\t\t\t\tFROM ForumUsers INNER JOIN ForumMessages ON ForumUsers.UserID=ForumMessages.UserID\r\n\t\t\t\tWHERE Disabled=0 AND HidePresence=0\r\n\t\t\t\tGROUP BY ForumUsers.UserID, ForumUsers.UserName, ForumUsers.AvatarFileName, ForumUsers.FirstName, ForumUsers.LastName\r\n\t\t\t\tORDER BY COUNT(ForumMessages.MessageID) DESC", new object[0]);
            this.rptMostActive.DataSource = reader;
            this.rptMostActive.DataBind();
            reader.Close();
        }

        private void BindRecentlyActiveUsers()
        {
            DbDataReader reader = base.Cn.ExecuteReader("SELECT TOP 15 ForumUsers.UserID, ForumUsers.UserName, COUNT(ForumMessages.MessageID) AS MsgCount, ForumUsers.AvatarFileName, ForumUsers.FirstName, ForumUsers.LastName\r\n\t\t\t\tFROM ForumUsers INNER JOIN ForumMessages ON ForumUsers.UserID=ForumMessages.UserID\r\n\t\t\t\tWHERE ForumMessages.CreationDate>?\r\n\t\t\t\tAND Disabled=0 AND HidePresence=0\r\n\t\t\t\tGROUP BY ForumUsers.UserID, ForumUsers.UserName, ForumUsers.AvatarFileName, ForumUsers.FirstName, ForumUsers.LastName\r\n\t\t\t\tORDER BY COUNT(ForumMessages.MessageID) DESC", new object[] { Various.GetCurrTime().AddDays(-14.0) });
            this.rptRecentlyActive.DataSource = reader;
            this.rptRecentlyActive.DataBind();
            reader.Close();
        }

        private void BindRecentUsers()
        {
            DbDataReader reader = base.Cn.ExecuteReader("SELECT top 15 UserID, UserName, AvatarFileName, FirstName, LastName\r\n\t\t\t\tFROM ForumUsers WHERE Disabled=0 AND HidePresence=0 ORDER BY UserID DESC", new object[0]);
            DataTable table = new DataTable();
            table.Load(reader);
            table.DefaultView.Sort = "UserName";
            this.rptRecent.DataSource = table.DefaultView;
            this.rptRecent.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (base.CurrentUserID == 0)
            {
                base.Response.Redirect("default.aspx", true);
            }
            else
            {
                this.lnkOnlineUsers.Visible = this.spanNonActive.Visible = this.spanAddUser.Visible = base.IsAdministrator;
                base.Cn.Open();
                this.BindRecentUsers();
                this.BindActiveUsers();
                this.BindRecentlyActiveUsers();
                base.Cn.Close();
            }
        }
    }
}