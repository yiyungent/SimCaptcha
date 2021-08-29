using System;
using System.Collections.Generic;
using System.Text;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.ResponseModels
{
    public class VCodeCheckResponseModel
    {
        /// <summary>
        /// <para>0 正确 -> 下发票据 ticket</para>
        /// <para>-1 错误 -> 验证码错误 且 错误次数未达上限 -> message: 点错啦，请重试</para>
        /// <para>-2 错误 -> 验证码错误 且 错误次数已达上限 -> message: 这题有点难，为你换一个试试吧</para>
        /// <para>-3 错误 -> 验证码无效(被篡改导致解密失败), 获取新验证码</para>
        /// <para>-4 错误 -> 验证码无效(过期)(Cache里有,但它标识已过期), 获取新验证码</para>
        /// <para>-5 错误 -> 验证码无效(过期)(Cache无此vCodeKey,可能过期被自动删除), 获取新验证码</para>
        /// <para>-6 错误 -> appId 效验不通过 -> 不允许验证, 提示错误信息</para>
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

        }
    }
}
