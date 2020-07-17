using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimCaptcha.Interface
{
    /// <summary>
    /// 错误日志记录(非必需,可能为null)
    /// </summary>
    public interface ILogHelper
    {
        void Write(string message);
    }
}
