using SimCaptcha.Interface;
using System;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.AspNetCore
{
    public class ConsoleLogHelper : ILogHelper
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
