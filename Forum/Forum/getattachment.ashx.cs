using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ForumTP
{
    /// <summary>
    /// Summary description for getattachment
    /// </summary>
    public class getattachment : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            int result = 0;
            int userId = 0;
            string fileName = "";
            int.TryParse(request.QueryString["FileID"], out result);
            bool isPrivateMessage = request.QueryString["personal"] != null;
            try
            {
                Attachments.GetAttachment(result, isPrivateMessage, out userId, out fileName);
                string path = Attachments.GetUploadDirAbsolutePath() + userId.ToString() + @"\" + fileName;
                if (!File.Exists(path))
                {
                    path = Attachments.GetUploadDirAbsolutePathOLDVersion() + userId.ToString() + @"\" + fileName;
                }
                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    response.Clear();
                    response.ContentType = "application/octet-stream";
                    response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\";");
                    response.AddHeader("Content-Length", info.Length.ToString());
                    response.TransmitFile(path);
                }
                else
                {
                    response.Clear();
                    response.TrySkipIisCustomErrors = true;
                    response.StatusCode = 0x194;
                }
            }
            catch (AccessViolationException)
            {
                response.Write("Access denied. No permission.");
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