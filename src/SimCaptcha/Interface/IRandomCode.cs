using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Interface
{
    public interface IRandomCode
    {
        public string Create(int number);
    }
}
