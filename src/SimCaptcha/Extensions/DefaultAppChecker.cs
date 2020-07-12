using SimCaptcha.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimCaptcha.Extensions
{
    public class DefaultAppChecker : IAppChecker
    {
        public SimCaptchaOptions Options { get; set; }

        public DefaultAppChecker(SimCaptchaOptions options)
        {
            this.Options = options;
        }

        public (bool, string) Check(string appId, string appSecret)
        {
            bool isExist = Options.AppList?.Contains(new AppItemModel { AppId = appId, AppSecret = appSecret }) ?? false;
            if (!isExist)
            {
                return (false, "appId或appSecret不正确");
            }
            return (true, "appId和appSecret效验通过");
        }

        public (bool, string) CheckAppId(string appId)
        {
            IList<AppItemModel> appList = Options.AppList;
            bool isExist = appList?.Select(m => m.AppId).Contains(appId) ?? false;
            if (!isExist)
            {
                return (false, "appId 不存在");
            }
            return (true, "appId 效验通过");
        }
    }
}
