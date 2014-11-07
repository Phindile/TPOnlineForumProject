using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForumTP
{
    /// <summary>
    /// Summary description for messagesajax
    /// </summary>
    public class messagesajax : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            if (request.Form["Mode"] == "Rate")
            {
                int score = int.Parse(request.Form["Score"]);
                int messageId = int.Parse(request.Form["MessageID"]);
                int currentUserID = ForumTP.Utility.User.CurrentUserID;
                if (currentUserID != 0)
                {
                    int? nullable = Message.RateMessage(messageId, currentUserID, score);
                    response.Write(nullable);
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