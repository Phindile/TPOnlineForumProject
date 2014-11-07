using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ForumTP
{
    public partial class admin : ForumPage
    {
        protected Button btnAdd;
        protected DropDownList ddlForumGroup;
        private const int DEL_COLUMN_INDEX = 4;
        protected THDataGrid gridForums;
        protected HtmlGenericControl lblEnterGroup;
        protected Label lblError;
        protected Label lblNoForums;
        protected HtmlGenericControl lblSelectGroup;
        protected HtmlAnchor lnkEditForumGroups;
        protected TextBox tbDescr;
        protected TextBox tbForumGroup;
        protected TextBox tbTitle;

        private void BindForumGroups()
        {
            base.Cn.Open();
            DbDataReader reader = base.Cn.ExecuteReader("SELECT * FROM ForumGroups", new object[0]);
            this.ddlForumGroup.DataSource = reader;
            this.ddlForumGroup.DataBind();
            reader.Close();
            base.Cn.Close();
            this.lblSelectGroup.Visible = this.lnkEditForumGroups.Visible = this.ddlForumGroup.Visible = this.lblEnterGroup.Visible = this.ddlForumGroup.Items.Count > 0;
        }

        private void BindForums()
        {
            base.Cn.Open();
            DbDataReader reader = base.Cn.ExecuteReader("SELECT * FROM Forums ORDER BY OrderByNumber", new object[0]);
            this.gridForums.DataSource = reader;
            this.gridForums.DataBind();
            reader.Close();
            base.Cn.Close();
            this.lblNoForums.Visible = this.gridForums.Items.Count == 0;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            this.lblError.Visible = false;
            if (((this.tbForumGroup.Text == "") && (this.ddlForumGroup.Items.Count == 0)) || ((this.tbTitle.Text.Trim() == "") || (this.tbDescr.Text.Trim() == "")))
            {
                this.lblError.Visible = true;
                this.BindForumGroups();
            }
            else
            {
                int forumGroupId = 0;
                if (this.tbForumGroup.Text.Trim() != "")
                {
                    forumGroupId = Forum.AddForumGroup(this.tbForumGroup.Text);
                }
                else
                {
                    forumGroupId = int.Parse(this.ddlForumGroup.SelectedValue);
                }
                Forum.AddForum(this.tbTitle.Text.Trim(), this.tbDescr.Text.Trim(), forumGroupId);
                this.BindForums();
                this.BindForumGroups();
                this.tbDescr.Text = "";
                this.tbTitle.Text = "";
                this.tbForumGroup.Text = "";
            }
        }

        protected void gridForums_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                Forum.DeleteForum(int.Parse(e.Item.Cells[0].Text));
                this.BindForums();
            }
            else if ((e.CommandName == "up") || (e.CommandName == "down"))
            {
                this.SaveCurrentOrderOfSectinsCategories();
                string text = e.Item.Cells[0].Text;
                if ((e.CommandName == "up") && (e.Item.ItemIndex > 0))
                {
                    DataGridItem item = this.gridForums.Items[e.Item.ItemIndex - 1];
                    if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
                    {
                        string str3 = item.Cells[0].Text;
                        base.Cn.Open();
                        base.Cn.ExecuteNonQuery("UPDATE Forums SET OrderByNumber = OrderByNumber-1 WHERE ForumID=?", new object[] { text });
                        base.Cn.ExecuteNonQuery("UPDATE Forums SET OrderByNumber = OrderByNumber+1 WHERE ForumID=?", new object[] { str3 });
                        base.Cn.Close();
                        this.BindForums();
                    }
                }
                if ((e.CommandName == "down") && (e.Item.ItemIndex < (this.gridForums.Items.Count - 1)))
                {
                    DataGridItem item2 = this.gridForums.Items[e.Item.ItemIndex + 1];
                    if ((item2.ItemType == ListItemType.Item) || (item2.ItemType == ListItemType.AlternatingItem))
                    {
                        string str4 = item2.Cells[0].Text;
                        base.Cn.Open();
                        base.Cn.ExecuteNonQuery("UPDATE Forums SET OrderByNumber = OrderByNumber+1\tWHERE ForumID=?", new object[] { text });
                        base.Cn.ExecuteNonQuery("UPDATE Forums SET OrderByNumber = OrderByNumber-1 WHERE ForumID=?", new object[] { str4 });
                        base.Cn.Close();
                        this.BindForums();
                    }
                }
            }
        }

        protected void gridForums_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                WebControl control = e.Item.Cells[4].Controls[0] as WebControl;
                if (control != null)
                {
                    control.Attributes.Add("onclick", "return confirm('Delete?');");
                }
            }
            if (e.Item.ItemType == ListItemType.Header)
            {
                e.Item.Cells[1].ColumnSpan = 4;
                e.Item.Cells[2].Visible = false;
                e.Item.Cells[3].Visible = false;
                e.Item.Cells[4].Visible = false;
            }
        }

        private bool IsNewVersionAvailable()
        {
            if (base.Cache["newVer"] == null)
            {
                bool flag;
                try
                {
                    Stream stream = new WebClient().OpenRead("#");
                    string str = new StreamReader(stream).ReadToEnd();
                    stream.Close();
                    flag = !Assembly.GetExecutingAssembly().GetName().Version.ToString().StartsWith(str);
                }
                catch
                {
                    flag = false;
                }
                base.Cache.Add("newVer", flag, null, DateTime.Now.AddDays(1.0), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            return (bool)base.Cache["newVer"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.btnAdd.DataBind();
            this.BindForums();
            if (!base.IsPostBack)
            {
                this.BindForumGroups();
            }
        }

        private void SaveCurrentOrderOfSectinsCategories()
        {
            base.Cn.Open();
            foreach (DataGridItem item in this.gridForums.Items)
            {
                if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
                {
                    base.Cn.ExecuteNonQuery("UPDATE Forums SET OrderByNumber = ? WHERE ForumID=?", new object[] { item.ItemIndex, item.Cells[0].Text });
                }
            }
            base.Cn.Close();
        }
    }
}