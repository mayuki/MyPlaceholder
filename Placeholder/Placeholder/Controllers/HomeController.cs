using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace Placeholder.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 60*60, VaryByParam = "*")]
        public ActionResult Render(Int32 width, Int32? height, String backgroundColor, String foregroundColor, String text)
        {
            var imageData = GetImage(width, height ?? width, backgroundColor, foregroundColor, text);
            return File(imageData, "image/png");
        }

        private Byte[] GetImage(Int32 width, Int32 height, String backgroundColor, String foregroundColor, String text)
        {
            // Size
            width = Math.Max(Math.Min(3000, width), 1);
            height = Math.Max(Math.Min(3000, height), 1);

            // Color
            Color color = Color.FromArgb(0xf0, 0xf0, 0xf0);
            Color textColor = Color.FromArgb(0xc0, 0xc0, 0xc0);
            if (Regex.IsMatch(backgroundColor ?? "", "^[0-9a-f]{1,8}$"))
            {
                color = Color.FromArgb((Int32)((backgroundColor.Length == 6) ? 0xff000000 : 0x0) |
                                        Int32.Parse(backgroundColor, NumberStyles.HexNumber));
            }
            if (Regex.IsMatch(foregroundColor ?? "", "^[0-9a-f]{1,8}$"))
            {
                textColor = Color.FromArgb((Int32)((foregroundColor.Length == 6) ? 0xff000000 : 0x0) |
                                        Int32.Parse(foregroundColor, NumberStyles.HexNumber));
            }

            // Label
            String label = text ?? String.Format("{0}x{1}", width, height, color);

            // Cache
            TimeSpan cacheSlidingExpiration = TimeSpan.FromMinutes(5);
            String cacheKey = String.Format("Placeholder.Cache.{0}x{1}:{2}:{3}:{4}", width, height, color, textColor, label);

            // Image Data
            Byte[] imageData = HttpContext.Cache[cacheKey] as Byte[];
            if (imageData == null)
            {
                // Create Image Data (Cache miss)
                MemoryStream memStream = new MemoryStream();
                using (Font font = new Font("Verdana", Math.Min(height / 3, width / label.Length), FontStyle.Bold, GraphicsUnit.Pixel))
                using (Pen pen = new Pen(textColor))
                using (Brush brush = new SolidBrush(textColor))
                using (Image image = new Bitmap(width, height))
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.Clear(color);
                    SizeF size = g.MeasureString(label, font);
                    //g.DrawRectangle(pen, 0, 0, width-1, height-1);
                    g.DrawString(label, font, brush, width / 2 - size.Width / 2, height / 2 - size.Height / 2);
                    image.Save(memStream, ImageFormat.Png);
                }
                imageData = memStream.ToArray();
                HttpContext.Cache.Remove(cacheKey);
                HttpContext.Cache.Add(cacheKey, imageData, null, Cache.NoAbsoluteExpiration, cacheSlidingExpiration, CacheItemPriority.Low, null);
            }

            return imageData;
        }
    }
}
