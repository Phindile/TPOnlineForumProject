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
    public partial class addpost : ForumPage
    {
        private bool _addTopic;
        private bool _allowFileUploads;
        private bool _allowGuestPosts;
        private bool _changeTopic;
        private int _forumID;
        private bool _isEditing;
        private bool _isIPhoneOrAndroid;
        private int _messageAuthorID;
        private int _messageId;
        private bool _premoderated;
        private int _topicID;
        protected HtmlButton btnPreview;
        protected Button btnSave;
        protected HtmlButton btnSmilies;
        protected CheckBox cbSubscribe;
        protected HtmlGenericControl divCaptcha;
        protected HtmlGenericControl divEditbar;
        protected HtmlGenericControl divFiles;
        protected HtmlGenericControl divMain;
        protected HtmlGenericControl divPolls;
        protected FileUpload fileUpload;
        protected Label lblDenied;
        protected Label lblFileSizeError;
        protected Label lblMaxSize;
        protected Label lblSubjectText;
        protected RequiredFieldValidator reqSubject;
        protected Repeater rptExistingFiles;
        protected Repeater rptMessages;
        protected HtmlGenericControl spanUtils;
        protected TextBox tbImgCode;
        protected TextBox tbMsg;
        protected TextBox tbPollQuestion;
        protected TextBox tbSubj;

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (((base.CurrentUserID != 0) || !this._allowGuestPosts) || (this.tbImgCode.Text == ((string)this.Session["CaptchaImageText"])))
            {
                string msgBody = this.tbMsg.Text.Trim();
                if (msgBody != "")
                {
                    msgBody = msgBody.Replace("<", "&lt;").Replace(">", "&gt;");
                    bool flag = base.IsModerator(this._forumID);
                    bool visible = !this._premoderated || flag;
                    if (!Attachments.CheckAttachmentsSize())
                    {
                        this.lblMaxSize.Text = (Settings.MaxUploadFileSize / 0x3e8) + " Kb";
                        this.lblMaxSize.Visible = this.lblFileSizeError.Visible = true;
                    }
                    else
                    {
                        this.lblMaxSize.Visible = this.lblFileSizeError.Visible = false;
                        base.Cn.Open();
                        if (this._addTopic || this._changeTopic)
                        {
                            string subject = this.tbSubj.Text.Trim();
                            if (subject == "")
                            {
                                base.Cn.Close();
                                return;
                            }
                            subject = subject.Replace("<", "&lt;").Replace(">", "&gt;");
                            if (this._addTopic)
                            {
                                this._topicID = Topic.CreateTopic(base.Cn, this._forumID, base.CurrentUserID, subject, msgBody, visible);
                                string question = this.tbPollQuestion.Text.Trim().Replace("<", "&lt;").Replace(">", "&gt;");
                                if (question.Length > 0)
                                {
                                    Topic.CreatePoll(base.Cn, this._topicID, question, this.GetPollOptionsFromRequestForm());
                                }
                            }
                            else if (this._changeTopic)
                            {
                                Topic.ChangeTopicSubject(base.Cn, this._topicID, subject);
                            }
                        }
                        SendNots.UpdateTopicNotificationSettings(base.CurrentUserID, this._topicID, this.cbSubscribe.Checked, base.Cn);
                        if (this._isEditing)
                        {
                            if (flag || (this._messageAuthorID == base.CurrentUserID))
                            {
                                Message.UpdateMessageText(base.Cn, this._messageId, msgBody, visible);
                                Attachments.SaveAttachments(this._messageId, false, base.Cn);
                            }
                        }
                        else
                        {
                            this._messageId = Message.AddMessage(base.Cn, this._topicID, msgBody, visible, Various.GetUserIpAddress(base.Request), this._addTopic);
                            Attachments.SaveAttachments(this._messageId, false, base.Cn);
                        }
                        if (this._premoderated && !flag)
                        {
                            base.Cn.Close();
                            base.Response.Redirect("premoderatedmessage.aspx");
                        }
                        else
                        {
                            int num2 = (Convert.ToInt32(base.Cn.ExecuteScalar("SELECT COUNT(MessageID) FROM ForumMessages WHERE Visible=? AND TopicID=" + this._topicID, new object[] { true })) - 1) / base.PageSize;
                            base.Cn.Close();
                            string str4 = (this._changeTopic || this._addTopic) ? this.tbSubj.Text : this.lblSubjectText.Text;
                            string url = Various.GetTopicURL(this._topicID, str4, false);
                            string str6 = (url.IndexOf("?") > -1) ? "&" : "?";
                            url = (num2 > 0) ? string.Concat(new object[] { url, str6, "Page=", num2 }) : url;
                            object obj2 = url;
                            url = string.Concat(new object[] { obj2, str6, "MessageID=", this._messageId });
                            base.Response.Redirect(url);
                        }
                    }
                }
            }
        }

        private List<string> GetPollOptionsFromRequestForm()
        {
            List<string> list = new List<string>();
            for (int i = 0; (base.Request.Form["PollOption" + i] != null) && (base.Request.Form["PollOption" + i].Trim().Length > 0); i++)
            {
                list.Add(base.Request.Form["PollOption" + i].Replace("<", "&lt;").Replace(">", "&gt;"));
            }
            return list;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.tbSubj.Style.Add("width", "100%");
            this.tbMsg.Style.Add("width", "100%");
            this.tbSubj.Attributes["placeholder"] = various.Subject;
            this.btnSave.Text = various.AddMessage;
            this.cbSubscribe.Text = various.NotifyMeOnReply;
            this._allowFileUploads = Settings.EnableFileUploads;
            this.divFiles.Visible = (this._allowFileUploads && (base.CurrentUserID != 0)) && !this._isIPhoneOrAndroid;
            this._allowGuestPosts = Settings.AllowGuestPosts;
            if (base.Request.QueryString["TopicID"] != null)
            {
                this._topicID = int.Parse(base.Request.QueryString["TopicID"]);
            }
            if (base.Request.QueryString["ForumID"] != null)
            {
                this._forumID = int.Parse(base.Request.QueryString["ForumID"]);
            }
            if ((this._forumID == 0) && (this._topicID == 0))
            {
                base.Response.Write("Either Topic or Forum must be specified");
                base.Response.End();
            }
            if ((base.CurrentUserID == 0) && !this._allowGuestPosts)
            {
                base.Response.Write("Sorry, posting and editing is allowed only for authenticated users");
                base.Response.End();
            }
            this.cbSubscribe.Visible = (Settings.MailNotificationsEnabled && (base.CurrentUserID != 0)) && !this._isIPhoneOrAndroid;
            this.btnSmilies.Visible = Settings.AllowSmilies && !this._isIPhoneOrAndroid;
            this.spanUtils.Visible = this.divEditbar.Visible = this.btnPreview.Visible = !this._isIPhoneOrAndroid;
            base.Cn.Open();
            this._messageId = 0;
            if (base.Request.QueryString["Edit"] != null)
            {
                this._messageId = int.Parse(base.Request.QueryString["Edit"]);
                this._isEditing = true;
                this.btnSave.Text = "update message";
                object obj2 = base.Cn.ExecuteScalar("SELECT MIN(MessageID) FROM ForumMessages WHERE TopicID=" + this._topicID, new object[0]);
                this._changeTopic = Convert.ToInt32(obj2) == this._messageId;
            }
            if (base.Request.QueryString["Quote"] != null)
            {
                this._messageId = int.Parse(base.Request.QueryString["Quote"]);
                this._isEditing = false;
            }
            if (this._forumID == 0)
            {
                this._addTopic = false;
                bool flag = false;
                DbDataReader reader = base.Cn.ExecuteReader("SELECT Forums.ForumID, Forums.Title, Forums.Premoderated, ForumTopics.IsClosed, ForumTopics.Subject FROM Forums INNER JOIN ForumTopics ON Forums.ForumID=ForumTopics.ForumID WHERE ForumTopics.TopicID=" + this._topicID, new object[0]);
                if (reader.Read())
                {
                    this._forumID = Convert.ToInt32(reader["ForumID"]);
                    this._premoderated = Convert.ToBoolean(reader["Premoderated"]);
                    flag = Convert.ToBoolean(reader["IsClosed"]);
                    if (this._changeTopic)
                    {
                        if (!base.IsPostBack)
                        {
                            this.tbSubj.Text = reader["Subject"].ToString();
                        }
                    }
                    else
                    {
                        this.lblSubjectText.Text = reader["Subject"].ToString();
                    }
                }
                reader.Close();
                if (flag && !this._isEditing)
                {
                    base.Cn.Close();
                    base.Response.End();
                    return;
                }
            }
            else
            {
                this._addTopic = true;
                DbDataReader reader2 = base.Cn.ExecuteReader("SELECT Forums.ForumID, Forums.Title, Forums.Premoderated FROM Forums WHERE Forums.ForumID=" + this._forumID, new object[0]);
                if (reader2.Read())
                {
                    this._premoderated = Convert.ToBoolean(reader2["Premoderated"]);
                }
                reader2.Close();
            }
            this.divPolls.Visible = this._addTopic && !this._isIPhoneOrAndroid;
            if (!Forum.CheckForumPostPermissions(this._forumID, base.CurrentUserID))
            {
                this.lblDenied.Visible = true;
                this.divMain.Visible = false;
            }
            if (this._addTopic || this._changeTopic)
            {
                this.tbSubj.Visible = true;
                this.reqSubject.Enabled = true;
            }
            if (!this._addTopic && !base.IsPostBack)
            {
                if (this.cbSubscribe.Visible)
                {
                    object obj3 = base.Cn.ExecuteScalar(string.Concat(new object[] { "SELECT UserID FROM ForumSubscriptions WHERE UserID=", base.CurrentUserID, " AND TopicID=", this._topicID }), new object[0]);
                    this.cbSubscribe.Checked = obj3 != null;
                }
                if (!this._isIPhoneOrAndroid)
                {
                    DbDataReader reader3 = base.Cn.ExecuteReader("SELECT ForumMessages.Body, ForumUsers.UserName, ForumMessages.CreationDate\r\n\t\t\t\t\t\t\tFROM ForumMessages LEFT JOIN ForumUsers ON ForumUsers.UserID=ForumMessages.UserID\r\n\t\t\t\t\t\t\tWHERE ForumMessages.TopicID=" + this._topicID + " and ForumMessages.Visible=? ORDER BY ForumMessages.CreationDate DESC", new object[] { true });
                    this.rptMessages.DataSource = reader3;
                    this.rptMessages.DataBind();
                    reader3.Close();
                }
                else
                {
                    this.rptMessages.Visible = false;
                }
            }
            if (this._messageId != 0)
            {
                object obj4 = base.Cn.ExecuteScalar("SELECT UserID FROM ForumMessages WHERE MessageID=" + this._messageId, new object[0]);
                this._messageAuthorID = (obj4 == null) ? -1 : Convert.ToInt32(obj4);
                if (!base.IsPostBack)
                {
                    DbDataReader reader4;
                    if (this._isEditing)
                    {
                        reader4 = base.Cn.ExecuteReader("SELECT FileID, FileName FROM ForumUploadedFiles WHERE MessageID=" + this._messageId, new object[0]);
                        this.rptExistingFiles.DataSource = reader4;
                        this.rptExistingFiles.DataBind();
                        this.rptExistingFiles.Visible = this.rptExistingFiles.Items.Count > 0;
                        reader4.Close();
                    }
                    reader4 = base.Cn.ExecuteReader("SELECT ForumMessages.Body, ForumUsers.UserName, ForumUsers.FirstName, ForumUsers.LastName, ForumMessages.UserID FROM ForumMessages LEFT OUTER JOIN ForumUsers ON ForumUsers.UserID=ForumMessages.UserID WHERE ForumMessages.MessageID=" + this._messageId, new object[0]);
                    if (reader4.Read())
                    {
                        string str = Regex.Replace(reader4["Body"].ToString().Replace("<br>", "\r\n").Replace("<br/>", "\r\n").Replace("<br />", "\r\n"), @"<\S[^>]*>", "");
                        if (!this._isEditing)
                        {
                            string str2 = ForumTP.Utility.User.GetUserDisplayName(reader4["UserName"].ToString(), reader4["FirstName"].ToString(), reader4["LastName"].ToString());
                            this.tbMsg.Text = "[quote=" + str2 + "]" + str + "[/quote]\r\n\r\n";
                        }
                        else
                        {
                            this.tbMsg.Text = str;
                        }
                    }
                    reader4.Close();
                }
            }
            base.Cn.Close();
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this._isIPhoneOrAndroid = base.IsiPhoneOrAndroid();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if ((base.CurrentUserID == 0) && this._allowGuestPosts)
            {
                this.Session["CaptchaImageText"] = RandomNumericCode.NumericCode();
                this.tbImgCode.Text = "";
                this.divCaptcha.Visible = true;
            }
            else
            {
                this.divCaptcha.Visible = false;
            }
        }
    }
}