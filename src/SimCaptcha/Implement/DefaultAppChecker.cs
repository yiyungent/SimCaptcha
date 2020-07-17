using SimCaptcha.Interface;
using SimCaptcha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Implement
{
    public class DefaultAppChecker : IAppChecker
    {
        public ISimCaptchaOptions Options { get; set; }

        public DefaultAppChecker(SimCaptchaOptions options)
        {
            this.Options = options;
        }

        public AppCheckModel Check(string appId, string appSecret)
        {
            AppCheckModel rtnModel = new AppCheckModel();
            IList<AppItemModel> appList = null;
            try
            {
                appList = ((SimCaptchaOptions)Options).AppList;
            }
            catch (Exception ex)
            {
                throw new Exception("DefaultAppChecker 必需使用 SimCaptchaOptions(因为这里面有AppList), 如果你需要使用AppChecker, 请自行实现 IAppChecker");
            }
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
            IList<AppItemModel> appList = null;
            try
            {
                appList = ((SimCaptchaOptions)Options).AppList;
            }
            catch (Exception ex)
            {
                throw new Exception("DefaultAppChecker 必需使用 SimCaptchaOptions(因为这里面有AppList), 如果你需要使用AppChecker, 请自行实现 IAppChecker");
            }
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
