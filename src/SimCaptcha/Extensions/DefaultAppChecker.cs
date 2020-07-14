using SimCaptcha.Interface;
using SimCaptcha.Models;
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

        public AppCheckModel Check(string appId, string appSecret)
        {
            AppCheckModel rtnModel = new AppCheckModel();
            IList<AppItemModel> appList = Options.AppList;
            if (appList == null)
            {
                rtnModel.Pass = false;
                rtnModel.Message = "appId或appSecret不正确";
                return rtnModel;
            }
            bool isExist = appList.Where(m => m.AppId == appId && m.AppSecret == appSecret)?.Count() >= 1;
            if (!isExist)
            {
                rtnModel.Pass = false;
                rtnModel.Message = "appId或appSecret不正确";
                return rtnModel;
            }
            rtnModel.Pass = true;
            rtnModel.Message = "appId和appSecret效验通过";
            return rtnModel;
        }

        public AppCheckModel CheckAppId(string appId)
        {
            AppCheckModel rtnModel = new AppCheckModel();
            IList<AppItemModel> appList = Options.AppList;
            bool isExist = appList?.Select(m => m.AppId).Contains(appId) ?? false;
            if (!isExist)
            {
                rtnModel.Pass = false;
                rtnModel.Message = "appId 不存在";
                return rtnModel;
            }
            rtnModel.Pass = true;
            rtnModel.Message = "appId 效验通过";
            return rtnModel;
        }
    }
}
