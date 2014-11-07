using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForumTP
{
    /// <summary>
    /// Summary description for renewsession
    /// </summary>
    public class renewsession : IHttpHandler
    {

        private byte[] gif = new byte[] { 
            0x47, 0x49, 70, 0x38, 0x39, 0x61, 1, 0, 1, 0, 0x91, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0x21, 0xf9, 4, 9, 0, 0, 0, 
            0, 0x2c, 0, 0, 0, 0, 1, 0, 1, 0, 0, 8, 4, 0, 1, 4, 
            4, 0, 0x3b, 0
         };

        public void ProcessRequest(HttpContext context)
        {
            object obj1 = context.Session["ForumUserID"];
            context.Response.AddHeader("ContentType", "image/gif");
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.BinaryWrite(this.gif);
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