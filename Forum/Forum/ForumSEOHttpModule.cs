using System;
using System.IO;
using System.Web;

namespace ForumTP
{
    public class ForumSEOHttpModule : IHttpModule
    {
        private static bool _moduleLoaded;

        private void context_PostResolveRequestCache(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            if (!File.Exists(application.Request.PhysicalPath))
            {
                string str = application.Request.Path.ToLower();
                if (str.EndsWith(".aspx"))
                {
                    int index = str.IndexOf("/topic", str.LastIndexOf("/"));
                    int length = str.IndexOf("/forum", str.LastIndexOf("/"));
                    if (index > -1)
                    {
                        string str2 = str.Substring(0, index);
                        string s = str.Substring(index).Replace("/topic", "");
                        int num3 = s.IndexOf("-");
                        if (num3 >= 0)
                        {
                            s = s.Substring(0, num3);
                            int result = 0;
                            if (int.TryParse(s, out result))
                            {
                                string path = str2 + "/messages.aspx?TopicID=" + s + "&" + application.Request.QueryString.ToString();
                                context.RewritePath(path);
                            }
                        }
                    }
                    else if (length > -1)
                    {
                        string str5 = str.Substring(0, length);
                        string str6 = str.Substring(length).Replace("/forum", "");
                        int num5 = str6.IndexOf("-");
                        if (num5 >= 0)
                        {
                            str6 = str6.Substring(0, num5);
                            int num6 = 0;
                            if (int.TryParse(str6, out num6))
                            {
                                string str7 = str5 + "/topics.aspx?ForumID=" + str6 + "&" + application.Request.QueryString.ToString();
                                context.RewritePath(str7);
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            _moduleLoaded = true;
            context.PostResolveRequestCache += new EventHandler(this.context_PostResolveRequestCache);
        }

        public static bool SEOUrlsEnabled
        {
            get
            {
                return _moduleLoaded;
            }
        }
    }
}
