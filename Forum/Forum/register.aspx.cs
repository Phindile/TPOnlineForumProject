using ForumTP.Resources;
using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ForumTP
{
    public partial class register : ForumPage
    {
        protected Button btnAgree;
        protected Button btnDisagree;
        protected Button btnOK;
        protected HtmlGenericControl divTOS;
        protected Label lbl;
        protected Label lblCaptcha;
        protected Label lblCheck;
        protected Label lblEmail;
        protected Label lblError;
        protected Label lblHomepage;
        protected Label lblInterests;
        protected Label lblPsw;
        protected Label lblPswConf;
        protected Label lblRegister;
        protected Label lblSuccess;
        protected Label lblSuccessEmail;
        protected Label lblUsername;
        protected HtmlTable registerTable;
        protected TextBox tbEmail;
        protected TextBox tbHomepage;
        protected TextBox tbImgCode;
        protected TextBox tbInterests;
        protected TextBox tbPsw1;
        protected TextBox tbPsw2;
        protected TextBox tbUserName;

        protected void btnAgree_Click(object sender, EventArgs e)
        {
            this.registerTable.Visible = true;
        }

        protected void btnDisagree_Click(object sender, EventArgs e)
        {
            base.Response.Redirect("default.aspx");
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            Exception lastError = base.Server.GetLastError();
            if (((lastError is HttpRequestValidationException) || (lastError is ViewStateException)) || (lastError.InnerException is ViewStateException))
            {
                base.Response.End();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!base.IsPostBack)
            {
                this.registerTable.Visible = false;
                this.divTOS.Visible = true;
            }
            else
            {
                this.divTOS.Visible = false;
            }
            if (base.IsPostBack && this.registerTable.Visible)
            {
                this.RegisterUser();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.Session["CaptchaImageText"] = RandomNumericCode.NumericCode();
            this.tbImgCode.Text = "";
        }

        protected void RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(base.Request.Form["email"]))
            {
                if ((((this.tbUserName.Text.Trim().Length == 0) || (this.tbPsw1.Text.Trim().Length == 0)) || ((this.tbPsw2.Text.Trim().Length == 0) || (this.tbEmail.Text.Trim().Length == 0))) || ((this.tbEmail.Text.IndexOf("@") == -1) || (this.tbEmail.Text.IndexOf(".") == -1)))
                {
                    this.lblError.Visible = true;
                    this.lblError.Text = "Please, fill all the fields correctly";
                }
                else if (this.tbPsw1.Text != this.tbPsw2.Text)
                {
                    this.lblError.Visible = true;
                    this.lblError.Text = various.ErrorPasswordsDoNotMatch;
                }
                else if (this.tbPsw1.Text.Length < Settings.MinPasswordLength)
                {
                    this.lblError.Visible = true;
                    this.lblError.Text = string.Format("Password is too short, {0} characters minimum", Settings.MinPasswordLength);
                }
                else if (this.tbImgCode.Text != ((string)this.Session["CaptchaImageText"]))
                {
                    this.lblError.Visible = true;
                    this.lblError.Text = "Wrong code entered";
                }
                else
                {
                    string username = this.tbUserName.Text.Trim();
                    if (ForumTP.Utility.User.GetUserIdByUserName(username) != 0)
                    {
                        this.lblError.Visible = true;
                        this.lblError.Text = string.Format("Username {0} already exists, please select another one.", username);
                    }
                    else if (ForumTP.Utility.User.GetUserIdByEmail(this.tbEmail.Text) != 0)
                    {
                        this.lblError.Visible = true;
                        this.lblError.Text = string.Format("Email {0} already exists, please select another one or use the password recovery form.", this.tbEmail.Text);
                    }
                    else
                    {
                        bool enableEmailActivation = Settings.EnableEmailActivation;
                        bool newUsersNotifyAdmin = Settings.NewUsersNotifyAdmin;
                        bool newUsersDisabledByDefault = Settings.NewUsersDisabledByDefault;
                        string activationCode = RandomAlphaNumericCode.AlphaNumericCode(9);
                        base.Cn.Open();
                        int userId = ForumTP.Utility.User.CreateUser(username, this.tbEmail.Text, Password.CalculateHash(this.tbPsw1.Text), this.tbHomepage.Text, this.tbInterests.Text, newUsersDisabledByDefault, activationCode, "", "", "", "", "");
                        base.Cn.Close();
                        this.lblError.Visible = false;
                        this.lblSuccess.Visible = true;
                        this.registerTable.Visible = false;
                        if (enableEmailActivation && newUsersDisabledByDefault)
                        {
                            ForumTP.Utility.User.SendActivationEmail(userId);
                            this.lblSuccessEmail.Visible = true;
                        }
                        if (newUsersNotifyAdmin)
                        {
                            SendNots.SendNewUserRegAdminNotification(Various.ForumURL + "viewprofile.aspx?UserID=" + userId);
                        }
                    }
                }
            }
        }
    }
}