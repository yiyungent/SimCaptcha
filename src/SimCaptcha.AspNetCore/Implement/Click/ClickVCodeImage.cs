using SimCaptcha.Click;
using SimCaptcha.Interface;
using SimCaptcha.Models;
using SimCaptcha.Models.Click;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.AspNetCore
{
    public class ClickVCodeImage : IClickVCodeImage
    {
        public ClickVCodeImgModel Create(string code, int width, int height)
        {
            ClickVCodeImgModel rtnResult = new ClickVCodeImgModel { VCodePos = new List<PointPosModel>() };

            // TODO: 变化点: 答案: 4个字
            int rightCodeLength = 4;

            Bitmap bitmap = null;
            Graphics g = null;
            MemoryStream ms = null;
            Random random = new Random();

            Color[] colorArray = { Color.Black, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };

            string bgImagesDir = Path.Combine(Environment.CurrentDirectory, "SimCaptcha", "click");
            string[] bgImagesFiles = System.IO.Directory.GetFiles(bgImagesDir);

            if (bgImagesFiles == null || bgImagesFiles.Length == 0)
            {
                Console.WriteLine("SimCaptcha/click 需放置背景图片");
                throw new Exception("SimCaptcha/click 需放置背景图片");
            }

            // 字体来自：https://www.zcool.com.cn/special/zcoolfonts/
            string fontsDir = Path.Combine(Environment.CurrentDirectory, "SimCaptcha", "fonts");
            string[] fontFiles = new DirectoryInfo(fontsDir)?.GetFiles()
                ?.Where(m => m.Extension.ToLower() == ".ttf")
                ?.Select(m => m.FullName).ToArray();
            if (fontFiles == null || fontFiles.Length == 0)
            {
                Console.WriteLine("SimCaptcha/fonts 需放置字体文件 .ttf");
                throw new Exception("SimCaptcha/fonts 需放置字体文件 .ttf");
            }


            int imgIndex = random.Next(bgImagesFiles.Length);
            string randomImgFile = bgImagesFiles[imgIndex];
            var imageStream = Image.FromFile(randomImgFile);

            bitmap = new Bitmap(imageStream, width, height);
            imageStream.Dispose();
            g = Graphics.FromImage(bitmap);
            Color[] penColor = { Color.LightGray, Color.Green, Color.Blue };
            int code_length = code.Length;
            List<string> words = new List<string>();
            for (int i = 0; i < code_length; i++)
            {
                int colorIndex = random.Next(colorArray.Length);
                int fontIndex = random.Next(fontFiles.Length);
                Font f = LoadFont(fontFiles[fontIndex], 15, FontStyle.Bold);
                Brush b = new SolidBrush(colorArray[colorIndex]);
                int _y = random.Next(height);
                if (_y > (height - 30))
                    _y = _y - 60;

                int _x = width / (i + 1);
                if ((width - _x) < 50)
                {
                    _x = width - 60;
                }
                string word = code.Substring(i, 1);
                if (rtnResult.VCodePos.Count < rightCodeLength)
                {
                    (int, int) percentPos = ToPercentPos((width, height), (_x, _y));
                    // 添加正确答案 位置数据
                    rtnResult.VCodePos.Add(new PointPosModel()
                    {
                        X = percentPos.Item1,
                        Y = percentPos.Item2,
                    });
                    words.Add(word);
                }
                g.DrawString(word, f, b, _x, _y);
            }
            rtnResult.Words = words;
            rtnResult.VCodeTip = "请依次点击: " + string.Join(",", words);

            ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            g.Dispose();
            bitmap.Dispose();
            ms.Dispose();
            rtnResult.ImgBase64 = "data:image/jpg;base64," + Convert.ToBase64String(ms.GetBuffer());

            return rtnResult;
        }


        /// <summary>
        /// 转换为相对于图片的百分比单位
        /// </summary>
        /// <param name="widthAndHeight">图片宽高</param>
        /// <param name="xAndy">相对于图片的绝对尺寸</param>
        /// <returns>(int:xPercent, int:yPercent)</returns>
        protected (int, int) ToPercentPos((int, int) widthAndHeight, (int, int) xAndy)
        {
            (int, int) rtnResult = (0, 0);
            // 注意: int / int = int (小数部分会被截断)
            rtnResult.Item1 = (int)(((double)xAndy.Item1) / ((double)widthAndHeight.Item1) * 100);
            rtnResult.Item2 = (int)(((double)xAndy.Item2) / ((double)widthAndHeight.Item2) * 100);

            return rtnResult;
        }


        /// <summary>
        /// 加载字体
        /// </summary>
        /// <param name="path">字体文件路径,包含字体文件名和后缀名</param>
        /// <param name="size">大小</param>
        /// <param name="fontStyle">字形(常规/粗体/斜体/粗斜体)</param>
        protected Font LoadFont(string path, int size, FontStyle fontStyle)
        {
            try
            {
                System.Drawing.Text.PrivateFontCollection pfc = new System.Drawing.Text.PrivateFontCollection();

                pfc.AddFontFile(path);// 字体文件的路径

                Font myFont = new Font(pfc.Families[0], size, fontStyle);

                return myFont;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }


}
