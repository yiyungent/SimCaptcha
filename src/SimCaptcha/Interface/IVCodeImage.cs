using SimCaptcha.Models;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Interface
{
    public interface IVCodeImage
    {
        VCodeImgModel Create(string code, int width, int height);
    }
}
