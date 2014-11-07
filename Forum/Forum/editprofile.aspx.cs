using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Caching;
using ForumTP.Utility;
using System.Data.Common;
using ForumTP.Resources;
using System.Drawing;
using System.IO;

namespace ForumTP
{
    public partial class editprofile : ForumPage
    {
        private bool _bAvatarsEnabled;
        private int _editedUserID;
        private int _maxAvatarFileSize;
        private int _maxAvatarPictureSize;
        protected FileUpload avatarUpload;
        protected Button btnChangePsw;
        protected Button btnSave;
        protected Button btnSaveAvatar;
        protected CheckBox cbHidePresence;
        protected HtmlGenericControl divMain;
        protected HtmlImage imgAvatar;
        protected Label lblAvatar;
        protected Label lblAvatar2;
        protected Label lblAvatarsNote;
        protected Label lblChangePsw;
        protected Label lblConfPsw;
        protected Label lblEmail;
        protected Label lblFirstName;
        protected Label lblHome;
        protected Label lblHomepage;
        protected HyperLink lblInbox;
        protected Label lblInterests;
        protected Label lblLastName;
        protected Label lblLeaveBlank;
        protected Label lblMaxDimenstions;
        protected Label lblMaxSize;
        protected Label lblMyProfile;
        protected HyperLink lblMySubs;
        protected Label lblNewPsw;
        protected Label lblNotLoggedIn;
        protected Label lblOldPsw;
        protected Label lblProfile;
        protected Label lblResult;
        protected Label lblSignature;
        protected Label lblUsername;
        protected Repeater rptDefaultAvatars;
        protected Repeater rptMember;
        protected Repeater rptNotMember;
        protected TextBox tbAvatarURL;
        protected TextBox tbEmail;
        protected TextBox tbFirstName;
        protected TextBox tbHomepage;
        protected TextBox tbInterests;
        protected TextBox tbLastName;
        protected HtmlTable tblAvatar;
        protected HtmlTable tblChangePsw;
        protected HtmlTable tblGroups;
        protected TextBox tbNewPsw1;
        protected TextBox tbNewPsw2;
        protected TextBox tbOldPsw;
        protected TextBox tbSignature;
        protected TextBox tbUsername;
        protected HtmlTableRow trGravatar;
        
        private void BindMemberGroups()
        {
            IEnumerable<int> groupIdsForUser = ForumTP.Utility.User.GetGroupIdsForUser(this._editedUserID);
            base.Cn.Open();
            if (groupIdsForUser.Any<int>())
            {
                DbDataReader reader = base.Cn.ExecuteReader("SELECT ForumUserGroups.GroupID, ForumUserGroups.Title\r\n\t\t\t\t\tFROM ForumUserGroups\r\n\t\t\t\t\tWHERE GroupID IN (" + (from x in groupIdsForUser select x.ToString()).Aggregate<string>((x, y) => (x + "," + y)) + ")\r\n\t\t\t\t\tORDER BY Title", new object[0]);
                this.rptMember.DataSource = reader;
                this.rptMember.DataBind();
                reader.Close();

                DbDataReader reader2 = base.Cn.ExecuteReader("SELECT ForumUserGroups.GroupID, ForumUserGroups.Title\r\n\t\t\t\tFROM ForumUserGroups WHERE GroupID NOT IN (" + (from x in groupIdsForUser select x.ToString()).Aggregate<string>((x, y) => (x + "," + y)) + ") ORDER BY Title", new object[0]);
                this.rptNotMember.DataSource = reader2;
                this.rptNotMember.DataBind();
                base.Cn.Close();                
            }
            
        }
        
        protected void btnChangePsw_Click(object sender, EventArgs e)
        {
            if (((this.tbNewPsw1.Text == "") || (this.tbNewPsw2.Text == "")) || (this.tbNewPsw1.Text != this.tbNewPsw2.Text))
            {
                this.lblResult.Text = various.ErrorPasswordsDoNotMatch;
            }
            else if (this.tbNewPsw1.Text.Length < Settings.MinPasswordLength)
            {
                this.lblResult.Text = string.Format("Password is too short, {0} characters minimum", Settings.MinPasswordLength);
            }
            else
            {
                base.Cn.Open();
                object obj2 = base.Cn.ExecuteScalar("SELECT UserID FROM ForumUsers WHERE (Password=?) AND UserID=?", new object[] { Password.CalculateHash(this.tbOldPsw.Text), this._editedUserID });
                if (base.IsAdministrator || (obj2 != null))
                {
                    base.Cn.ExecuteNonQuery("UPDATE ForumUsers SET [Password]=? WHERE UserID=?", new object[] { Password.CalculateHash(this.tbNewPsw1.Text), this._editedUserID });
                    this.lblResult.Text = various.PasswordChanged;
                }
                else
                {
                    this.lblResult.Text = various.ErrorWrongOldPassword;
                }
                base.Cn.Close();
            }
        }
        
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ForumTP.Utility.User.CurrentUserID == this._editedUserID)
            {
                this.Session["AvatarPath"] = null;
            }
            string str = this.tbUsername.Text.Replace("<", "&lt;").Replace(">", "&gt;");
            string str2 = this.tbEmail.Text.Replace("<", "&lt;").Replace(">", "&gt;");
            string str3 = this.tbInterests.Text.Replace("<", "&lt;").Replace(">", "&gt;");
            string str4 = this.tbHomepage.Text.Replace("<", "&lt;").Replace(">", "&gt;");
            string str5 = this.tbFirstName.Text.Trim().Replace("<", "&lt;").Replace(">", "&gt;");
            string str6 = this.tbLastName.Text.Trim().Replace("<", "&lt;").Replace(">", "&gt;");
            string str7 = this.tbSignature.Text.Trim().Replace("<", "&lt;").Replace(">", "&gt;");
            str7 = (str7.Length > 0x3e8) ? str7.Substring(0, 0x3e8) : str7;
            if ((Settings.IntegratedAuthentication && (this._editedUserID == base.CurrentUserID)) && (this.tbUsername.Text.ToLower() != this.Session["aspnetforumUserName"].ToString().ToLower()))
            {
                this.lblResult.Text = various.ErrorIntegratedUserName;
            }
            else
            {
                base.Cn.Open();
                if (base.Cn.ExecuteScalar("SELECT UserID FROM ForumUsers WHERE UserName=? AND UserID<>?", new object[] { str, this._editedUserID }) != null)
                {
                    base.Cn.Close();
                    this.lblResult.Text = string.Format(various.ErrorUserExists, str);
                }
                else
                {
                    base.Cn.ExecuteNonQuery("UPDATE ForumUsers SET UserName=?, Email=?, Homepage=?, Interests=?, Signature=?, FirstName=?, LastName=?, HidePresence=? WHERE UserID=?", new object[] { str, str2, str4, str3, str7, str5, str6, this.cbHidePresence.Checked, this._editedUserID });
                    base.Cn.Close();
                    if (this._editedUserID == base.CurrentUserID)
                    {
                        this.Session["aspnetforumUserName"] = str;
                    }
                    this.lblResult.Text = various.ProfileSaved;
                    this.ShowUserInfo();
                }
            }
        }
        
        protected void btnSaveAvatar_Click(object sender, EventArgs e)
        {
            string avatarsDirAbsolutePath = Attachments.GetAvatarsDirAbsolutePath();
            string fileName = this.avatarUpload.PostedFile.FileName;
            if (!(fileName != ""))
            {
                if (base.Request.Form["DefaultAvatarInput"] != null)
                {
                    string str3 = base.Request.Form["DefaultAvatarInput"];
                    bool useGravatar = str3 == "GRAVATAR";
                    ForumTP.Utility.User.SetAvatarUrl(this._editedUserID, useGravatar ? "" : str3, useGravatar);
                }
                else if ((this.tbAvatarURL.Text.StartsWith("http://") || this.tbAvatarURL.Text.StartsWith("https://")) && (this.tbAvatarURL.Text.Length > 10))
                {
                    ForumTP.Utility.User.SetAvatarUrl(this._editedUserID, this.tbAvatarURL.Text, false);
                }
                else
                {
                    ForumTP.Utility.User.SetAvatarUrl(this._editedUserID, "", false);
                }
            }
            else
            {
                Bitmap bitmap;
                if (this.avatarUpload.PostedFile.ContentLength > this._maxAvatarFileSize)
                {
                    this.lblResult.Text = various.ErrorBigAvatar;
                    return;
                }
                fileName = Attachments.ChangeFileNameIfAlreadyExists(Path.GetFileName(fileName), avatarsDirAbsolutePath);
                try
                {
                    if (Attachments.IsExtForbidden(fileName))
                    {
                        throw new Exception("NOOOO");
                    }
                    bitmap = new Bitmap(this.avatarUpload.PostedFile.InputStream);
                }
                catch
                {
                    this.lblResult.Text = various.ErrorNotPictureFile;
                    return;
                }
                if ((bitmap.Width <= this._maxAvatarPictureSize) && (bitmap.Height <= this._maxAvatarPictureSize))
                {
                    this.avatarUpload.PostedFile.SaveAs(avatarsDirAbsolutePath + @"\" + fileName);
                    ForumTP.Utility.User.SetAvatarUrl(this._editedUserID, fileName, false);
                }
                else
                {
                    this.lblResult.Text = various.ErrorBigPictureDimensions;
                    return;
                }
            }
            if (ForumTP.Utility.User.CurrentUserID == this._editedUserID)
            {
                this.Session["AvatarPath"] = null;
            }
            this.ShowUserInfo();
        }
        
        protected string GetGravatarUrl()
        {
            return ForumTP.Utility.User.GetGravatarUrl(ForumTP.Utility.User.GetUserEmail(this._editedUserID));
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lblResult.Text = "";
            if (Settings.IntegratedAuthentication)
            {
                this.tblChangePsw.Visible = false;
                this.tbUsername.ReadOnly = true;
                this.lblUsername.Enabled = false;
            }
            this.trGravatar.Visible = Settings.EnableGravatar;
            if (base.Request.QueryString["userid"] != null)
            {
                this.lblOldPsw.Enabled = false;
                this.tbOldPsw.Enabled = false;
                this.lblInbox.Enabled = false;
                this.lblMySubs.Enabled = false;
                if (base.IsAdministrator)
                {
                    this._editedUserID = int.Parse(base.Request.QueryString["userid"]);
                }
                else
                {
                    this._editedUserID = 0;
                }
            }
            else
            {
                this._editedUserID = base.CurrentUserID;
            }
            if (this._editedUserID == 0)
            {
                this.lblNotLoggedIn.Visible = true;
                this.divMain.Visible = false;
            }
            else
            {
                this.lblNotLoggedIn.Visible = false;
                this.divMain.Visible = true;
                this._bAvatarsEnabled = Settings.EnableAvatars;
                this.tblAvatar.Visible = this._bAvatarsEnabled;
                this.lblInbox.Visible = Settings.EnablePrivateMessaging;
                if (this._bAvatarsEnabled)
                {
                    this._maxAvatarFileSize = Settings.MaxAvatarFileSizeInBytes;
                    this._maxAvatarPictureSize = Settings.MaxAvatarWidthHeight;
                    this.lblMaxSize.Text = this._maxAvatarFileSize.ToString();
                    this.lblMaxDimenstions.Text = string.Format("{0}x{1}", this._maxAvatarPictureSize, this._maxAvatarPictureSize);
                }
                this.ShowDefaultAvatars();
                this.lblAvatarsNote.Visible = base.IsAdministrator;
                this.tblGroups.Visible = base.IsAdministrator;
                if (!base.IsPostBack)
                {
                    this.ShowUserInfo();
                }
                if (base.IsAdministrator)
                {
                    this.BindMemberGroups();
                }
            }
        }
        
        protected void rptMember_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                ForumTP.Utility.User.RemoveUserFromGroup(this._editedUserID, int.Parse(e.CommandArgument.ToString()));
            }
            this.BindMemberGroups();
        }
        
        protected void rptNotMember_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "add")
            {
                ForumTP.Utility.User.AddUserToGroup(this._editedUserID, int.Parse(e.CommandArgument.ToString()));
            }
            this.BindMemberGroups();
        }
        
        private void ShowDefaultAvatars()
        {
            string[] files = Directory.GetFiles(base.MapPath("images"), "AspNetForumAvatar*");
            this.rptDefaultAvatars.Visible = files.Length > 0;
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            this.rptDefaultAvatars.DataSource = files;
            this.rptDefaultAvatars.DataBind();
        }
        
        private void ShowUserInfo()
        {
            base.Cn.Open();
            DbDataReader reader = base.Cn.ExecuteReader("SELECT * FROM ForumUsers WHERE UserID=" + this._editedUserID, new object[0]);
            if (reader.Read())
            {
                this.UseGravatar = Convert.ToBoolean(reader["UseGravatar"]);
                this.tbUsername.Text = reader["Username"].ToString();
                string email = reader["Email"].ToString();
                this.tbEmail.Text = email;
                this.tbHomepage.Text = reader["Homepage"].ToString();
                this.tbInterests.Text = reader["Interests"].ToString();
                this.tbSignature.Text = reader["Signature"].ToString();
                this.tbFirstName.Text = reader["FirstName"].ToString();
                this.tbLastName.Text = reader["LastName"].ToString();
                this.cbHidePresence.Checked = !(reader["HidePresence"] is DBNull) && Convert.ToBoolean(reader["HidePresence"]);
                string avatarFileName = reader["AvatarFileName"].ToString();
                this.imgAvatar.Visible = this._bAvatarsEnabled;
                this.imgAvatar.Src = ForumTP.Utility.User.GetAvatarFileName(avatarFileName, this.UseGravatar, email);
                if (avatarFileName == "http://")
                {
                    this.tbAvatarURL.Text = "";
                }
                else if (avatarFileName.StartsWith("http://") || avatarFileName.StartsWith("https://"))
                {
                    this.tbAvatarURL.Text = avatarFileName;
                }
                else
                {
                    this.tbAvatarURL.Text = "";
                }
            }
            reader.Close();
            base.Cn.Close();
        }
        
        protected bool UseGravatar { get; private set; }
    }
}