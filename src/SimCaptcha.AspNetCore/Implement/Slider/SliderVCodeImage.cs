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

            int imgIndex = new Random().Next(bgImagesFiles.Length);
            string randomImgFile = bgImagesFiles[imgIndex];
            Image image = Image.FromFile(randomImgFile);

            GraphicsPath path = DrawPath(image, out int x, out int y);
            sliderVCodeImgModel.BgImg = BitmapToBase64(FillPath(image, path, null));
            sliderVCodeImgModel.SliderImg = BitmapToBase64(CutPath(image, path, null));
            sliderVCodeImgModel.VCodePos = (x, y);
            sliderVCodeImgModel.VCodeTip = "拖动滑块完成拼图";

            return sliderVCodeImgModel;
        }


        protected GraphicsPath DrawPath(Image image, out int x, out int y)
        {
            // 滑块: 正方形: 高=宽
            int s = 100;
            // 滑块: 左上角点 位置
            x = new Random().Next(image.Width / 6, image.Width - s);
            y = new Random().Next(image.Height / 6, image.Height - s);

            GraphicsPath path = new GraphicsPath(FillMode.Alternate);
            path.AddLine(x + 0, y + 0, x + 3 * s, y + 0);
            path.AddLine(x + 3 * s, y + 0, x + 3 * s, y + s);
            path.AddArc(new RectangleF(x + 2.5F * s, y + s, s, s), 90, 180);
            path.AddLine(x + 3 * s, y + 2 * s, x + 3 * s, y + 3 * s);
            path.AddArc(new RectangleF(x + s, y + 2.5F * s, s, s), 0, 180);
            path.AddLine(x + s, y + 3 * s, x + 0, y + 3 * s);
            path.AddLine(x + 0, y + 3 * s, x + 0, y + 0);

            return path;
        }

        protected Bitmap CutPath(Image image, GraphicsPath path, string savePath)
        {
            var bounds = path.GetBounds();
            var rectangle = new RectangleF(0, 0, bounds.X + bounds.Width, bounds.Y + bounds.Height);

            Bitmap bitmap = new Bitmap((int)bounds.Width, (int)bounds.Height);
            using (Bitmap bitBitmap = new Bitmap((int)(bounds.X + bounds.Width), (int)(bounds.Y + bounds.Height)))
            {
                using (Graphics bigGraphics = Graphics.FromImage(bitBitmap))
                {
                    using (TextureBrush bigTextureBrush = new TextureBrush(image, WrapMode.Clamp, rectangle))
                    {
                        bigGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                        bigGraphics.FillPath(bigTextureBrush, path);

                        using (Graphics transGraphics = Graphics.FromImage(bitmap))
                        {
                            transGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                            transGraphics.DrawImage(bitBitmap, -bounds.X, -bounds.Y);
                        }
                    }
                }
            }


            if (!string.IsNullOrEmpty(savePath)) { bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Png); }
            return bitmap;
        }

        protected Bitmap FillPath(Image image, GraphicsPath path, string savePath)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (TextureBrush bigTextureBrush = new TextureBrush(image, WrapMode.Clamp))
                {
                    graphics.FillRectangle(bigTextureBrush, 0, 0, image.Width, image.Height);
                    graphics.FillPath(new SolidBrush(Color.FromArgb(128, 255, 255, 255)), path);
                }
            }

            if (!string.IsNullOrEmpty(savePath)) { bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Png); }
            return bitmap;
        }


        protected string BitmapToBase64(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            string rtnStr = "data:image/png;base64," + Convert.ToBase64String(ms.GetBuffer());
            //bitmap.Dispose(); // 注意: 不要释放, 会使用两次
            ms.Dispose();

            return rtnStr;
        }

    }
}
