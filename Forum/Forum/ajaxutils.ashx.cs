using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ForumTP
{
    /// <summary>
    /// Summary description for ajaxutils1
    /// </summary>
    public class ajaxutils : IHttpHandler, IReadOnlySessionState, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            switch (request.Form["mode"])
            {
                case "CheckUserNameAvailability":
                    {
                        int userIdByUserName = 1;
                        if (!string.IsNullOrEmpty(request.Form["username"]))
                        {
                            userIdByUserName = ForumTP.Utility.User.GetUserIdByUserName(request.Form["username"]);
                        }
                        response.Write((userIdByUserName == 0) ? "0" : "1");
                        return;
                    }
                case "HideTag":
                    context.Session["HideTag"] = DateTime.Now;
                    break;
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