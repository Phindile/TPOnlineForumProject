using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ForumTP
{
    public partial class allusers : ForumPage
    {
        private bool _showDisabled;
        protected ImageButton btnDel;
        protected ImageButton btnDisable;
        protected HtmlAnchor lnkAdd;
        protected string pagerString = "";
        protected Repeater rptUsersList;
        protected HtmlGenericControl spanActive;
        protected HtmlGenericControl spanNonActive;
        protected TextBox tbUsername;

        private void BindRepeater(string username)
        {
            base.Cn.Open();
            string commandText = "SELECT * FROM ForumUsers WHERE Disabled=? ";
            if (!base.IsAdministrator)
            {
                commandText = commandText + " AND HidePresence=0";
            }
            if (base.Request.QueryString["Admin"] != null)
            {
                commandText = commandText + " AND UserID IN (SELECT UserID FROM ForumAdministrators)";
            }
            if ((username != null) && (username.Trim() != ""))
            {
                username = username.Replace("'", "");
                commandText = commandText + string.Format(" AND (UserName LIKE '{0}%' OR Email LIKE '{0}%') ", username);
            }
            string str2 = base.Request.QueryString["order"];
            if (str2 == "regdate")
            {
                commandText = commandText + " ORDER BY RegistrationDate";
            }
            else if (str2 == "email")
            {
                commandText = commandText + " ORDER BY Email";
            }
            else if (str2 == "posts")
            {
                commandText = commandText + " ORDER BY PostsCount";
            }
            else if (str2 == "logondate")
            {
                commandText = commandText + " ORDER BY LastLogonDate";
            }
            else
            {
                commandText = commandText + " ORDER BY UserName";
            }
            DataTable table = new DataTable();
            DbDataReader reader = base.Cn.ExecuteReader(commandText, new object[] { base.Request.QueryString["Disabled"] == "1" });
            table.Load(reader);
            reader.Close();
            base.Cn.Close();
            PagedDataSource source = new PagedDataSource
            {
                DataSource = table.DefaultView,
                AllowPaging = true,
                PageSize = base.PageSize * 5
            };
            int result = 0;
            if (base.Request.QueryString["page"] != null)
            {
                int.TryParse(base.Request.QueryString["page"], out result);
            }
            source.CurrentPageIndex = result;
            this.pagerString = Various.GetPaginationString(result, source.PageCount, "allusers.aspx?order=" + str2 + "&q=" + base.Server.UrlEncode((username == null) ? "" : username));
            this.rptUsersList.DataSource = source;
            this.rptUsersList.DataBind();
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            foreach (string str in base.Request.Form.Keys)
            {
                int num;
                if (str.StartsWith("cbDel") && int.TryParse(str.Substring(5), out num))
                {
                    ForumTP.Utility.User.DeleteUser(num);
                }
            }
            this.BindRepeater(null);
        }

        protected void btnDisable_Click(object sender, EventArgs e)
        {
            foreach (string str in base.Request.Form.Keys)
            {
                if (str.StartsWith("cbDel"))
                {
                    int userId = Convert.ToInt32(str.Substring(5));
                    if (!this._showDisabled)
                    {
                        ForumTP.Utility.User.DisableUser(userId);
                    }
                    else
                    {
                        ForumTP.Utility.User.EnableUser(userId, false);
                    }
                }
            }
            this.BindRepeater(null);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (base.CurrentUserID == 0)
            {
                base.Response.Redirect("default.aspx", true);
            }
            else
            {
                this._showDisabled = base.Request.QueryString["Disabled"] == "1";
                ForumPage.AssignButtonTextboxEnterKey(this.tbUsername, "btnSearch");
                this.btnDel.Visible = this.btnDisable.Visible = this.lnkAdd.Visible = base.IsAdministrator;
                this.spanNonActive.Visible = base.IsAdministrator && !this._showDisabled;
                this.spanActive.Visible = base.IsAdministrator && this._showDisabled;
                if (this._showDisabled)
                {
                    this.btnDisable.ToolTip = "RE-ENABLE selected users";
                }
                if ((base.Request.QueryString["q"] == null) || (base.Request.QueryString["q"].Length == 0))
                {
                    this.BindRepeater(null);
                }
                else
                {
                    this.BindRepeater(base.Server.UrlDecode(base.Request.QueryString["q"]));
                }
            }
        }

        protected string ShowEmail(object email)
        {
            if (!base.IsAdministrator)
            {
                return "******";
            }
            return email.ToString();
        }
    }
}