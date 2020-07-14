using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Interface
{
    public interface IRandomCode
    {
        string Create(int number);
    }
}
