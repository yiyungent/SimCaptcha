using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Models
{
    public class TicketModel
    {
        /// <summary>
        /// 是否验证通过
        /// </summary>
        public bool IsPass { get; set; }
        /// <summary>
        /// 下发票据时间（创建时间）
        /// <para>js 13位 毫秒时间戳</para>
        /// </summary>
        public long TS { get; set; }

        /// <summary>
        /// 下发给目标客户端的IP地址
        /// </summary>
        public string IP { get; set; }
    }
}
