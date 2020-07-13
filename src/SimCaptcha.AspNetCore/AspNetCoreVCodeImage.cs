using SimCaptcha.Interface;
using SimCaptcha.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace SimCaptcha.AspNetCore
{
    public class AspNetCoreVCodeImage : IVCodeImage
    {
        public VCodeImgModel Create(string code, int width, int height)
        {
            VCodeImgModel rtnResult = new VCodeImgModel { VCodePos = new List<PointPosModel>() };

            // TODO: 变化点: 答案: 4个字
            int rightCodeLength = 4;

            Bitmap Img = null;
            Graphics g = null;
            MemoryStream ms = null;
            Random random = new Random();

            Color[] color_Array = { Color.Black, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            string[] fonts = { "lnk Free", "Segoe Print", "Comic Sans MS", "MV Boli", "华文行楷" };
            // TODO: 可能有错, 在windows, linux下 分割符不同
            string _base = Path.Combine(Environment.CurrentDirectory, "SimCaptcha", "bgImages");
            //string _base = Environment.CurrentDirectory + "\\SimCaptcha\\bgImages\\";

            var _file_List = System.IO.Directory.GetFiles(_base);
            int imageCount = _file_List.Length;
            if (imageCount == 0)
                throw new Exception("image not Null");

            int imageRandom = random.Next(1, (imageCount + 1));
            string _random_file_image = _file_List[imageRandom - 1];
            var imageStream = Image.FromFile(_random_file_image);

            Img = new Bitmap(imageStream, width, height);
            imageStream.Dispose();
            g = Graphics.FromImage(Img);
            Color[] penColor = { Color.LightGray, Color.Green, Color.Blue };
            int code_length = code.Length;
            List<string> words = new List<string>();
            for (int i = 0; i < code_length; i++)
            {
                int cindex = random.Next(color_Array.Length);
                int findex = random.Next(fonts.Length);
                Font f = new Font(fonts[findex], 15, FontStyle.Bold);
                Brush b = new SolidBrush(color_Array[cindex]);
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
            Img.Save(ms, ImageFormat.Jpeg);
            g.Dispose();
            Img.Dispose();
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
        private (int, int) ToPercentPos((int, int) widthAndHeight, (int, int) xAndy)
        {
            (int, int) rtnResult = (0, 0);
            // 注意: int / int = int (小数部分会被截断)
            rtnResult.Item1 = (int)(((double)xAndy.Item1) / ((double)widthAndHeight.Item1) * 100);
            rtnResult.Item2 = (int)(((double)xAndy.Item2) / ((double)widthAndHeight.Item2) * 100);

            return rtnResult;
        }
    }
}
