using SimCaptcha.Interface.Slider;
using SimCaptcha.Models.Slider;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace SimCaptcha.AspNetCore.Implement.Slider
{
    public class SliderVCodeImage : ISliderVCodeImage
    {
        public SliderVCodeImgModel Create()
        {
            SliderVCodeImgModel sliderVCodeImgModel = new SliderVCodeImgModel();
            string bgImagesDir = Path.Combine(Environment.CurrentDirectory, "SimCaptcha", "slider");
            string[] bgImagesFiles = System.IO.Directory.GetFiles(bgImagesDir);

            if (bgImagesFiles == null || bgImagesFiles.Length == 0)
            {
                Console.WriteLine("SimCaptcha/slider 需放置背景图片");
                throw new Exception("SimCaptcha/slider 需放置背景图片");
            }

            var path = DrawPath();



            return sliderVCodeImgModel;
        }


        private GraphicsPath DrawPath()
        {
            int x = 100;

            GraphicsPath path = new GraphicsPath(FillMode.Alternate);
            path.AddLine(0, 0, 3 * x, 0);
            path.AddLine(3 * x, 0, 3 * x, x);
            path.AddArc(new RectangleF(2.5F * x, x, x, x), 90, 180);
            path.AddLine(3 * x, 2 * x, 3 * x, 3 * x);
            path.AddArc(new RectangleF(x, 2.5F * x, x, x), 0, 180);
            path.AddLine(x, 3 * x, 0, 3 * x);
            path.AddLine(0, 3 * x, 0, 0);

            return path;
        }

        private string CutPath(Image image, GraphicsPath path)
        {
            Bitmap bitmap = new Bitmap((int)path.GetBounds().Width, (int)path.GetBounds().Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (TextureBrush textureBrush = new TextureBrush(image, WrapMode.Clamp, path.GetBounds()))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.FillPath(textureBrush, path);
                }
            }

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            string imgBase64 = "data:image/png;base64," + Convert.ToBase64String(ms.GetBuffer());
            bitmap.Dispose();
            ms.Dispose();

            return imgBase64;
        }

        private string FillPath(Image image, GraphicsPath path)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(image, 0, 0);
                graphics.FillPath(new SolidBrush(Color.FromArgb(120, 0, 0, 0)), path);
            }

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            string imgBase64 = "data:image/png;base64," + Convert.ToBase64String(ms.GetBuffer());
            bitmap.Dispose();
            ms.Dispose();

            return imgBase64;
        }



    }
}
