using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ForumTP.Utility;

namespace ForumTP
{
    public partial class adduser : ForumPage
    {
        protected Button btnAdd;
        protected Label lblError;
        protected Label lblSuccess;
        protected RequiredFieldValidator RequiredFieldValidator1;
        protected RequiredFieldValidator RequiredFieldValidator2;
        protected RequiredFieldValidator RequiredFieldValidator3;
        protected TextBox txEmail;
        protected TextBox txHomepage;
        protected TextBox txPsw;
        protected TextBox txUserName;

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (base.IsValid)
            {
                string userName = this.txUserName.Text.Trim();
                base.Cn.Open();
                if (base.Cn.ExecuteScalar("select UserID from ForumUsers WHERE UserName=?", new object[] { userName }) == null)
                {
                    if (base.Cn.ExecuteScalar("select UserID from ForumUsers WHERE Email=?", new object[] { this.txEmail.Text }) == null)
                    {
                        int num = ForumTP.Utility.User.CreateUser(userName, this.txEmail.Text, Password.CalculateHash(this.txPsw.Text), this.txHomepage.Text, string.Empty, false, "", "", "", "", "", "");
                        this.lblError.Visible = false;
                        this.lblSuccess.Visible = true;
                        base.Response.Redirect("viewprofile.aspx?UserID=" + num);
                    }
                    else
                    {
                        this.lblError.Text = "Email address already exists!";
                        this.lblError.Visible = true;
                        this.lblSuccess.Visible = false;
                    }
                }
                else
                {
                    this.lblError.Text = "User already exists!";
                    this.lblError.Visible = true;
                    this.lblSuccess.Visible = false;
                }
                base.Cn.Close();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.btnAdd.DataBind();
        }
    }
}