using SimCaptcha.Common;
using SimCaptcha.Interface;
using SimCaptcha.Interface.Slider;
using SimCaptcha.Models;
using SimCaptcha.Models.Click;
using SimCaptcha.Models.Slider;
using SimCaptcha.ResponseModels;
using SimCaptcha.ResponseModels.Slider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimCaptcha.Implement.Slider
{
    public class SliderSimCaptchaService : SimCaptchaService
    {
        public ISliderVCodeImage VCodeImage { get; set; }

        public override string MessageReTry => "请正确拼合图形";


        #region Ctor

        public SliderSimCaptchaService(ISimCaptchaOptions options, ICacheHelper cacheHelper, IJsonHelper jsonHelper,
            IEncryptHelper encryptHelper, IAppChecker appChecker, ILogHelper logHelper,
            ISliderVCodeImage sliderVCodeImage)
            : base(options, cacheHelper, jsonHelper, encryptHelper, appChecker, logHelper)
        {
            this.VCodeImage = sliderVCodeImage;
        }

        #endregion

        public override Task<VCodeResponseModel> VCode()
        {
            SliderVCodeResponseModel rtnResult = new SliderVCodeResponseModel();

            try
            {
                SliderVCodeImgModel model = (SliderVCodeImgModel)CreateVCodeImg();
                string userId = Guid.NewGuid().ToString();
                rtnResult.code = 0;
                rtnResult.message = "获取验证码成功";
                rtnResult.data = new SliderVCodeResponseModel.DataModel
                {
                    userId = userId,
                    bgImg = model.BgImg,
                    sliderImg = model.SliderImg,
                    vCodeTip = model.VCodeTip,
                    captchaType = "slider"
                };
                // 生成 vCodeKey: 转为json字符串 -> AES加密
                string vCodekeyJsonStr = JsonHelper.Serialize(new SliderVCodeKeyModel
                {
                    ErrorNum = 0,
                    TS = DateTimeHelper.NowTimeStamp13(),
                    VCodePos = model.VCodePos
                });
                string vCodeKey = _encryptHelper.Encrypt(vCodekeyJsonStr, _options.EncryptKey);
                // 答案 保存到 此次用户会话对应的 Cache 中
                _cacheHelper.Insert<string>(CachePrefixVCodeKey + userId, vCodeKey);
                // 保存验证类型
                _cacheHelper.Insert<string>(CachePrefixCaptchaType + userId, "slider");
            }
            catch (Exception ex)
            {
                rtnResult.code = -1;
                rtnResult.message = "获取验证码失败";

                _logHelper?.Write(ex.ToString());
            }

            VCodeResponseModel vCodeResponseModel = (VCodeResponseModel)rtnResult;

            // TODO: 在.net framework 4.0下未测试
            // Task 参考: https://www.cnblogs.com/yaopengfei/p/8183530.html
#if NETFULL40
            return Task.Factory.StartNew(() => { return rtnResult; });
#else
            return Task.FromResult(vCodeResponseModel);
#endif
        }

        protected override VCodeImgModel CreateVCodeImg()
        {
            SliderVCodeImgModel rtnResult = new SliderVCodeImgModel();
            rtnResult = VCodeImage.Create();

            return rtnResult;
        }

        protected override VCodeKeyModel VCodeKeyModelJsonModel(string jsonStr)
        {
            VCodeKeyModel vCodeKeyModel = JsonHelper.Deserialize<SliderVCodeKeyModel>(jsonStr);

            return vCodeKeyModel;
        }

        protected override bool Verify(VCodeKeyModel vCodeKeyModel, VerifyInfoModel verifyInfoModel)
        {
            SliderVCodeKeyModel vCodeKey = (SliderVCodeKeyModel)vCodeKeyModel;
            SliderVerifyInfoModel verifyInfo = (SliderVerifyInfoModel)verifyInfoModel;
            int allowOffset = this._options.Slider.AllowOffset;

            // 验证码是否正确
            bool isPass = true;
            // Y轴平方差
            int avgY = verifyInfo.TrackPoints.Select(m => m.Y).Sum() / verifyInfo.TrackPoints.Count;
            double stddeY = verifyInfo.TrackPoints.Select(m => Math.Pow(m.Y - avgY, 2)).Sum() / verifyInfo.TrackPoints.Count;
            if (stddeY == 0)
            {
                isPass = false;
            }
            if (verifyInfo.VCodePos.Y != vCodeKey.VCodePos.Y || verifyInfo.VCodePos.X - vCodeKey.VCodePos.X > allowOffset)
            {
                isPass = false;
            }

            return isPass;
        }
    }
}
