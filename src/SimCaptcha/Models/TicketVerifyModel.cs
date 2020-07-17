using System;
using System.Collections.Generic;
using System.Text;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Models
{
    public class TicketVerifyModel
    {
        // string appId, string appSecret, string ticket, string userId, string userIp

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string  Ticket { get; set; }

        public string UserId { get; set; }

        public string UserIp { get; set; }

    }
}
