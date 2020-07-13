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
            IList<AppItemModel> appList = Options.AppList;
            if (appList == null)
            {
                return (false, "appId或appSecret不正确");
            }
            bool isExist = appList.Where(m => m.AppId == appId && m.AppSecret == appSecret)?.Count() >= 1;
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
