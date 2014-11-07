using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ForumTP
{
    public partial class forums : ForumPage
    {
        protected HtmlGenericControl divNoForumsAdmin;
        protected HtmlGenericControl divRecent;
        protected HtmlGenericControl lblNoForums;
        protected recentposts1 recentPosts;
        protected Repeater rptGroupsList;

        public static string GetForumIcon(object iconFile)
        {
            if (iconFile != null)
            {
                string str = iconFile.ToString();
                if (!(str == ""))
                {
                    return ("getforumicon.ashx?icon=" + str);
                }
            }
            return "images/forum.png";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataSet forumsForFrontpage = Forum.GetForumsForFrontpage(base.CurrentUserID);
            this.rptGroupsList.DataSource = forumsForFrontpage.Tables[0];
            this.rptGroupsList.DataBind();
            bool flag = this.rptGroupsList.Items.Count == 0;
            this.rptGroupsList.Visible = !flag;
            this.lblNoForums.Visible = flag && !base.IsAdministrator;
            this.divNoForumsAdmin.Visible = flag && base.IsAdministrator;
            this.divRecent.Visible = this.recentPosts.Visible = Settings.ShowRecentPostsOnHomepage;
        }

        protected void rptGroupsList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                Repeater repeater = e.Item.FindControl("rptForumsList") as Repeater;
                DataView view2 = (e.Item.DataItem as DataRowView).CreateChildView("ForumGroupsForums");
                if (view2.Count == 0)
                {
                    e.Item.Visible = false;
                }
                else
                {
                    repeater.DataSource = view2;
                    repeater.DataBind();
                }
            }
        }
    }
}