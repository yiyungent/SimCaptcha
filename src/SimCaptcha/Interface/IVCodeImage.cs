using SimCaptcha.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Interface
{
    public interface IVCodeImage
    {
        VCodeImgModel Create(string code, int width, int height);
    }
}
