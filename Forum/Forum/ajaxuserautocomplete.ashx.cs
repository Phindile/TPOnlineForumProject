using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace ForumTP
{
    /// <summary>
    /// Summary description for ajaxuserautocomplete
    /// </summary>
    public class ajaxuserautocomplete : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string[] strArray = new string[] { ";", "--", "/*", @"*\", "xp_" };
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            response.Expires = -1;
            string str = request.QueryString["q"].Replace("'", "");
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Trim();
                if (str.Length >= 1)
                {
                    foreach (string str2 in strArray)
                    {
                        if (request.QueryString["q"].Contains(str2))
                        {
                            response.TrySkipIisCustomErrors = true;
                            response.StatusCode = 400;
                            response.Write("Bad request");
                            response.End();
                            return;
                        }
                    }
                    using (DbConnection connection = DataAccessUtil.CreateOpenConnection())
                    {
                        DbDataReader reader = connection.ExecuteReader("SELECT TOP 20 UserName, UserID FROM ForumUsers\r\n\t\t\t\t\tWHERE UserName LIKE '" + str + "%' AND Disabled=? ORDER BY UserName", new object[] { false });
                        while (reader.Read())
                        {
                            response.Write(string.Concat(new object[] { reader["UserName"], "|", reader["UserID"], "\r\n" }));
                        }
                        reader.Close();
                    }
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}