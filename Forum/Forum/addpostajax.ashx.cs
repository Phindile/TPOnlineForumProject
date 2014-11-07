using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ForumTP
{
    /// <summary>
    /// Summary description for addpostajax
    /// </summary>
    public class addpostajax : IHttpHandler, IReadOnlySessionState, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            if (request.Form["mode"] == "preview")
            {
                string input = request.Form["messagetext"];
                input = input.Replace("<", "&lt;").Replace(">", "&gt;");
                response.Write(Formatting.FormatMessageHTML(input));
                response.End();
            }
            if (request.Form["mode"] == "delfile")
            {
                int result = 0;
                if (int.TryParse(request.Form["FileID"], out result))
                {
                    Attachments.DeleteMessageAttachmentById(result);
                }
                response.End();
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