using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ForumTP
{
    public partial class adminonlineusers : System.Web.UI.Page
    {
        protected Repeater rptUsers;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.rptUsers.DataSource = ForumTP.Utility.User.OnlineUsersSessions.Values;
            this.rptUsers.DataBind();
        }
    }
}