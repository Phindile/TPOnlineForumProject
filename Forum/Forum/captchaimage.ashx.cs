using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ForumTP
{
    /// <summary>
    /// Summary description for captchaimage
    /// </summary>
    public class captchaimage : IHttpHandler, IReadOnlySessionState, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            HttpSessionState session = HttpContext.Current.Session;
            HttpResponse response = HttpContext.Current.Response;
            if (session["CaptchaImageText"] != null)
            {
                CaptchaImage image = new CaptchaImage(session["CaptchaImageText"].ToString(), 200, 50);
                response.Clear();
                response.ContentType = "image/jpeg";
                image.Image.Save(response.OutputStream, ImageFormat.Jpeg);
                image.Dispose();
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