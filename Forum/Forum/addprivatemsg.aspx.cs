using ForumTP.Resources;
using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ForumTP
{
    public partial class addprivatemsg : ForumPage
    {
        protected Button btnSave;
        protected HtmlGenericControl divFiles;
        protected HtmlGenericControl divMain;
        protected FileUpload fileUpload;
        protected Label lblError;
        protected Label lblFileSizeError;
        protected Label lblMaxSize;
        protected bool mailNotificationsEnabled;
        protected TextBox tbMsg;
        protected int toUserID;

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Attachments.CheckAttachmentsSize())
            {
                this.lblMaxSize.Text = (Settings.MaxUploadFileSize / 0x3e8) + " Kb";
                this.lblMaxSize.Visible = true;
                this.lblFileSizeError.Visible = true;
            }
            else
            {
                ForumTP.Utility.User.SendPM(this.toUserID, this.tbMsg.Text);
                base.Response.Redirect("privateinbox.aspx?UserID=" + this.toUserID);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Settings.EnablePrivateMessaging)
            {
                base.Response.End();
            }
            else
            {
                try
                {
                    this.toUserID = int.Parse(base.Request.QueryString["ToUserID"]);
                    if (base.CurrentUserID == 0)
                    {
                        throw new Exception("not logged in");
                    }
                }
                catch
                {
                    this.divMain.Style["display"] = "none";
                    this.lblError.Visible = true;
                    return;
                }
                this.btnSave.DataBind();
                this.mailNotificationsEnabled = Settings.MailNotificationsEnabled;
                if ((base.Request.QueryString["Quote"] != null) && !base.IsPostBack)
                {
                    int num = int.Parse(base.Request.QueryString["Quote"]);
                    base.Cn.Open();
                    DbDataReader reader = base.Cn.ExecuteReader("SELECT ForumPersonalMessages.Body, ForumUsers.UserName\r\n\t\t\t\t\tFROM ForumUsers INNER JOIN ForumPersonalMessages ON ForumUsers.UserID=ForumPersonalMessages.FromUserID\r\n\t\t\t\t\tWHERE ForumPersonalMessages.MessageID=?", new object[] { num });
                    if (reader.Read())
                    {
                        string str = Regex.Replace(reader["Body"].ToString().Replace("<br>", "\r\n"), @"<\S[^>]*>", "");
                        this.tbMsg.Text = "[quote=" + reader["UserName"].ToString() + "]" + str + "[/quote]";
                    }
                    reader.Close();
                    base.Cn.Close();
                }
            }
        }
    }
}