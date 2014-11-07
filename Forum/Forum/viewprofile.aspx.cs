using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ForumTP
{
    public partial class viewprofile : ForumPage
    {
        protected int _userId;
        protected Button btnActivateUser;
        protected Button btnDeleteAllPostsAndDelete;
        protected Button btnDeleteAllPostsAndTopics;
        protected Button btnDelUser;
        protected Button btnDisableUser;
        protected Button btnEditUser;
        protected Button btnMakeAdmin;
        protected Button btnResendActivaton;
        protected Button btnRevokeAdmin;
        protected HtmlGenericControl divAchievements;
        protected HtmlGenericControl divGroups;
        protected GridView gridGroups;
        protected HyperLink homepage;
        protected HtmlImage imgAvatar;
        protected Label Label5;
        protected Label lblFullName;
        protected Label lblHomepage;
        protected Label lblInterests;
        protected Label lblInterestsTitle;
        protected Label lblLastLogonDate;
        protected Label lblLastLogonDateValue;
        protected Label lblProfile;
        protected Label lblRating;
        protected Label lblRatingValue;
        protected Label lblReggedSince;
        protected Label lblRegistrationDate;
        protected Label lblTotalPosts;
        protected Label lblUser;
        protected Label lblUserName;
        protected Label lblUsernameTitle;
        protected HtmlAnchor lnkViewPosts;
        protected Repeater rptAchievements;
        protected HtmlTableRow trRating;

        private void BindUserGroups()
        {
            IEnumerable<int> groupIdsForUser = ForumTP.Utility.User.GetGroupIdsForUser(this._userId);
            base.Cn.Open();
            DbDataReader reader = base.Cn.ExecuteReader("SELECT ForumUserGroups.GroupID, ForumUserGroups.Title\r\n\t\t\t\tFROM ForumUserGroups\r\n\t\t\t\tWHERE GroupID IN (" + (from x in groupIdsForUser select x.ToString()).Aggregate<string>((x, y) => (x + "," + y)) + ")", new object[0]);
            this.gridGroups.DataSource = reader;
            this.gridGroups.DataBind();
            reader.Close();
            base.Cn.Close();
        }

        protected void btnActivateUser_Click(object sender, EventArgs e)
        {
            if (base.IsAdministrator)
            {
                ForumTP.Utility.User.EnableUser(this._userId, false);
                this.btnActivateUser.Visible = false;
                this.btnDisableUser.Visible = true;
            }
        }

        protected void btnDeleteAllPostsAndDelete_Click(object sender, EventArgs e)
        {
            if (base.IsAdministrator)
            {
                ForumTP.Utility.User.DeleteAllPosts(this._userId);
                ForumTP.Utility.User.DeleteUser(this._userId);
                base.Response.Redirect("users.aspx");
            }
        }

        protected void btnDeleteAllPostsAndTopics_Click(object sender, EventArgs e)
        {
            if (base.IsAdministrator)
            {
                ForumTP.Utility.User.DeleteAllPosts(this._userId);
                this.btnActivateUser.Visible = false;
                this.btnDisableUser.Visible = true;
            }
        }

        protected void btnDelUser_Click(object sender, EventArgs e)
        {
            if (base.IsAdministrator)
            {
                ForumTP.Utility.User.DeleteUser(this._userId);
                base.Response.Redirect("users.aspx");
            }
        }

        protected void btnDisableUser_Click(object sender, EventArgs e)
        {
            if (base.IsAdministrator)
            {
                ForumTP.Utility.User.DisableUser(this._userId);
                this.btnActivateUser.Visible = true;
                this.btnDisableUser.Visible = false;
            }
        }

        protected void btnMakeAdmin_Click(object sender, EventArgs e)
        {
            if (base.IsAdministrator)
            {
                ForumTP.Utility.User.MakeAdmin(this._userId);
                this.btnMakeAdmin.Visible = false;
                this.btnRevokeAdmin.Visible = true;
            }
        }

        protected void btnResendActivaton_Click(object sender, EventArgs e)
        {
            if (base.IsAdministrator)
            {
                ForumTP.Utility.User.SendActivationEmail(this._userId);
            }
        }

        protected void btnRevokeAdmin_Click(object sender, EventArgs e)
        {
            if (base.IsAdministrator)
            {
                ForumTP.Utility.User.RevokeAdmin(this._userId);
                this.btnRevokeAdmin.Visible = false;
                this.btnMakeAdmin.Visible = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this._userId = int.Parse(base.Request.QueryString["UserID"]);
            }
            catch
            {
                base.Response.TrySkipIisCustomErrors = true;
                base.Response.StatusCode = 400;
                base.Response.Write("Invalid UserID passed");
                base.Response.End();
                return;
            }
            this.lblUserName.Visible = !Settings.ShowFullNamesInsteadOfUsernames;
            if (base.IsAdministrator)
            {
                this.BindUserGroups();
            }
            this.divAchievements.Visible = !Settings.DisableAchievements;
            if (!Settings.DisableAchievements)
            {
                this.rptAchievements.DataSource = Achievements.GetAchievementsForUser(this._userId);
                this.rptAchievements.DataBind();
            }
            this.trRating.Visible = Settings.EnableRating;
            this.divGroups.Visible = base.IsAdministrator && (this.gridGroups.Rows.Count > 0);
            this.btnEditUser.Visible = base.IsAdministrator || (this._userId == base.CurrentUserID);
            this.btnDelUser.Visible = base.IsAdministrator;
            this.btnActivateUser.Visible = base.IsAdministrator;
            this.btnDisableUser.Visible = base.IsAdministrator;
            this.btnDeleteAllPostsAndTopics.Visible = base.IsAdministrator;
            this.btnDeleteAllPostsAndDelete.Visible = base.IsAdministrator;
            this.btnEditUser.OnClientClick = "document.location.href='editprofile.aspx" + ((this._userId == base.CurrentUserID) ? "" : ("?userid=" + this._userId)) + "';return false;";
            base.Cn.Open();
            DbDataReader reader = base.Cn.ExecuteReader("SELECT * FROM ForumUsers WHERE UserID=" + this._userId, new object[0]);
            if (reader.Read())
            {
                this.lblUser.Text = reader["UserName"].ToString();
                base.Title = reader["UserName"].ToString();
                this.lblUserName.Text = reader["UserName"].ToString();
                this.lblFullName.Text = reader["FirstName"] + " " + reader["LastName"];
                this.lblInterests.Text = reader["Interests"].ToString();
                string str = reader["Homepage"].ToString();
                if (!str.StartsWith("http://") && !str.StartsWith("https://"))
                {
                    str = "http://" + str;
                }
                this.homepage.NavigateUrl = str;
                this.homepage.Text = reader["Homepage"].ToString();
                this.lnkViewPosts.InnerText = reader["PostsCount"].ToString();
                this.lnkViewPosts.HRef = "viewpostsbyuser.aspx?UserID=" + this._userId;
                this.lblRegistrationDate.Text = Convert.ToDateTime(reader["RegistrationDate"]).ToShortDateString();
                this.lblLastLogonDateValue.Text = reader["LastLogonDate"].ToString();
                bool flag = Convert.ToBoolean(reader["Disabled"]);
                this.btnActivateUser.Visible = flag && base.IsAdministrator;
                this.btnDisableUser.Visible = !flag && base.IsAdministrator;
                this.btnResendActivaton.Visible = flag && base.IsAdministrator;
                this.lblRatingValue.Text = reader["ReputationCache"].ToString();
                if (!(reader["ReputationCache"] is DBNull))
                {
                    Color red;
                    if (Convert.ToInt32(reader["ReputationCache"]) < 0)
                    {
                        red = Color.Red;
                    }
                    else
                    {
                        red = Color.Green;
                    }
                    this.lblRatingValue.ForeColor = red;
                }
                this.imgAvatar.Src = ForumTP.Utility.User.GetAvatarFileName(reader["AvatarFileName"], reader["UseGravatar"], reader["Email"]);
            }
            reader.Close();
            base.MetaDescription = "AspNetForum - viewing " + this.lblUser.Text + "'s user profile";
            bool flag2 = ForumTP.Utility.User.IsAdministrator(this._userId);
            this.btnMakeAdmin.Visible = !flag2 && base.IsAdministrator;
            this.btnRevokeAdmin.Visible = flag2 && base.IsAdministrator;
            base.Cn.Close();
        }
    }
}