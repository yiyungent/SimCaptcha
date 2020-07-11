using System;
using System.Collections.Generic;
using System.Text;

namespace SimCaptcha.Models
{
    public class VCodeCheckResponseModel
    {
        /// <summary>
        /// <para>0 正确 -> 下发票据 ticket</para>
        /// <para>-1 错误 -> 验证码错误 且 错误次数未达上限 -> message: 点错啦，请重试</para>
        /// <para>-2 错误 -> 验证码错误 且 错误次数已达上限 -> message: 这题有点难，为你换一个试试吧</para>
        /// <para>-3 错误 -> 验证码无效(被篡改), 获取新验证码</para>
        /// <para>-4 错误 -> 验证码无效(过期), 获取新验证码</para>
        /// </summary>
        public int code { get; set; }

        public string message { get; set; }

        /// <summary>
        /// 第一个string
        /// </summary>
        public DataModel data { get; set; }

        public class DataModel
        {
            public string appId { get; set; }

            /// <summary>
            /// (先被json字符串化，再被AES加密后的) 票据字符串
            /// </summary>
            public string ticket { get; set; }

            /// <summary>
            /// 仅当错误次数未达上限，允许再次尝试时有值
            /// </summary>
            public string vCodeKey { get; set; }
        }
    }
}
