using SimCaptcha.Models;
using SimCaptcha.Models.Click;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Click
{
    public interface IClickVCodeImage
    {
        ClickVCodeImgModel Create(string code, int width, int height);
    }
}
