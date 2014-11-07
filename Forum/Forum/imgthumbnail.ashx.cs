using ForumTP.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace ForumTP
{
    /// <summary>
    /// Summary description for imgthumbnail
    /// </summary>
    public class imgthumbnail : IHttpHandler
    {

        private static Bitmap CreateThumbnail(string lcFilename, int lnWidth, int lnHeight)
        {
            Bitmap image = null;
            try
            {
                Bitmap bitmap2 = new Bitmap(lcFilename);
                ImageFormat rawFormat = bitmap2.RawFormat;
                int width = 0;
                int height = 0;
                if ((bitmap2.Width < lnWidth) && (bitmap2.Height < lnHeight))
                {
                    return bitmap2;
                }
                decimal num = lnHeight / bitmap2.Height;
                height = lnHeight;
                decimal num4 = bitmap2.Width * num;
                width = (int)num4;
                image = new Bitmap(width, height);
                Graphics graphics = Graphics.FromImage(image);
                graphics.InterpolationMode = InterpolationMode.Bicubic;
                graphics.FillRectangle(Brushes.White, 0, 0, width, height);
                graphics.DrawImage(bitmap2, 0, 0, width, height);
                bitmap2.Dispose();
            }
            catch
            {
                return null;
            }
            return image;
        }

        private void ErrorResult(HttpResponse response)
        {
            response.Clear();
            response.TrySkipIisCustomErrors = true;
            response.StatusCode = 0x194;
            response.End();
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            string str = request.QueryString["Image"];
            if (str == null)
            {
                this.ErrorResult(response);
            }
            else
            {
                string s = request["Size"];
                int lnWidth = 0x40;
                if (s != null)
                {
                    lnWidth = int.Parse(s);
                }
                string path = Attachments.GetUploadDirAbsolutePath() + str;
                if (!File.Exists(path))
                {
                    path = context.Server.MapPath(request.Path);
                    path = path.Substring(0, path.IndexOf("imgthumbnail.ashx")) + @"upload\" + str;
                }
                Bitmap bitmap = CreateThumbnail(path, lnWidth, lnWidth);
                if (bitmap == null)
                {
                    this.ErrorResult(response);
                }
                else
                {
                    response.ContentType = "image/jpeg";
                    bitmap.Save(response.OutputStream, ImageFormat.Jpeg);
                    bitmap.Dispose();
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