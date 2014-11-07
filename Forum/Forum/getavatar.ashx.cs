using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ForumTP
{
    /// <summary>
    /// Summary description for getavatar
    /// </summary>
    public class getavatar : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string safeFileNameFromQueryStirng = Various.GetSafeFileNameFromQueryStirng(HttpUtility.UrlDecode(context.Request["avatar"]));
            if (!string.IsNullOrEmpty(safeFileNameFromQueryStirng))
            {
                string path = Attachments.GetAvatarsDirAbsolutePath() + safeFileNameFromQueryStirng;
                if (!File.Exists(path))
                {
                    path = Attachments.GetUploadDirAbsolutePathOLDVersion() + safeFileNameFromQueryStirng;
                }
                if (File.Exists(path))
                {
                    HttpResponse response = context.Response;
                    response.Clear();
                    response.ContentType = Attachments.GetContentType(path);
                    response.AddHeader("Content-Disposition", "attachment; filename=\"" + safeFileNameFromQueryStirng + "\";");
                    response.AddHeader("Content-Length", new FileInfo(path).Length.ToString());
                    response.TransmitFile(path);
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}