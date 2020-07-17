// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Interface
{
    public interface ISimCaptchaOptions
    {
        string AppId { get; set; }

        string AppSecret { get; set; }

        string TicketVerifyUrl { get; set; }


        // ↑以上仅供业务后台使用
        // ↓以下仅供验证码服务端使用


        string EncryptKey { get; set; }

        /// <summary>
        /// 允许的错误次数，例如允许2次，那么错误2次后，还可以再次尝试check，但第3次错误后就不能再尝试了
        /// </summary>
        int AllowErrorNum { get; set; }

        /// <summary>
        /// <para>验证码在被创建多少秒后过期</para>
        /// <para>ticket在被创建多少秒后过期</para>
        /// <para>均使用此属性</para>
        /// </summary>
        int ExpiredSec { get; set; }
    }
}
