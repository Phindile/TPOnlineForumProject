using ForumTP.Content;
using ForumTP.Resources;
using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ForumTP
{
    public partial class ForumPage : System.Web.UI.Page
    {        
        private int? _currentUserId;
        private string _forumTitle;
        private bool? _isAdministrator = null;
        private readonly HtmlMeta _metaDescription = new HtmlMeta();
        private readonly HtmlMeta _metaKeywords = new HtmlMeta();
        private static Regex _regexBody = new Regex("<body.*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public DbCommand Cmd = DataAccessUtil.CreateCommand(null, null, new object[0]);
        public DbConnection Cn;
        
        public ForumPage()
        {
            this.Cn = this.Cmd.Connection;
            this.PageSize = Settings.PageSize;
            this._forumTitle = Settings.ForumTitle;
            this._metaKeywords.Content = this._forumTitle;
            this._metaDescription.Content = this._forumTitle;
            this._metaDescription.Name = "description";
            this._metaKeywords.Name = "keywords";
        }
        
        private void ApplicationInstance_Error(object sender, EventArgs e)
        {
            Exception lastError = base.Server.GetLastError();
            if (lastError is TypeInitializationException)
            {
                HttpRuntime.UnloadAppDomain();
            }
            else if ((lastError != null) && (!(lastError is ViewStateException) && !(lastError.InnerException is ViewStateException)))
            {
                try
                {
                    string body = lastError.ToString();
                    if ((HttpContext.Current != null) && (HttpContext.Current.Request != null))
                    {
                        body = string.Concat(new object[] { HttpContext.Current.Request.Url, "\n\n", body, "\n\n", HttpContext.Current.Request.UserAgent, "\n", HttpContext.Current.Request.UserHostAddress });
                    }
                    SendNots.SendEmail(!string.IsNullOrEmpty(Settings.SendErrorReportsTo) ? Settings.SendErrorReportsTo.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries) : new string[1], "error in the Forum application", body, true, false);
                }
                catch
                {
                }
            }
        }
        
        public static void AssignButtonTextboxEnterKey(TextBox textbox, string buttonClientId)
        {
            string str = "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + buttonClientId + "').click();return false;}} else {return true};";
            textbox.Attributes.Add("onkeypress", str);
        }
        
        public static void AssignButtonTextboxEnterKey(TextBox textbox, Button button)
        {
            AssignButtonTextboxEnterKey(textbox, button.ClientID);
        }
        
        private static T FindControlByTypeRecursive<T>(Control parent) where T: Control
        {
            if (parent is T)
            {
                return (T) parent;
            }
            foreach (Control control in parent.Controls)
            {
                T local = FindControlByTypeRecursive<T>(control);
                if (local != null)
                {
                    return local;
                }
            }
            return default(T);
        }
        
        private string GetCurrentPageName()
        {
            return Path.GetFileName(base.Request.CurrentExecutionFilePath);
        }
        
        protected bool IsIpad()
        {
            return ((base.Request.UserAgent != null) && base.Request.UserAgent.ToLower().Contains("ipad"));
        }
        
        protected bool IsiPhoneOrAndroid()
        {
            if ((base.Request.QueryString["mobile"] == "0") || (this.Session["AlwaysFullVersion"] != null))
            {
                this.Session["AlwaysFullVersion"] = true;
                return false;
            }
            if (base.Request.UserAgent == null)
            {
                return false;
            }
            string str = base.Request.UserAgent.ToLower();
            if ((!str.Contains("iphone") && !str.Contains("ipod")) && !str.Contains("android"))
            {
                return str.Contains("blackberry");
            }
            return true;
        }
        
        public bool IsModerator(int forumid)
        {
            if (this.CurrentUserID == 0)
            {
                return false;
            }
            return ForumTP.Utility.User.IsModerator(forumid, this.CurrentUserID);
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.IsNonLoginPostBack = base.IsPostBack;
            bool flag = this.IsiPhoneOrAndroid();
            if (flag && (base.Request.QueryString["rss"] != "1"))
            {
                string currentPageName = this.GetCurrentPageName();
                if (!currentPageName.ToLower().Contains("-iphone"))
                {
                    string virtualPath = currentPageName.Substring(0, currentPageName.IndexOf(".")) + "-iphone.aspx";
                    if (File.Exists(base.Request.MapPath(virtualPath)))
                    {
                        base.Server.Execute(virtualPath, base.Response.Output, true);
                        base.Response.End();
                        return;
                    }
                }
            }
            if (!flag)
            {
                switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                {
                    case "ar":
                    case "he":
                        this.Page.Form.Attributes.Add("dir", "rtl");
                        break;
                }
            }
            this.Context.ApplicationInstance.Error += new EventHandler(this.ApplicationInstance_Error);
            HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            base.Response.Cache.SetExpires(DateTime.Now);
            base.Response.Cache.SetValidUntilExpires(true);
            if (Settings.BannedIPs != null)
            {
                string userIpAddress = Various.GetUserIpAddress(base.Request);
                foreach (string str5 in Settings.BannedIPs)
                {
                    if (StringUtils.IpAddressMatchesPattern(userIpAddress, str5))
                    {
                        base.Response.Write("Looks like your IP-address " + userIpAddress + " has been banned by the forum administrator.");
                        base.Response.End();
                    }
                }
            }
            if (Settings.DisableRSS && (base.Master is ForumMaster))
            {
                ContentPlaceHolder mainPlaceHolder = ((ForumMaster)base.Master).MainPlaceHolder;
                if (mainPlaceHolder != null)
                {
                    HtmlAnchor anchor = mainPlaceHolder.FindControl("rssLink") as HtmlAnchor;
                    if (anchor != null)
                    {
                        anchor.Visible = false;
                    }
                }
            }
            bool integratedAuthentication = Settings.IntegratedAuthentication;
            if (this.CurrentUserID == 0)
            {
                if (((!base.IsPostBack || (base.Request.Form["LoginName"] == null)) || ((base.Request.Form["LoginName"] == "") || (base.Request.Form["Password"] == null))) || ((base.Request.Form["Password"] == "") || (base.Request.Form["loginbutton"] == null)))
                {
                    if (!integratedAuthentication)
                    {
                        if ((base.Request.Cookies["aspnetforumUID"] != null) && (base.Request.Cookies["aspnetforumUID"].Value != ""))
                        {
                            ForumTP.Utility.User.ProcessCookieLogin();
                        }
                    }
                    else if (this.Page.User.Identity.IsAuthenticated)
                    {
                        ForumTP.Utility.User.ProcessMembershipLogin(base.User.Identity.Name);
                    }
                }
                else
                {
                    bool flag3;
                    bool flag4;
                    int num;
                    this.IsNonLoginPostBack = false;
                    ForumTP.Utility.User.ProcessLogin(base.Request.Form["LoginName"], base.Request.Form["Password"], out flag3, out flag4, out num);
                    if (flag3)
                    {
                        if (flag4)
                        {
                            this.Session["InvalidLoginUserId"] = num;
                            base.Response.Redirect("notactivated.aspx");
                        }
                        else if (this is activate)
                        {
                            base.Response.Redirect("default.aspx");
                        }
                    }
                    else if ((base.Master is ForumMaster) && (((ForumMaster)base.Master).LoginErrorLabel != null))
                    {
                        ((ForumMaster)base.Master).LoginErrorLabel.Visible = true;
                    }
                }
            }
            else
            {
                if (integratedAuthentication)
                {
                    if (!base.User.Identity.IsAuthenticated)
                    {
                        ForumTP.Utility.User.Logout();
                        return;
                    }
                    if (this.Session["aspnetforumUserName"].ToString() != base.User.Identity.Name)
                    {
                        ForumTP.Utility.User.ProcessMembershipLogin(base.User.Identity.Name);
                    }
                }
                ForumTP.Utility.User.UpdateCurrentUserLastLogonDate();
            }
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            ForumTP.Utility.User.UpdateOnlineUsersCount();
            base.OnPreRender(e);
            if ((base.Header != null) && (FindControlByTypeRecursive<HtmlMeta>(this.Page) == null))
            {
                base.Header.Controls.Add(this._metaDescription);
                base.Header.Controls.Add(this._metaKeywords);
            }
            base.Title = base.Title + (string.IsNullOrEmpty(base.Title) ? "" : " - ") + this._forumTitle;
        }
        
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            this.Cmd.Dispose();
            this.Cn.Dispose();
        }
        
        protected override void Render(HtmlTextWriter writer)
        {
            using (StringWriter writer2 = new StringWriter())
            {
                using (HtmlTextWriter writer3 = new HtmlTextWriter(writer2))
                {
                    base.Render(writer3);                    
                    writer.Write(writer2);
                }
            }
        }
        
        protected void SendOutRssAndQuit(string rssXml)
        {
            base.Response.Clear();
            base.Response.ContentType = "text/xml";
            base.Response.Write(rssXml);
            base.Response.End();
        }
        
        public static string ToAgoString(DateTime date)
        {
            return date.ToAgoString(various.SecondsAgo, various.MinutesAgo, various.HoursAgo, various.DaysAgo, "d", new DateTime?(Various.GetCurrTime()));
        }
        
        public int CurrentUserID
        {
            get
            {
                if (!this._currentUserId.HasValue || (this._currentUserId.Value == 0))
                {
                    this._currentUserId = new int?(ForumTP.Utility.User.CurrentUserID);
                }
                return this._currentUserId.Value;
            }
        }
        
        public bool IsAdministrator
        {
            get
            {
                if (!this._isAdministrator.HasValue)
                {
                    if (this.CurrentUserID == 0)
                    {
                        this._isAdministrator = false;
                    }
                    else
                    {
                        this._isAdministrator = new bool?(ForumTP.Utility.User.IsAdministrator(this.CurrentUserID));
                    }
                }
                return this._isAdministrator.Value;
            }
        }
        
        public bool IsNonLoginPostBack { get; private set; }
        
        public string MetaDescription
        {
            get
            {
                return this._metaDescription.Content;
            }
            set
            {
                try
                {
                    this._metaDescription.Content = value;
                }
                catch
                {
                }
            }
        }
        
        public string MetaKeywords
        {
            get
            {
                return this._metaKeywords.Content;
            }
            set
            {
                string str = Regex.Replace(Regex.Replace(value, @"\W", ","), ",{2,}", ",");
                try
                {
                    this._metaKeywords.Content = str;
                }
                catch
                {
                }
            }
        }
        
        public int PageSize { get; private set; }
    }

    public class AdminForumPage : ForumPage
    {
        protected override void OnLoad(EventArgs e)
        {
            if (!base.IsAdministrator)
            {
                if (base.Cn.State == ConnectionState.Open)
                {
                    base.Cn.Close();
                }
                base.Response.Redirect("default.aspx", true);
                base.Response.End();
            }
            else
            {
                base.OnLoad(e);
            }
        }
    }
}
