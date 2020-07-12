using SimCaptcha.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimCaptcha.Implement
{
    public class RandomCodeHanZi : IRandomCode
    {
        public string Create(int number)
        {
            var str = "天地玄黄宇宙洪荒日月盈昃辰宿列张寒来暑往秋收冬藏闰余成岁律吕调阳云腾致雨露结为霜金生丽水玉出昆冈剑号巨阙珠称夜光果珍李柰菜重芥姜海咸河淡鳞潜羽翔龙师火帝鸟官人皇始制文字乃服衣裳推位让国有虞陶唐吊民伐罪周发殷汤坐朝问道垂拱平章爱育黎首臣伏戎羌遐迩体率宾归王";
            char[] str_char_arrary = str.ToArray();
            Random rand = new Random();
            HashSet<string> hs = new HashSet<string>();
            bool randomBool = true;
            while (randomBool)
            {
                if (hs.Count == number)
                    break;
                int rand_number = rand.Next(str_char_arrary.Length);
                hs.Add(str_char_arrary[rand_number].ToString());
            }
            string code = string.Join("", hs);
            return code;
        }
    }
}
