using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ForumTP.Utility;

namespace ForumTP
{
    public partial class activate : ForumPage
    {
        protected Label lblError;
        protected Label lblSuccess;

        protected void Page_Load(object sender, EventArgs e)
        {
            string str = base.Request.QueryString["user"];
            string str2 = base.Request.QueryString["code"];
            if ((str == null) || (str2 == null))
            {
                base.Response.End();
            }
            else
            {
                base.Cn.Open();
                object obj2 = base.Cn.ExecuteScalar("select UserID from ForumUsers WHERE UserName=? AND ActivationCode=?", new object[] { str, str2 });
                base.Cn.Close();
                if (obj2 != null)
                {
                    ForumTP.Utility.User.EnableUser(Convert.ToInt32(obj2), false);
                    this.lblSuccess.Visible = true;
                    this.lblError.Visible = false;
                }
                else
                {
                    this.lblError.Visible = true;
                    this.lblSuccess.Visible = false;
                }
            }
        }
    }
}